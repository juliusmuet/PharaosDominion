using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraph
{
    public static MapGraph currentMapGraph = null;

    public static readonly float[] CHILD_PROPABILITIES = new float[] { 1f, 0.5f, 0.25f, -1f};

    public MapGraphNode[,] nodes; //track,layer
    public Vector2[,] nodeDisplayOffs; //track, layer (Only for display);
    public Vector2 offset;

    private int tracks, layers;

    private MapGraphNode[] visitedNodes;
    private int currentLayer = -1;

    private MapGraph(int tracks, int layers) {
        this.tracks = tracks;
        this.layers = layers;
        nodes = new MapGraphNode[tracks, layers];
        nodeDisplayOffs = new Vector2[tracks, layers];
        visitedNodes = new MapGraphNode[layers];
    }


    public bool QualityAssureMapGraph() {
        int minNodes = (int)(tracks * (layers - 2) / 1.5f);

        //Check Min Nodes
        int totalNodes = 0;
        for (int ix = 0; ix < tracks; ix++) {
            for (int iy = 0; iy < layers; iy++) {
                if (nodes[ix, iy] != null)
                    totalNodes++;
            }
        }

        if (totalNodes < minNodes) {
            Debug.Log("MapGraph of low quality: Too few nodes!");
            return false;
        }

        //Check Big "Holes" on tracks
        const int maxHoleSize = 3;
        for (int ix = 0; ix < tracks; ix++) {
            int currentHole = 0;
            for (int iy = 0; iy < layers; iy++) {
                if (nodes[ix, iy] == null)
                    currentHole++;
                else
                    currentHole = 0;
                if (currentHole > maxHoleSize) {
                    Debug.Log("MapGraph of low quality: Too big Holes!");
                    return false;
                }
            }
        }
        return true;
    }

    public void PlaceLocations() {
        for (int track = 0; track < tracks; track++) {
            for (int layer = 0; layer < layers; layer++) {
                if (nodes[track, layer] == null)
                    continue;

                ChanceSystem<MAP_LOCATION> sys = new ChanceSystem<MAP_LOCATION>();


                if (layer >= 1) {
                    sys.AddItem(MAP_LOCATION.FIGHT, (int)(MAP_LOCATION.FIGHT.GetProbability() * LocationProbablityDiscountOnEqualLocationBefore(MAP_LOCATION.FIGHT, track, layer)));
                    sys.AddItem(MAP_LOCATION.SHOP, (int)(MAP_LOCATION.SHOP.GetProbability() * LocationProbablityDiscountOnEqualLocationBefore(MAP_LOCATION.SHOP, track, layer)));
                    sys.AddItem(MAP_LOCATION.EVENT, (int)(MAP_LOCATION.EVENT.GetProbability() * LocationProbablityDiscountOnEqualLocationBefore(MAP_LOCATION.EVENT, track, layer)));
                    sys.AddItem(MAP_LOCATION.CHALLANGE, (int)(MAP_LOCATION.CHALLANGE.GetProbability() * LocationProbablityDiscountOnEqualLocationBefore(MAP_LOCATION.CHALLANGE, track, layer)));
                } else {
                    sys.AddItem(MAP_LOCATION.FIGHT, MAP_LOCATION.FIGHT.GetProbability());
                }

                MAP_LOCATION loc = sys.Generate();

                if (layer == layers - 1)
                    loc = MAP_LOCATION.BOSS;

                nodes[track, layer].location = loc;

            }
        }
    }

    private float LocationProbablityDiscountOnEqualLocationBefore(MAP_LOCATION loc, int track, int layer) {
        float multiplicator = 1f;
        for(int t = 0; t < tracks; t++) {
            MapGraphNode node = nodes[t, layer - 1];
            if (node != null && node.connectTo[track] && node.location == loc)
                multiplicator *= 0.33f;
        }
        return multiplicator;
    }

    private void PlaceInitialTracks(int amount) {
        
        while (amount > 0) {
            int pos = Random.Range(0, tracks);
            if (nodes[pos, 0] == null) {
                amount--;
                nodes[pos, 0] = new MapGraphNode(tracks);
            }
        } 
    }

    private void PlaceLastLayer() {
        int position = (int)(tracks / 2);
        
        nodes[position, layers - 1] = new MapGraphNode(tracks);
        nodes[position, layers - 1].location = MAP_LOCATION.BOSS;
        for (int i = 0; i < tracks; i++) {
            if (nodes[i, layers - 2] != null)
                nodes[i, layers - 2].connectTo[position] = true;
        }
    }

    private void FillLayer(int layer) {

        for(int track = 0; track < tracks; track++) {
            if (nodes[track, layer - 1] == null)
                continue;

            MapGraphNode currentNode = nodes[track, layer - 1];

            int children = 0;
            int[] childTracks = new int[] { -1, -1, -1 };
            while (Random.value < CHILD_PROPABILITIES[children]) {
                if (children == 2 && (track == 0 || track == tracks - 1))
                    break;

                bool found;
                do {
                    childTracks[children] = Random.Range(Mathf.Max(0, track - 1), Mathf.Min(track + 2, tracks)); // Check this

                    found = true;
                    for (int i = 0; i < children; i++) {
                        if (childTracks[i] == childTracks[children]) {
                            found = false;
                            break;
                        }
                    }

                } while (!found);


                children++;
            }
            
            for(int i = 0; i < children; i++) {
                if (nodes[childTracks[i], layer] == null) {
                    nodes[childTracks[i], layer] = new MapGraphNode(tracks);
                }

                currentNode.connectTo[childTracks[i]] = true;
            }

        }
    }

    public void MarkNodeAsVisited(int track, int layer) {
        visitedNodes[layer] = nodes[track, layer];
        for (int i = 0; i < layers; i++) {
            if (visitedNodes[i] == null) {
                currentLayer = i - 1;
                return;
            }
        }
        currentLayer = layers;
    }

    public bool IsNodeVisited(int track, int layer) {
        return visitedNodes[layer] == nodes[track, layer];
    }

    public bool IsNodeReachable(int track, int layer) {
        if (currentLayer == -1) {
            return layer == 0 && nodes[track, 0] != null;
        }
        return layer == currentLayer + 1 && visitedNodes[currentLayer].connectTo[track];
    }

    public static MapGraph GenerateOfSufficientQuality(int layers, int tracks, int inititalTracks = 3) { 
        MapGraph graph;

        do {
            graph = Generate(layers, tracks, inititalTracks);
        } while (!graph.QualityAssureMapGraph());
        return graph;

    }

    public static MapGraph GenerateSpecificLayout(MAP_LOCATION loc) {
        const int tracks = 2;
        const int layers = 9;
        MapGraph graph = new MapGraph(tracks, layers);

        for(int ix = 0; ix < 8; ix++) {
            graph.nodes[0, ix] = new MapGraphNode(tracks);
            graph.nodes[0, ix].connectTo[0] = true;
            graph.nodes[0, ix].connectTo[1] = true;
            graph.nodes[0, ix].location = loc;
            graph.nodes[1, ix] = new MapGraphNode(tracks);
            graph.nodes[1, ix].connectTo[0] = true;
            graph.nodes[1, ix].connectTo[1] = true;
            graph.nodes[1, ix].location = MAP_LOCATION.FIGHT;
        }

        graph.PlaceLastLayer();
        currentMapGraph = graph;

        return graph;
    }

    public static MapGraph GenerateTutorial() {
        const int tracks = 1;
        MapGraph graph = new MapGraph(tracks, 5);
        graph.nodes[0, 0] = new MapGraphNode(tracks);
        graph.nodes[0, 0].connectTo[0] = true;
        graph.nodes[0, 0].location = MAP_LOCATION.FIGHT;

        graph.nodes[0, 1] = new MapGraphNode(tracks);
        graph.nodes[0, 1].connectTo[0] = true;
        graph.nodes[0, 1].location = MAP_LOCATION.EVENT;

        graph.nodes[0, 2] = new MapGraphNode(tracks);
        graph.nodes[0, 2].connectTo[0] = true;
        graph.nodes[0, 2].location = MAP_LOCATION.SHOP;

        graph.nodes[0, 3] = new MapGraphNode(tracks);
        graph.nodes[0, 3].connectTo[0] = true;
        graph.nodes[0, 3].location = MAP_LOCATION.CHALLANGE;

        graph.nodes[0, 4] = new MapGraphNode(tracks);
        //graph.nodes[0, 4].connectTo[0] = true;
        graph.nodes[0, 4].location = MAP_LOCATION.FIGHT;

        currentMapGraph = graph;

        return graph;
    }

    public static MapGraph Generate(int layers, int tracks, int inititalTracks = 3) {
        MapGraph graph = new MapGraph(tracks, layers);

        if (tracks < inititalTracks) {
            Debug.LogError("Number of tracks must be greater or equal than the number of initial tracks!");
            return null;
        }
        
        graph.PlaceInitialTracks(inititalTracks);
        for (int i = 1; i < layers - 1; i++) {
            graph.FillLayer(i);
        }

        graph.PlaceLastLayer();

        graph.PlaceLocations();

        currentMapGraph = graph;

        return graph;
    }

    public int GetTracks() {
        return tracks;
    }

    public int GetLayers() {
        return layers;
    }

    public class MapGraphNode {
        public bool[] connectTo;
        public MAP_LOCATION location;

        public MapGraphNode(int tracks) {
            connectTo = new bool[tracks];
        }
    }
}



public enum MAP_LOCATION { FIGHT, ELITE, SHOP, EVENT, CHALLANGE, BOSS }

public static class MapLocationExtension {
    public static int GetProbability(this MAP_LOCATION location) {
        switch (location) {
            case MAP_LOCATION.FIGHT:
                return 10;
            case MAP_LOCATION.ELITE:

                return 0;
            case MAP_LOCATION.SHOP:

                return 2;

            case MAP_LOCATION.EVENT:

                return 4;

            case MAP_LOCATION.CHALLANGE:

                return 2;
            case MAP_LOCATION.BOSS:

                return 0;
            default:
                Debug.Log("This should not happen");
                return 0;
        }
    }
}
