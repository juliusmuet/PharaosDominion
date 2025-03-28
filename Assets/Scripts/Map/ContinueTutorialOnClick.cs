using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueTutorialOnClick : MonoBehaviour
{

    public void Update() {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space)) {
            Tutorial.Instance.IncreaseStep();
        }
    }
}
