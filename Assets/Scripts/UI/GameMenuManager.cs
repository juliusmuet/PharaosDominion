using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] public GameObject waveProgressDisplay;
    [SerializeField] private GameObject UpgradeMenu;
    [SerializeField] private AudioClip UpgradeMenuAudioClip;

    [HideInInspector] public bool isPaused;
    [HideInInspector] public bool isGameOver;

    public static GameMenuManager instance; //singleton

    void Start()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        ResumeGame();

        gameOverMenu.SetActive(false);
        isGameOver = false;
    }

    void Update()
    {
        if (isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    private void OnEnable()
    {
        Entity.OnEntityKilled.AddListener(PlayerDeath);
    }

    private void OnDisable()
    {
        Entity.OnEntityKilled.RemoveListener(PlayerDeath);
    }

    private void OnDestroy()
    {
        Entity.OnEntityKilled.RemoveListener(PlayerDeath);
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void GoToMainMenu()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowUpgradeMenu()
    {
        waveProgressDisplay.SetActive(false);
        UpgradeMenu.SetActive(true);

        AudioManager.instance.PlayBGM(AudioManager.instance.audioUpgradeMenuBGM);
        AudioManager.instance.randomSoundFX.StopRandomSoundFX();
    }

    private void PlayerDeath(Entity entity)
    {
        if (!entity.gameObject.CompareTag("Player")) return;

        isPaused = true;
        Time.timeScale = 0f;

        isGameOver = true;

        //Reset World Map on GameOver
        MapGraph.currentMapGraph = null;

        gameOverMenu.SetActive(true);

        AudioManager.instance.StopBGM();
        AudioManager.instance.PlaySoundFX(AudioManager.instance.audioDeathFX);
        AudioManager.instance.randomSoundFX.StopRandomSoundFX();
    }
}
