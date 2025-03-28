using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCombatAttack : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    private Team team;

    [SerializeField] private Hitbox[] hitboxes;
    [SerializeField] private float range = 0.5f;
    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] private bool splash = false;


    private void Start() {
        ITeam team = transform.parent.GetComponent<Entity>();

        foreach (Hitbox hitbox in hitboxes) {
            hitbox.SetTeam(team);
            hitbox.SetOffset(hitbox.GetOffset() + offset);
        }
    }

    /// <summary>
    /// Check for collision once and disable
    /// </summary>
    public void Update() {
        if (splash) {
            LinkedList<Hitbox> hit = SpacialGrouping.currentGrouping.CollisionWithAll(hitboxes);
            foreach (Hitbox h in hit) {
                h.GetComponent<Entity>()?.Damage(damage);
            }
        } else {
            Hitbox coll = null;
            float leastDistSQ = float.MaxValue;

            foreach (Hitbox h in hitboxes) {
                Hitbox c = SpacialGrouping.currentGrouping.CollisionWith(h);
                if (c != null) {
                    float distSQ = (c.GetCenter() - h.GetCenter()).sqrMagnitude;
                    if (leastDistSQ > distSQ) {
                        leastDistSQ = distSQ;
                        coll = c;
                    }
                }
            }
            if (coll != null) {
                coll.GetComponent<Entity>()?.Damage(damage);
            }

        }

        this.gameObject.SetActive(false);
    }


    public void OnDrawGizmosSelected() {
        Vector3 rangeOffs = new Vector3(range, 0, 0);
        Gizmos.color = Color.green;
        
        foreach (Hitbox h in hitboxes) {
            h.SupressDrawGizmos();
            Gizmos.DrawWireSphere(offset + rangeOffs + transform.position + h.GetOffset(), h.GetRadius());
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(offset + transform.position, offset + transform.position + rangeOffs);
    }
}
