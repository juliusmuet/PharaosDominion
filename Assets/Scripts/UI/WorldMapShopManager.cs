using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapShopManager : MonoBehaviour
{

    public static WorldMapShopManager Instance {  get; private set; }  

    [SerializeField] private GameObject upgradeMenu;

    private void Awake() {
        Instance = this;
    }

    public void Open() {
        upgradeMenu.SetActive(true);
        MapNode.nodesInteractable = false;

        AudioManager.instance.PlayBGM(AudioManager.instance.audioUpgradeMenuBGM);
        AudioManager.instance.randomSoundFX.StopRandomSoundFX();
    }
}
