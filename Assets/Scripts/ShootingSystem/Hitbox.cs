using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Hitbox : MonoBehaviour
{

    private Transform _transform;
    [SerializeField] private float radius = 1f;
    [SerializeField] private Vector3 offset;
    [SerializeField] private ITeam team;

    private bool supressDrawGizmos = false;

    private void Awake() {
        _transform = transform;
    }
    public bool PointInside(Vector3 point) {
        return Vector3.Distance(point, GetCenter()) <= radius;
    }

    public bool Collide(Hitbox hitbox) {
        return hitbox.team.GetTeam() != this.team.GetTeam() && Vector3.Distance(hitbox.GetCenter(), GetCenter()) <= radius + hitbox.radius;
    }

    public Vector3 GetCenter() {
        if (_transform == null) return new Vector3(-9999, -9999);
        return _transform.position + offset;
    }

    public Team GetTeam() {
        return team.GetTeam();
    }

    public void SetTeam(ITeam team) {
        this.team = team;
    }

    public float GetRadius() {
        return radius;
    }

    public Vector3 GetOffset() {
        return offset;  
    }

    public void SetOffset(Vector3 offs) {
        this.offset = offs;
    }

    public void SupressDrawGizmos() {
        supressDrawGizmos = true;
    }

    public void OnDrawGizmosSelected() {
        if (!supressDrawGizmos) 
            Gizmos.DrawWireSphere(transform.position + offset, radius);
    }
}
/// <summary>
/// ASSUMING CELLW AND CELLH ARE BIGGER THAN ANY HITBOX DIAMETER!
/// </summary>
public class SpacialGrouping {
    public static SpacialGrouping currentGrouping = null;

    private int cellSize;
    private int gridW, gridH;

    private Vector3 gridOffset;

    private LinkedList<Hitbox>[,] buckets;
    public SpacialGrouping(int cellSize) {
        this.cellSize = cellSize;

        Bounds levelBounds = LevelData.Instance.GetLevelBoundsWorldSpace();

        gridOffset = levelBounds.min;

        gridW = Mathf.CeilToInt(levelBounds.size.x / (float)cellSize);
        gridH = Mathf.CeilToInt(levelBounds.size.y / (float)cellSize);

        buckets = new LinkedList<Hitbox>[gridW, gridH];
        for (int ix = 0; ix < gridW; ix++) {
            for (int iy = 0; iy < gridH; iy++) {
                buckets[ix, iy] = new LinkedList<Hitbox>();
            }
        }
    }

    public (int, int) GetBucket(Hitbox hitbox) {
        Vector3 center = hitbox.GetCenter() - gridOffset;
        return ((int)(Mathf.Clamp(center.x, 0, cellSize * (gridW - 1)) / cellSize), (int)(Mathf.Clamp(center.y, 0, cellSize * (gridH - 1)) / cellSize));
    }

    public (int, int) GetBucket(Vector3 point) {
        point -= gridOffset;
        return ((int)(Mathf.Clamp(point.x, 0, cellSize * (gridW - 1)) / cellSize), (int)(Mathf.Clamp(point.y, 0, cellSize * (gridH - 1)) / cellSize));
    }

    public (int bucketX, int bucketY) Add(Hitbox hitbox) {
        (int bucketX, int bucketY) = GetBucket(hitbox);

        //Debug.Log($"UPDATE TO: ({bucketX}, {bucketY})");
        buckets[bucketX, bucketY].AddLast(hitbox);
        return (bucketX, bucketY);
    }

    public void Remove(Hitbox hitbox) {
        (int bucketX, int bucketY) = GetBucket(hitbox);
        buckets[bucketX, bucketY].Remove(hitbox);
    }

    public void Remove(Hitbox hitbox, (int bucketX, int bucketY) data) {
        buckets[data.bucketX, data.bucketY].Remove(hitbox);
    }

    public LinkedList<Hitbox> CollisionWithAll(Hitbox hitbox) {
        LinkedList<Hitbox> res = new LinkedList<Hitbox>();
        (int bucketX, int bucketY) = GetBucket(hitbox);
        var buckets = GetNeigbourBuckets(bucketX, bucketY, 1 + (int)(hitbox.GetRadius() / cellSize));
        foreach (var bucket in buckets) {
            foreach (Hitbox h in bucket) {
                if (h.Collide(hitbox)) {
                    res.AddLast(h);
                }
            }
        }

        return res;
    }

