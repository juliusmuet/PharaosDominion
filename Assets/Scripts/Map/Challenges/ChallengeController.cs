using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeController : MonoBehaviour
{
    public static ChallengeController Instance { get; private set; }

    private LinkedList<Challenge> activeChallenges = new LinkedList<Challenge>();

    [HideInInspector] public float bulletSpeedMultiplicator = 1f;
    [HideInInspector] public int darkness = 0;
    [HideInInspector] public int stryBullets = 0;
    [HideInInspector] public int trapAmount = 0;
    [HideInInspector] public float enemyHealthModificator = 1f;
    [HideInInspector] public float timeSpeedUpModificator = 1f;

    private void Awake() {
        
        if (Instance != null) {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);


    }
      
    public void AddChallenge(Challenge challenge) {
        activeChallenges.AddLast(challenge);

        challenge.Apply(this);
    }

    public void OnLevelEnd() {
        foreach (Challenge challenge in activeChallenges) {
            challenge.Remove(this);
        }
        activeChallenges.Clear();
    }

    public void ResetChallanges() {
        OnLevelEnd();
        
    }

    public void MoveChallengesToRewards() {
        foreach(Challenge c in activeChallenges) {
            ChallengeRewardWindow.Instance.AddReward(c.GetChallengeReward());
        }
    }

}
