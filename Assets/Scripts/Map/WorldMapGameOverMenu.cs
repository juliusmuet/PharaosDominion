using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapGameOverMenu : MonoBehaviour
{
    public static WorldMapGameOverMenu Instance { get; private set; }


    public void Awake() {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void Show() {
        Debug.Log("Game Over, notes not interactable");
        MapNode.nodesInteractable = false;
        GameObjButton.areInteractable = false;
        MapGraph.currentMapGraph = null;
        this.gameObject.SetActive(true);
    }

    public void SwitchToMainMenu() {
        ScreenTransition.Instance.LoadScene("MainMenu");
    }

    public void QuitApplication() {
        Application.Quit();
    }
}
