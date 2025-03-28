using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeamManager : MonoBehaviour
{
    public static readonly int MAX_TEAM_COUNT = 10;
    public static PlayerTeamManager instance;   //singleton instance

    [SerializeField] private GameObject playerPrefab;
    public int playerCurrentHP = -1;
    public int playerMaxHp = -1;
    public int playerBaseHp = -1;
    public bool isMummyCursed = false;

    public List<PlayerTeamNPCData> playerTeamNPCs;    //NPCs in player's team

    private void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); //keep singleton in new scenes

        playerBaseHp = playerPrefab.GetComponent<Entity>().GetBaseMaxHp();
        playerMaxHp = playerPrefab.GetComponent<Entity>().GetMaxHP();
        playerCurrentHP = playerMaxHp;



    }

    public List<PlayerTeamNPCData> GetSnakes() {
        List<PlayerTeamNPCData> result = new List<PlayerTeamNPCData>();
        foreach (PlayerTeamNPCData data in playerTeamNPCs) {
            if (data.prefab.name.ToLower().Contains("snake"))
                result.Add(data);
        }
        return result;
    }

    public void ReplaceAllSnakesWithGolden_WORLDMAP() {
        var snakes = GetSnakes();
        foreach (var snake in snakes) {
            playerTeamNPCs.Remove(snake);
        }
        for (int i = 0; i < snakes.Count; i++) {
            AddNPC(WorldMapFactory.Instance.goldenSnake);
        }
    }

    private void OnEnable()
    {
        Entity.OnEntityKilled.AddListener(RemoveNPC);
    }

    private void OnDisable()
    {
        Entity.OnEntityKilled.RemoveListener(RemoveNPC);
    }

    private void OnDestroy()
    {
        Entity.OnEntityKilled.RemoveListener(RemoveNPC);
    }

    private void RemoveNPC(Entity npc)
    {
        if (npc.GetTeam() != Team.PLAYER) return;

        //find corresponding prefab based on dead NPC gameObject
        PlayerTeamNPCData dataToRemove = playerTeamNPCs.Find(playerTeamNPCData => playerTeamNPCData.instance == npc.gameObject);

        if (dataToRemove != null)
        {
            playerTeamNPCs.Remove(dataToRemove);
            Debug.Log($"Removed {dataToRemove.prefab.name} from player team due to death.");
        }
    }

    public void ResetTeam()
    {
        playerTeamNPCs.Clear();
        playerCurrentHP = playerMaxHp;
        isMummyCursed = false;
    }

    public void AddNPC(GameObject npcPrefab)
    {
        PlayerTeamNPCData newNpcData = new PlayerTeamNPCData
        {
            prefab = npcPrefab,
            health = npcPrefab.GetComponent<Entity>().GetMaxHP(),
            instance = null
        };

        playerTeamNPCs.Add(newNpcData);
    }

    /*
    private void RemoveNPC(Entity npc)
    {
        if (npc.GetTeam() != Team.PLAYER) return;

        //find corresponding prefab based on dead NPC gameObject
        GameObject prefabToRemove = null;
        foreach (GameObject prefab in playerTeamNPCs)
        {
            if (prefab.name == npc.gameObject.name.Replace("(Clone)", "").Trim())
            {
                prefabToRemove = prefab;
                break;
            }
        }

        if (prefabToRemove != null)
        {
            playerTeamNPCs.Remove(prefabToRemove);
            Debug.Log($"Removed {prefabToRemove.name} from playerTeamPrefabs due to death.");
        }
    }
    */
}
