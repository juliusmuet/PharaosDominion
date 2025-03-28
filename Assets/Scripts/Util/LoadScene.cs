using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void LoadNewScene(string scene)
    {
        ScreenTransition.Instance.LoadScene(scene);
    }

    public void DisableWorldMapUpgradeMenu() {
        transform.parent.gameObject.SetActive(false);

        MapNode.nodesInteractable = true;

        if (Tutorial.tutorialActivated)
            Tutorial.Instance.IncreaseStep();
    }
}
