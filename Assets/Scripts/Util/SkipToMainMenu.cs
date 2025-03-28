using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipToMainMenu : MonoBehaviour
{
    [SerializeField] private int mouseclicks = 2;

    public void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (--mouseclicks == 0) {
                MapGraph.currentMapGraph = null;
                ScreenTransition.Instance.LoadScene("Mainmenu");
            }
        }
    }
}
