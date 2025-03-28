using System;
using UnityEngine;

[Serializable]
public class PlayerTeamNPCData
{
    public GameObject prefab;  //NPC prefab
    public int health;         //health of NPC
    [NonSerialized] public GameObject instance;  //instantiated NPC, needed for deletion (see PlayerTeamManager)
}