using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapFactory : MonoBehaviour {
    public static bool isInWorldmap = false;
    public static WorldMapFactory Instance {get ; private set;}

    private void Awake() {
        Instance = this;
        isInWorldmap = true;
    }

    public EnumDataMapping<Sprite, MAP_LOCATION> mapLocationSprites;
    public EnumDataMapping<Sprite, MAP_LOCATION> mapLocationSpritesOutlines;

    public GameObject[] allEnemies;

    public GameObject goldenSnake;

    public GameObject rainingGems;
    public GameObject gemExplosion;

    public List<GameObject> GetRandomEnemies(int amount) {
        if (amount > allEnemies.Length) {
            Debug.LogError("Amount too big!");
            return null;
        }

        List<GameObject> list = new List<GameObject>();
        while (list.Count < amount) {
            GameObject enemy = allEnemies[Random.Range(0, allEnemies.Length)];
            if (!list.Contains(enemy)) {
                list.Add(enemy);
            }
        }
        return list;

    }

}
