using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    public static Factory Instance { get; private set; }

    public LayerMask walls;

    private void Awake() {
        Instance = this;
        InitializeChallengeRewards();
    }

    private void OnDrawGizmosSelected() {
        if (SpacialGrouping.currentGrouping == null || !Application.isPlaying) {
            return;
        } else {
            SpacialGrouping.currentGrouping.DrawGizmos();
        }

        
    }

    [SerializeField] private ChallengeReward[] rewards;
    public ChanceSystem<ChallengeReward> challengeRewards;

    public void InitializeChallengeRewards() {
        challengeRewards = new ChanceSystem<ChallengeReward>();
        foreach (ChallengeReward rew in rewards) {
            challengeRewards.AddItem(rew, rew.chanceWeight);
        }
    }
}
