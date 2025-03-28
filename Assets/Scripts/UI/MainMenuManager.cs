using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    [SerializeField] public GameObject[] playerTeam;

    private void Start()
    {
        PlayerTeamManager.instance.ResetTeam();
        if (ChallengeController.Instance != null)
            ChallengeController.Instance.ResetChallanges();
        //foreach (GameObject teamMember in playerTeam) PlayerTeamManager.instance.AddNPC(teamMember);

        UpgradeManager.instance.ResetAllUpgrades();
        WorldMapEvent.ResetLatestEvents();
        PlayerPrefs.SetInt("TotalMoney", 0);
    }

    private void Update()
    {
        if (Input.anyKeyDown)
            this.GetComponent<Animator>().SetTrigger("anykey");
    }

    public void NewRunClicked() {
        MapGraph.currentMapGraph = null;
        GameObjButton.areInteractable = true;
        Tutorial.tutorialActivated = false;
        ScreenTransition.Instance.LoadScene("WorldMap");
    }

    public void TutorialClicked() {
        GameObjButton.areInteractable = true;
        Tutorial.ResetTutorial();
        MapGraph.currentMapGraph = null;
        Tutorial.tutorialActivated = true;
        ScreenTransition.Instance.LoadScene("WorldMap");
    }
}
