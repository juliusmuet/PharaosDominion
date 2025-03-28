using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour {
    [SerializeField] private GameObject playerPrefab;




    public List<Upgrade> purchgableUpgrades;
    public static UpgradeManager instance;    //singleton

    private void Awake() {
        if (instance != null && instance != this) Destroy(gameObject);
        else {
            instance = this;
            DontDestroyOnLoad(gameObject); //keep singleton in new scenes
        }

        InitializeUpgrades();
    }

    public Upgrade GetUpgradeByType(UPGRADE_TYPE type) {

        foreach (Upgrade upgrade in purchgableUpgrades) {
            if (upgrade.type == type)
                return upgrade;
        }
        return null;
    }

    private void InitializeUpgrades() {
        purchgableUpgrades = new List<Upgrade>
        {
            new Upgrade
            {
                type = UPGRADE_TYPE.UPGRADE_PLAYER_HEAL,
                basePrice = 75,
                priceMultiplier = 1.0f,
                hasLevels = false,
                upgradeSteps = null,
                OnUpgradePurchased = PlayerFullHeal
            },new Upgrade
            {
                type = UPGRADE_TYPE.UPGRADE_TEAM_HEAL,
                basePrice = 400,
                priceMultiplier = 1.0f,
                hasLevels = false,
                upgradeSteps = null,
                OnUpgradePurchased = TeamFullHeal
            },
            new Upgrade
            {
                type = UPGRADE_TYPE.UPGRADE_PLAYER_HP,
                basePrice = 100,
                priceMultiplier = 1.5f,
                hasLevels = true,
                upgradeSteps = new List<float> {1.0f, 1.25f, 1.5f, 1.75f, 2.0f, 2.5f, 3.0f },
                OnUpgradePurchased = UpgradePlayerHP
            },
            new Upgrade
            {
                type = UPGRADE_TYPE.UPGRADE_PLAYER_MOVE_SPEED,
                basePrice = 75,
                priceMultiplier = 1.5f,
                hasLevels = true,
                upgradeSteps = new List<float> {1.0f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.75f, 2.0f },
                OnUpgradePurchased = UpgradePlayerMovementSpeed
            },
            new Upgrade
            {
                type = UPGRADE_TYPE.UPGRADE_PLAYER_ATTACK_SPEED,
                basePrice = 100,
                priceMultiplier = 1.75f,
                hasLevels = true,
                upgradeSteps = new List<float> {1.0f, 1.33f, 1.66f, 2.0f, 2.33f, 2.66f, 3.0f},
                OnUpgradePurchased = UpgradePlayerAttackSpeed
            },
            new Upgrade
            {
                type = UPGRADE_TYPE.UPGRADE_PLAYER_ATTACK_STRENGHT,
                basePrice = 125,
                priceMultiplier = 1.75f,
                hasLevels = true,
                upgradeSteps = new List<float> {1.0f, 1.33f, 1.66f, 2.0f, 2.33f, 2.66f, 3.0f},
                OnUpgradePurchased = UpgradePlayerAttackStrength
            },
            new Upgrade
            {
                type = UPGRADE_TYPE.UPGRADE_TEAM_DEFENSE,
                basePrice = 500,
                priceMultiplier = 1.75f,
                hasLevels = true,
                upgradeSteps = new List<float> {1.0f, 1.33f, 1.66f, 2.0f},
                OnUpgradePurchased = UpgradeTeamDefense
            }
        };
    }

    public void PlayerFullHeal(Upgrade upgrade, int level) {
        HealPlayer(1);
        ShowTeam.instance.ResetList();
    }

    public void TeamHalfHeal(Upgrade upgrade, int level) {
        HealTeam(0.5f);
        ShowTeam.instance.ResetList();
    }

    public void TeamFullHeal(Upgrade upgrade, int level) {
        HealTeam(1f);
        ShowTeam.instance.ResetList();
    }

    private void HealPlayer(float percent) {
        PlayerTeamManager.instance.playerCurrentHP = Mathf.Min(Mathf.RoundToInt(PlayerTeamManager.instance.playerMaxHp * percent) + PlayerTeamManager.instance.playerCurrentHP, PlayerTeamManager.instance.playerMaxHp);
        Debug.Log($"PlayerCurrent HP: {PlayerTeamManager.instance.playerCurrentHP} Max: {PlayerTeamManager.instance.playerMaxHp}");
        //GameObject player = GameObject.FindWithTag("Player");
        //if (player != null) {
        //    Entity entity = player.GetComponent<Entity>();
        //    entity.SetHP(Mathf.RoundToInt(entity.GetMaxHP() * percent) + entity.GetCurrentHP());
        //} else {
        //    UpgradeManager.instance.SetPlayerCurrentHP(Mathf.RoundToInt(UpgradeManager.instance.GetPlayerCurrentHP_PlayerPrefs() + UpgradeManager.instance.GetPlayerMaxHP_PlayerPrefs() * percent));
        //}
    }

    private void HealTeam(float percent) {
        HealPlayer(percent);

        List<PlayerTeamNPCData> playerTeamNPCDatas = PlayerTeamManager.instance.playerTeamNPCs;
        foreach (PlayerTeamNPCData playerTeamNPCData in playerTeamNPCDatas) {
            if (!playerTeamNPCData.instance) continue;
            Entity entity = playerTeamNPCData.instance.GetComponent<Entity>();
            int hp = Mathf.RoundToInt(entity.GetMaxHP() * percent) + entity.GetCurrentHP();
            entity.SetHP(hp);
            playerTeamNPCData.health = Mathf.Min(entity.GetCurrentHP());
        }
    }

    public void UpgradePlayerHP(Upgrade upgrade, int level) {
        //PlayerPrefs.SetFloat("PlayerMaxHPMultiplier", upgrade.upgradeSteps[level]);
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        //if (player != null)
        //    player.GetComponent<Entity>().UpgradeMaxHP(upgrade.upgradeSteps[level]);
        PlayerTeamManager.instance.playerMaxHp = (int)(upgrade.upgradeSteps[level] * PlayerTeamManager.instance.playerBaseHp);
        HealPlayer(1f);

        if (ShowTeam.instance != null)
            ShowTeam.instance.ResetList();
    }

    public void UpgradePlayerMovementSpeed(Upgrade upgrade, int level) {
        PlayerPrefs.SetFloat("PlayerMovementSpeedMultiplier", upgrade.upgradeSteps[level]);
    }

    public void UpgradePlayerAttackSpeed(Upgrade upgrade, int level) {
        PlayerPrefs.SetFloat("PlayerAttackSpeedMultiplier", upgrade.upgradeSteps[level]);
    }

    public void UpgradePlayerAttackStrength(Upgrade upgrade, int level) {
        PlayerPrefs.SetFloat("PlayerAttackStrengthMultiplier", upgrade.upgradeSteps[level]);
    }

    public void UpgradeTeamDefense(Upgrade upgrade, int level) {
        PlayerPrefs.SetFloat("TeamDefenseMultiplier", upgrade.upgradeSteps[level]);
    }

    public void ResetAllUpgrades() {
        purchgableUpgrades.Clear();
        InitializeUpgrades();
        PlayerTeamManager.instance.playerMaxHp = playerPrefab.GetComponent<Entity>().GetBaseMaxHp();
        PlayerTeamManager.instance.playerCurrentHP = playerPrefab.GetComponent<Entity>().GetBaseMaxHp();
        PlayerPrefs.SetFloat("PlayerMovementSpeedMultiplier", 1.0f);
        PlayerPrefs.SetFloat("PlayerAttackSpeedMultiplier", 1.0f);
        PlayerPrefs.SetFloat("PlayerAttackStrengthMultiplier", 1.0f);
        PlayerPrefs.SetFloat("TeamDefenseMultiplier", 1.0f);
    }

}