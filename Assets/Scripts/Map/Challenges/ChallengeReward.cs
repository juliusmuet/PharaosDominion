using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Reward", menuName = "Custome/Challenge Reward")]
public class ChallengeReward : ScriptableObject
{
    public RewardType type;
    public UPGRADE_TYPE upgradeType;
    public Sprite icon;
    public int multiplier = 1;
    public int chanceWeight = 10;

    public void Consume() {
        switch (type) {
            case RewardType.MONEY:
                MoneyManager.instance.AddToMoney(multiplier);
                return;
            case RewardType.UPGRADE:
                UpgradeManager.instance.GetUpgradeByType(upgradeType).Invoke();
                return;
            default:
                Debug.Log("REWARDTYPE UNIMPLEMENTED!");
                return;
        }
    }
}

public enum RewardType {
    MONEY,
    UPGRADE
}
