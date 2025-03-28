using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphinxBossTPs : MonoBehaviour
{
    public static SphinxBossTPs Instance { get; private set; }

    public Transform[] tps;

    private void Awake() {
        Instance = this;
    }

   
}
