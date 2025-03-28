using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public static bool nodesInteractable = true;

    public static readonly string IS_HOVERED_ANIM_BOOL = "is_hovered";

    [SerializeField] private float triggerRadiusSQ = 0.4f;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private SpriteRenderer visitedMarkSpriteRenderer;

    private int track, layer;
    private MapGraph graph;

    public void Init(MapGraph graph, int track, int layer) {
        this.graph = graph;
        this.track = track;
        this.layer = layer;

        spriteRenderer.sprite = WorldMapFactory.Instance.mapLocationSprites[graph.nodes[track, layer].location];
        outline.sprite = WorldMapFactory.Instance.mapLocationSpritesOutlines[graph.nodes[track, layer].location];

        if (graph.IsNodeReachable(track, layer)) {
            outline.enabled = true;
            spriteRenderer.transform.localScale *= 1.25f;
        }
    }


    public void Update() {
        Vector3 mouse = CameraController.Instance.GetMouseWorld();

        if ((transform.position.x - mouse.x) * (transform.position.x - mouse.x) + (transform.position.y - mouse.y) * (transform.position.y - mouse.y) <= triggerRadiusSQ) {
            anim.SetBool(IS_HOVERED_ANIM_BOOL, true);

            if (nodesInteractable && Input.GetMouseButtonDown(0) && graph.IsNodeReachable(track, layer)) {
                graph.MarkNodeAsVisited(track, layer);
                
                AudioManager.instance.PlaySoundFX(AudioManager.instance.audioButtonFX);
                //if (graph.nodes[track, layer].location )


                MapGraphRenderer.Instance.CreateRender(MapGraph.currentMapGraph);
                if (Tutorial.tutorialActivated)
                    Tutorial.Instance.IncreaseStep();

                if (Input.GetKey(KeyCode.LeftShift))
                    return;

                switch (graph.nodes[track, layer].location) {
                    case MAP_LOCATION.CHALLANGE:
                        ChallengeBoardSpawn.Instance.Spawn();
                        break;
                    case MAP_LOCATION.EVENT:
                        WorldMapEventManager.Instance.Open();
                        break;
                    case MAP_LOCATION.SHOP:
                        WorldMapShopManager.Instance.Open();
                        break;
                    case MAP_LOCATION.FIGHT:
                        WorldMapFactory.isInWorldmap = false;
                        
                        int difficulty = (int)(layer / (float)MapGraph.currentMapGraph.GetLayers() * 3);
                        if (Tutorial.tutorialActivated)
                            difficulty = 0;

                        PlayerPrefs.SetInt("Difficulty", difficulty);
                        Debug.Log($"Difficulty: {difficulty}");

                        int[] fightSceneIDs = Util.GetSceneIDByScenePrefix("level");
                        ScreenTransition.Instance.LoadScene(fightSceneIDs[UnityEngine.Random.Range(0, fightSceneIDs.Length)]);
                        break;

                    case MAP_LOCATION.BOSS:
                        WorldMapFactory.isInWorldmap = false;
                        ScreenTransition.Instance.LoadScene("Boss_Sphinx");
                        break;
                    default:
                        Debug.Log("Selected valid Node!");
                        break;
                }
            }

        } else {
            anim.SetBool(IS_HOVERED_ANIM_BOOL, false);
        }
    }

    public void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, Mathf.Sqrt(triggerRadiusSQ));
    }

    public SpriteRenderer GetSpriteRenderer() { 
        return spriteRenderer;
    }

    public void MarkVisited(bool flag) {
        visitedMarkSpriteRenderer.enabled = flag;
    }
}
