using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public static Tutorial Instance { get; private set; }
    public static bool tutorialActivated = false;
    private static int currentTutorialStep;
    private static bool fightTutorialFinished = false;


    [SerializeField] private GameObject[] texts;
    [SerializeField] private GameObject[] arrows;

    private void Awake() {
        Instance = this;
    }

    public void Start() {
        if (!tutorialActivated)
            return;

        currentTutorialStep--;
        IncreaseStep();
        
    }

    public void Update() {
        if (!tutorialActivated)
            return;


    }

    public void IncreaseStep() {
        if (currentTutorialStep >= 0) {
            texts[currentTutorialStep].SetActive(false);
            if (arrows[currentTutorialStep] != null)
                arrows[currentTutorialStep].SetActive(false);
        }

        currentTutorialStep++;

        if (currentTutorialStep == 15) {
            ScreenTransition.Instance.LoadScene("MainMenu");

        }

        if (currentTutorialStep >= texts.Length)
            return;

        texts[currentTutorialStep].SetActive(true);
        if (arrows[currentTutorialStep] != null)
            arrows[currentTutorialStep].SetActive(true);

        if (currentTutorialStep == 0) {
            MapNode.nodesInteractable = false;
        }

        if (currentTutorialStep == 2) {
            MapNode.nodesInteractable = true;
        }

        
    }

    public static int GetCurrentStep() {
        return currentTutorialStep;
    }

    public static bool GetFightTutorialFinished() {
        return fightTutorialFinished;
    }

    public static void SetFightTutorialFinished() {
        fightTutorialFinished = true;
    }

    public static void ResetTutorial() {
        currentTutorialStep = 0;
        fightTutorialFinished = false;
    }
}
