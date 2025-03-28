using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraphRenderer : MonoBehaviour
{
    public static MapGraphRenderer Instance { get; private set; }

    [SerializeField] private float horizontalNodeDist = 1f;
    [SerializeField] private float verticalNodeDist = 0.4f;
    [SerializeField, Range(0, 1)] private float nodeOffsetFactor = 0.2f;

    [SerializeField] private GameObject lineRendererPrefab;
    [SerializeField] private GameObject mapNodePrefab;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        if (MapGraph.currentMapGraph == null) {
            if (Tutorial.tutorialActivated) {
                GameObjButton.areInteractable = true;
                MapNode.nodesInteractable = false;
                MapGraph.GenerateTutorial();
            } else
                GenerateNewWorldMap();
        }
        CreateRender(MapGraph.currentMapGraph);
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GenerateNewWorldMap();
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            GenerateSpecificMap(MAP_LOCATION.CHALLANGE);
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            GenerateSpecificMap(MAP_LOCATION.EVENT);
        }
    }

    public void GenerateSpecificMap(MAP_LOCATION loc) {
        GameObjButton.areInteractable = true;
        CreateRender(MapGraph.GenerateSpecificLayout(loc));
    }

    public void GenerateNewWorldMap() {
        GameObjButton.areInteractable = true;
        CreateRender(MapGraph.GenerateOfSufficientQuality(9, 3, 2));
    }

    private GameObject CreateMapNode(int track, int layer, MapGraph graph) {
        GameObject newGo = Instantiate(mapNodePrefab);
        newGo.transform.parent = transform;

        if (graph.nodeDisplayOffs[track, layer] == Vector2.zero) {
            if (!Tutorial.tutorialActivated)
                graph.nodeDisplayOffs[track, layer] = new Vector2((Random.value - 0.5f) * horizontalNodeDist, (Random.value - 0.5f) * verticalNodeDist) * nodeOffsetFactor;

            if (graph.offset == Vector2.zero) {
                graph.offset = -0.5f *  new Vector2(horizontalNodeDist * (graph.GetLayers() - 0.5f), verticalNodeDist * (graph.GetTracks() - 0.5f));
            }
        }

        newGo.transform.position = GetNodePosWorld(track, layer, graph);
        MapNode nodeScript = newGo.GetComponent<MapNode>();
        nodeScript.Init(graph, track, layer);
        
        SpriteRenderer spriteRenderer = nodeScript.GetSpriteRenderer();
        
        
        if (!graph.IsNodeReachable(track, layer) && !graph.IsNodeVisited(track, layer)) {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);

        } else if (graph.IsNodeVisited(track, layer)) {
            nodeScript.MarkVisited(true);
        }

        return newGo;
    }

    private GameObject CreateLineRenderer(int fromTrack, int fromLayer, int toTrack, int toLayer, MapGraph graph) {
        GameObject go = Instantiate(lineRendererPrefab, transform);
        LineRenderer lineRenderer = go.GetComponent<LineRenderer>();
        lineRenderer.SetPositions(new Vector3[] {
            GetNodePosWorld(fromTrack, fromLayer, graph),
            GetNodePosWorld(toTrack, toLayer, graph)
        });

        bool isOnPath = graph.IsNodeVisited(fromTrack, fromLayer) && (graph.IsNodeVisited(toTrack, toLayer) || graph.IsNodeReachable(toTrack, toLayer));

        if (!isOnPath) {
            const float alphaFraction = 0.25f;
            Color temp = lineRenderer.startColor;
            temp.a = alphaFraction;
            lineRenderer.startColor = temp;
            temp = lineRenderer.endColor;
            temp.a = alphaFraction;
            lineRenderer.endColor = temp;
        }

        return go;
    }


    private Vector3 GetNodePosWorld(int track, int layer, MapGraph graph) {
        return new Vector3(layer * horizontalNodeDist + graph.nodeDisplayOffs[track, layer].x + graph.offset.x, track * verticalNodeDist + graph.nodeDisplayOffs[track, layer].y + graph.offset.y);
    }

    public void CreateRender(MapGraph graph) {
        MapNode.nodesInteractable = true;
        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }

        for (int layer = 0; layer < graph.GetLayers(); layer++) {
            for (int track = 0; track < graph.GetTracks(); track++) {
                if (graph.nodes[track, layer] == null)
                    continue;

                CreateMapNode(track, layer, graph);
            }
        }

        for (int layer = 0; layer < graph.GetLayers(); layer++) {
            for (int track = 0; track < graph.GetTracks(); track++) {
                for (int nxtTrack = 0; nxtTrack < graph.GetTracks(); nxtTrack++) {
                    if (graph.nodes[track, layer] == null)
                        continue;

                    if (graph.nodes[track, layer].connectTo[nxtTrack])
                        CreateLineRenderer(track, layer, nxtTrack, layer + 1, graph);
                }
            }
        }
    }
}
