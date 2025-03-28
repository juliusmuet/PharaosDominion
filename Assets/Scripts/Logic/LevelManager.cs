using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static bool levelFinished = false;

    public enum Difficulty { Easy, Medium, Hard, BOSS}
    public Difficulty difficulty;

    public List<GameObject> enemiesEasy; //list of all enemies for easy level
    public List<GameObject> enemiesMedium; //list of all enemies for medium level
    public List<GameObject> enemiesHard; //list of all enemies for hard level
    public GameObject enemyBoss;
    [System.NonSerialized] public List<GameObject> enemies; //list of all enemies for the level
    public List<Transform> enemyPositions; //positions for enemies to spawn
    public List<Transform> playerTeamPositions;    //positions for NPCs to spawn
    public float timeBetweenWaves = 2.0f; //delay between waves

    public AudioClip audioBGMEasy;
    public AudioClip audioBGMMedium;
    public AudioClip audioBGMHard;

    [SerializeField] private GameObject playerTeamSpawnEffect;
    [SerializeField] private GameObject enemySpawnEffect;

    [Header("Tutorial")]
    [SerializeField] private GameObject wasdInfo;
    [SerializeField] private GameObject dodgingInfo;
    [SerializeField] private GameObject shootingInfo;
    [SerializeField] private GameObject startInfo;

    private GameObject waveMenu;
    private TMP_Text waveText;
    private Image waveProgressImage;
    private Image waveProgressBackgroundImage;
    private Coroutine waveProgressUpdateCoroutine;
    private float textUpdateDuration = 1f;    //duration for interpolation in seconds
    private int enemiesInCurrentWave = 0;

    private int enemiesPerWave; //number of enemies to spawn per wave
    private int currentWave = 0; //track current wave number
    private int totalWaves; //total number of waves

    private int remainingEnemies; //number of remaining enemies in current wave
    private int currentEnemyIndex = 0; //index to track the next enemy to spawn in waves
    private float spawnDelay = 0.25f;   //delay between spawn of enemies

    public static LevelManager instance; //singleton instance
    
    private bool bossHP_handleFilled = false;
    private GameObject spawnedBoss = null;
    void Awake()
    {
        bossHP_handleFilled = false;
        levelFinished = false;

        //singelton pattern
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

    }

    void Start()
    {
        //load difficulty
        if (difficulty != Difficulty.BOSS)
        {
            int difficultyInt = PlayerPrefs.GetInt("Difficulty", 0);
            difficulty = (Difficulty)difficultyInt;
        }

        //set enemies and background music according to difficulty
        if (difficulty == Difficulty.Easy)
        {
            AudioManager.instance.PlayBGM(audioBGMEasy);
            GlobalLightController.instance.SetGlobalLightIntensity(0.75f);
            enemies = enemiesEasy;
        } 
        else if (difficulty == Difficulty.Medium)
        {
            AudioManager.instance.PlayBGM(audioBGMMedium);
            GlobalLightController.instance.SetGlobalLightIntensity(0.5f);
            enemies = enemiesMedium;
        }  
        else if (difficulty == Difficulty.Hard)
        {
            AudioManager.instance.PlayBGM(audioBGMHard);
            GlobalLightController.instance.SetGlobalLightIntensity(0.35f);
            enemies = enemiesHard;
        } 
        else if (difficulty == Difficulty.BOSS)
        {
            AudioManager.instance.PlayBGM(audioBGMHard);
            GlobalLightController.instance.SetGlobalLightIntensity(0.5f);
            enemies = new List<GameObject>();
            enemies.Add(enemyBoss);
        }


        //ensure NPCManager exists and assign NPCs
        if (PlayerTeamManager.instance == null)
        {
            Debug.LogWarning("NPCManager is missing.");
            return;
        }
        
        //set variables
        enemiesPerWave = enemyPositions.Count;
        totalWaves = Mathf.CeilToInt((float)enemies.Count / enemiesPerWave);
    }

    private void Update()
    {
        //set UI variables
        if (!waveMenu)
        {
            waveMenu = GameMenuManager.instance.waveProgressDisplay;
            waveMenu.SetActive(false);
            waveText = waveMenu.GetComponentInChildren<TMP_Text>();
            waveProgressBackgroundImage = waveMenu.GetComponentsInChildren<Image>(true)[0];
            waveProgressImage = waveMenu.GetComponentsInChildren<Image>(true)[1];
            waveProgressImage.fillAmount = 0;
            UpdateWaveNumberUI();
            //StartWaveProgressUpdate();
        }
    }

    private void OnEnable()
    {
        Entity.OnEntityKilled.AddListener(HandleEnemyKilled);
    }

    private void OnDisable()
    {
        Entity.OnEntityKilled.RemoveListener(HandleEnemyKilled);
    }

    private void OnDestroy()
    {
        Entity.OnEntityKilled.RemoveListener(HandleEnemyKilled);
    }

    public void StartLevel()
    {
        //assign NPCs to positions
        AssignPlayerTeamToPositions(PlayerTeamManager.instance.playerTeamNPCs, playerTeamPositions, Team.PLAYER);

        waveMenu.SetActive(true);

        //start first wave
        StartNextWave();
    }

    private void AssignPlayerTeamToPositions(List<PlayerTeamNPCData> npcDatas, List<Transform> positions, Team team)
    {
        //copy positions for random selection
        List<Transform> availablePositions = new List<Transform>(positions);

        for (int i = 0; i < npcDatas.Count && availablePositions.Count > 0; i++)
        {
            //select random position from available positions
            int randomIndex = Random.Range(0, availablePositions.Count);
            Transform spawnPosition = availablePositions[randomIndex];

            //instantiate prefab
            PlayerTeamNPCData npcData = npcDatas[i];
            GameObject newEntity = Instantiate(npcData.prefab, spawnPosition.position, Quaternion.identity);
            
            Instantiate(playerTeamSpawnEffect, spawnPosition.position, Quaternion.identity);

            //set npc's values
            // Set the NPC’s health from PlayerManager data
            Entity entityScript = newEntity.GetComponent<Entity>();
            if (npcData.health == 0) npcData.health = entityScript.GetMaxHP();
            entityScript.SetHP(npcData.health);
            entityScript.SetTeam(team);
            entityScript.playerTeamNPCData = npcData;

            //store instance reference to track NPC for deletion
            npcData.instance = newEntity;

            //remove assigned position from list to prevent reuse
            availablePositions.RemoveAt(randomIndex);
        }
    }

    private void StartNextWave()
    {
        if (currentEnemyIndex >= enemies.Count)
        {
            OnAllEnemiesKilled();
            return;
        }

        remainingEnemies = Mathf.Min(enemiesPerWave, enemies.Count - currentEnemyIndex); // Determine how many enemies to spawn in this wave
        enemiesInCurrentWave = remainingEnemies;

        currentWave++;

        //update UI
        if (waveMenu)
        {
            UpdateWaveNumberUI();
            StartWaveProgressUpdate();
        }

        StartCoroutine(AssignEnemiesToPositions()); //spawn enemies for the wave
    }

    private IEnumerator AssignEnemiesToPositions()
    {
        //Tutorial Stuff
        if (Tutorial.tutorialActivated && !Tutorial.GetFightTutorialFinished()) {
            wasdInfo.transform.position = PlayerMovement.Instance.transform.position + new Vector3(4, 0);
            wasdInfo.SetActive(true);
            yield return new WaitUntil(() => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));
            yield return new WaitForSeconds(2.5f);

            wasdInfo.SetActive(false);
            dodgingInfo.transform.position = PlayerMovement.Instance.transform.position + new Vector3(4, 0);
            dodgingInfo.SetActive(true);

            yield return new WaitUntil(() => Input.GetKey(KeyCode.Space));
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.Space));

            dodgingInfo.SetActive(false);
            shootingInfo.transform.position = PlayerMovement.Instance.transform.position + new Vector3(4, 0);
            shootingInfo.SetActive(true);

            yield return new WaitUntil(() => Input.GetKey(KeyCode.Space));
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.Space));

            shootingInfo.SetActive(false);
            startInfo.transform.position = PlayerMovement.Instance.transform.position + new Vector3(4, 0);
            startInfo.SetActive(true);

            yield return new WaitUntil(() => Input.GetKey(KeyCode.Space));
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.Space));

            startInfo.SetActive(false);

            Tutorial.SetFightTutorialFinished();
        }

        List<Transform> availablePositions = new List<Transform>(enemyPositions); // Copy positions for random selection

        for (int i = 0; i < enemiesInCurrentWave; i++) {
            // Select a random position
            int randomIndex = Random.Range(0, availablePositions.Count);
            Transform spawnPosition = availablePositions[randomIndex];
            GameObject newEntity = Instantiate(enemies[currentEnemyIndex], spawnPosition.position, Quaternion.identity);


            newEntity.GetComponent<Entity>().SetTeam(Team.ENEMY);

            if (difficulty == Difficulty.BOSS) {
                spawnedBoss = newEntity;
                Debug.Log("Spawned Boss Sphinx! HP: " + newEntity.GetComponent<Entity>().GetMaxHP());
            }

            Instantiate(enemySpawnEffect, spawnPosition.position, Quaternion.identity);

            availablePositions.RemoveAt(randomIndex); //prevent reusing this position within same wave

            currentEnemyIndex++; //move to next enemy in list

            //wait for short delay before spawning next enemy for varying animations
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void HandleEnemyKilled(Entity entity)
    {
        // Decrease remaining enemy count
        if (entity.GetTeam() == Team.ENEMY && !entity.CompareTag("NON_WAVE_RELEVANT_ENTITY"))
        {
            remainingEnemies--;
            StartWaveProgressUpdate();  //update wave progress UI

            //if all enemies in current wave defeated, start next wave if enemies remain
            if (remainingEnemies <= 0)
            {
                if (currentEnemyIndex >= enemies.Count)
                {
                    OnAllEnemiesKilled();
                    return;
                }
                Invoke("StartNextWave", timeBetweenWaves); //delay before next wave
            }
        }
    }

    private void OnAllEnemiesKilled()
    {
        AudioManager.instance.PlaySoundFX(AudioManager.instance.audioVictoryFX);
        levelFinished = true;
        StartCoroutine(WaitForWaveEnd());
    }

    private IEnumerator WaitForWaveEnd()
    {
        if (difficulty == Difficulty.BOSS)
        {
            waveProgressImage.fillAmount = 0f;
            yield return new WaitForSeconds(10f);
        }  

        yield return new WaitUntil(() => waveProgressImage.fillAmount == 0.0f); //wait until wave progress reaches zero

        //Claim Challenge Rewards
        ChallengeRewardWindow.Instance.Show();
        yield return new WaitUntil( () => ChallengeRewardWindow.Instance.ClaimedAllRewards() );

        OnWaveEnd();
    }

    private void OnWaveEnd()
    {
        //destroy all shots
        BulletCollection[] bulletCollections = Object.FindObjectsOfType<BulletCollection>();
        foreach (BulletCollection bulletCollection in bulletCollections) Destroy(bulletCollection.gameObject);

        Debug.Log("All enemies defeated. Level complete!");

        MoneyManager.instance.CollectAllCoins();    //collect all coins
        PlayerTeamManager.instance.playerCurrentHP = PlayerMovement.Instance.GetEntity().GetCurrentHP();
        //PlayerTeamManager.instance.playerMaxHp = PlayerMovement.Instance.GetEntity().GetMaxHP();

        if (Tutorial.tutorialActivated) {
            ScreenTransition.Instance.LoadScene("WorldMap");
            return;
        }

        if (difficulty != Difficulty.BOSS)
            GameMenuManager.instance.ShowUpgradeMenu(); //show upgrade menu
        else
            ScreenTransition.Instance.LoadScene("Credits");

    }

    private void UpdateWaveNumberUI()
    {
        waveText.text = "Wave " + currentWave.ToString() + " / " + totalWaves.ToString();
    }

    private void StartWaveProgressUpdate()
    {
        waveProgressImage.gameObject.SetActive(true);
        waveProgressBackgroundImage.gameObject.SetActive(true);

        //stop ongoing interpolation coroutine
        if (waveProgressUpdateCoroutine != null) StopCoroutine(waveProgressUpdateCoroutine);

        //start new interpolation coroutine
        waveProgressUpdateCoroutine = StartCoroutine(UpdateWaveProgress((float)remainingEnemies / (float)enemiesInCurrentWave));
    }

    private IEnumerator UpdateWaveProgress(float targetProgress)
    {
        if (difficulty == Difficulty.BOSS) {
            if (!bossHP_handleFilled) {
                bossHP_handleFilled = true;

                yield return new WaitUntil(() => spawnedBoss != null);
                Entity bossEnt = spawnedBoss.GetComponent<Entity>();

                if (bossEnt != null)
                    waveProgressImage.fillAmount = 1f;

                while (waveProgressImage.fillAmount != 0f) {
                    waveProgressImage.fillAmount = Mathf.Max(bossEnt.GetCurrentHP() / (float)bossEnt.GetMaxHP(), 0);
                    //Debug.Log("Rolling");
                    yield return null;
                }

            }

        } else {
            //get current displayed progress
            float currentProgressDisplayed = waveProgressImage.fillAmount;

            //if current progress matches target, no need to animate
            if (currentProgressDisplayed == targetProgress) yield break;

            float elapsed = 0f;

            //interpolate displayed progress over time
            while (elapsed < textUpdateDuration) {
                elapsed += Time.deltaTime;
                float interpolatedValue = Mathf.Lerp(currentProgressDisplayed, targetProgress, elapsed / textUpdateDuration);
                waveProgressImage.fillAmount = interpolatedValue;
                yield return null;
            }

            //ensure final value is exact target
            waveProgressImage.fillAmount = targetProgress;
        }
    }
}

