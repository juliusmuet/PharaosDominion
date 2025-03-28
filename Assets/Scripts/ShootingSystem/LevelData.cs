using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelData : MonoBehaviour
{
    public static LevelData Instance {  get; private set; }

    [SerializeField] private Tilemap walls;

    private BoundsInt tilemapBoundsCellSpace;
    private Bounds tilemapBoundsWorldSpace;
    public void Awake() {
        //Debug.Log("AWAKE LVL DATA");
        Instance = this;

        walls.CompressBounds();
        tilemapBoundsCellSpace = walls.cellBounds;
        tilemapBoundsWorldSpace = walls.localBounds;

        SpacialGrouping.currentGrouping = new SpacialGrouping(4);
    }

    public BoundsInt GetLevelBoundsCellSpace() {
        return tilemapBoundsCellSpace;
    }

    public Bounds GetLevelBoundsWorldSpace() {
        return tilemapBoundsWorldSpace;
    }

    public void OnDrawGizmosSelected() {
        SpacialGrouping.currentGrouping.DrawGizmos();
    }

}