    public LinkedList<Hitbox> CollisionWithAll(Hitbox[] hitbox) {
        LinkedList<Hitbox> res = new LinkedList<Hitbox>();

        foreach (Hitbox h in hitbox) {
            foreach (Hitbox h2 in CollisionWithAll(h))
                res.AddLast(h2);
        }
        return res;
    }

    public Hitbox GetClosestHitbox(Vector3 point, float maxRadius, Team ofTeam) {

        Hitbox bestHitbox = null;
        float distance = maxRadius;


        (int x, int y) bucketData = GetBucket(point);
        for(int i = 0; i < Mathf.CeilToInt(maxRadius / (cellSize / 2f)); i++) {
            LinkedList<LinkedList<Hitbox>> allBuckets = new LinkedList<LinkedList<Hitbox>>();
            
            if (i == 0) {
                allBuckets.AddLast(buckets[bucketData.x, bucketData.y]);
            } else {
                //Select Only Buckets in Range i (2, 3, 4, 5, ...)
                int maxIX = Mathf.Min(gridW - 1, bucketData.x + i);
                int maxIY = Mathf.Min(gridH - 1, bucketData.y + i);
                int minIX = Mathf.Max(0, bucketData.x - i);
                int minIY = Mathf.Max(0, bucketData.y - i);
                //TOP&BOT
                for (int ix = minIX; ix <= maxIX; ix++) {
                    allBuckets.AddLast(buckets[ix, minIY]);
                    //Debug.Log($"buckets[ix, maxIY] {ix}, {maxIY}");
                    allBuckets.AddLast(buckets[ix, maxIY]);
                }
                //LEFT&RIGHT
                for (int iy = minIY + 1; iy <= maxIY - 1; iy++) {
                    allBuckets.AddLast(buckets[minIX, iy]);
                    allBuckets.AddLast(buckets[maxIX, iy]);
                }
            }

            foreach (var bucket in allBuckets) {
                foreach (Hitbox hitbox in bucket) {
                    if (hitbox.GetTeam() != ofTeam)
                        continue;

                    float currentDistance = (hitbox.GetCenter() - point).magnitude - hitbox.GetRadius();
                    if (currentDistance < distance) {
                        distance = currentDistance;
                        bestHitbox = hitbox;
                    }
                }
            }
            if (bestHitbox != null) break;
        }

        //Debug.Log(bestHitbox);
        return bestHitbox;
    }


    public Hitbox CollisionWith(Hitbox hitbox) {
        (int bucketX, int bucketY) = GetBucket(hitbox);
        var bucketsList = GetNeigbourBuckets(bucketX, bucketY, 1 + (int)(hitbox.GetRadius() / cellSize));
        foreach (var bucket in bucketsList) {
            foreach (Hitbox h in bucket) {
                if (h.Collide(hitbox)) {
                    return h;
                }
            }
        }
        return null;
    }

    private LinkedList<LinkedList<Hitbox>> GetNeigbourBuckets(int x, int y, int range) {
        LinkedList<LinkedList<Hitbox>> res = new LinkedList<LinkedList<Hitbox>>();

        int maxIX = Mathf.Min(gridW, x + range);
        int maxIY = Mathf.Min(gridH, y + range);
        int minIY = Mathf.Max(0, y - range);
        for(int ix = Mathf.Max(0, x - range); ix < maxIX; ix++) {
            for(int iy = minIY; iy < maxIY; iy ++) {
                res.AddLast(buckets[ix, iy]);
            }
        }
        return res;
    }

    public (int bucketX, int bucketY) UpdateHitboxBucket((int bucketX, int bucketY) oldData, Hitbox hitbox) {
        (int bucketX, int bucketY) = GetBucket(hitbox);
        if (oldData.bucketX != bucketX || oldData.bucketY != bucketY) {
            Remove(hitbox, oldData);
            //Debug.Log($"UPDATE TO: ({data.bucketX}, {data.bucketY})");
            return Add(hitbox);
        }
        return oldData;
    }

    public void DrawGizmos() {
        for (int ix = 0; ix < gridW; ix++) {
            for (int iy = 0; iy < gridH; iy++) {
                Gizmos.color = new Color(Mathf.Sin(ix) * .5f + 0.5f, Mathf.Cos(iy) * .5f + 0.5f, ((ix + iy) / 2) / ((gridH + gridW) * .5f));
                Gizmos.DrawWireCube(new Vector3(ix + .5f, iy + .5f) * cellSize + gridOffset, new Vector3(cellSize, cellSize, .1f) * 0.95f);
                foreach (Hitbox hb in buckets[ix, iy]) {
                    hb.OnDrawGizmosSelected();
                }
            }
        }
    }
}
