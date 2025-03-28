using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Upgrade
{

    public UPGRADE_TYPE type;
    public float basePrice;
    public float priceMultiplier;
    public bool hasLevels;
    public int level;
    public List<float> upgradeSteps;
    public Action<Upgrade, int> OnUpgradePurchased;

    //get current price based on base price, level, and price multiplier
    public int CurrentPrice => Mathf.CeilToInt(basePrice * Mathf.Pow(priceMultiplier, level));

    public void Purchase()
    {
        if (!CheckEnoughMoney()) return;

        PurchaseDialogManager.instance.ShowPurchaseDialog(
            type.GetName(),
            () => {
                MoneyManager.instance.SubtractFromMoney(CurrentPrice);
                Invoke();
                if (ShowUpgrades.instance != null)
                    ShowUpgrades.instance.ResetList();
            },
            () => Debug.Log("Upgrade canceled")
        );
    }

    public void Invoke() {
        if (hasLevels) {
            if (IsMaxed()) return;
            level++;
        }
        OnUpgradePurchased?.Invoke(this, level);
    }

    public bool IsPurchasable()
    {
        if (!hasLevels) return true;
        if (IsMaxed()) return false;
        return true;
    }

    public bool IsMaxed()
    {
        return (level+1) >= upgradeSteps.Count;
    }

    private bool CheckEnoughMoney()
    {
        if (CurrentPrice > MoneyManager.instance.GetMoney())
        {
            PurchaseDialogManager.instance.ShowErrorDialog("You do not have enough money!");
            return false;
        }
        return true;
    }
}
public enum UPGRADE_TYPE {
    UPGRADE_PLAYER_HEAL,
    UPGRADE_TEAM_HEAL,
    UPGRADE_PLAYER_MOVE_SPEED,
    UPGRADE_PLAYER_HP,
    UPGRADE_PLAYER_ATTACK_SPEED,
    UPGRADE_PLAYER_ATTACK_STRENGHT,
    UPGRADE_TEAM_DEFENSE
}

public static class UpgradeTypeExtension {
    public static string GetName(this UPGRADE_TYPE type) {
        switch (type) {
            case UPGRADE_TYPE.UPGRADE_PLAYER_HEAL:
                return "Player Heal";
            case UPGRADE_TYPE.UPGRADE_TEAM_HEAL:
                return "Team Heal";
            case UPGRADE_TYPE.UPGRADE_PLAYER_MOVE_SPEED:
                return "Player Move Speed";
            case UPGRADE_TYPE.UPGRADE_PLAYER_HP:
                return "Player HP";
            case UPGRADE_TYPE.UPGRADE_PLAYER_ATTACK_STRENGHT:
                return "Player Attack Strength";
            case UPGRADE_TYPE.UPGRADE_PLAYER_ATTACK_SPEED:
                return "Player Attack Speed";
            case UPGRADE_TYPE.UPGRADE_TEAM_DEFENSE:
                return "Team Defense";
            default:
                return "Uninitialised name!";
        }
    }
}