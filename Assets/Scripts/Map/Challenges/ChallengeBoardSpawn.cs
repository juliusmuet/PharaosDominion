using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeBoardSpawn : MonoBehaviour
{
    public static ChallengeBoardSpawn Instance { get; private set; }
    [SerializeField] private GameObject challengeBoardPrefab;

    private void Awake() {
        Instance = this;
    }

    public void Spawn() {
        MapNode.nodesInteractable = false;
        GameObject go = Instantiate(challengeBoardPrefab, transform);
        go.GetComponent<ChallengeBordUI>().Open();
    }
}
