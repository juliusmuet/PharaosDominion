using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChallengeRewardWindow : MonoBehaviour
{
    
    public static ChallengeRewardWindow Instance { get; private set; }

    [SerializeField] private Animator anim;
    [SerializeField] private Image rewIcon;
    [SerializeField] private TextMeshProUGUI rewAmount;

    private LinkedList<ChallengeReward> queuedRewards = new LinkedList<ChallengeReward>();
    private bool rewardClaimed = false;
    private bool claimedAllRewards = false;

    public void Awake() {
        Instance = this;
        this.gameObject.SetActive(false);
    }

    public void AddReward(ChallengeReward rew) {
        queuedRewards.AddLast(rew);
    }

    public void Show() {
        if (queuedRewards.Count > 0) {
            this.gameObject.SetActive(true);
            claimedAllRewards = false;
            StartCoroutine(_Show());
        } else {
            claimedAllRewards = true;
        }
    }

    private IEnumerator _Show() {
        
        

        while(queuedRewards.Count > 0) {
            yield return ShowElement(queuedRewards.First.Value);
            queuedRewards.RemoveFirst();
        }

        this.gameObject.SetActive(false);
        claimedAllRewards = true;
    }

    private IEnumerator ShowElement(ChallengeReward rew) {
        rewardClaimed = false;
        rewIcon.sprite = rew.icon;
        rewAmount.text = "x" + rew.multiplier;

        anim.Play("ChallengeRewardOpen");
        yield return new WaitForSeconds(0.5f);
        rewardClaimed = false;
        yield return new WaitUntil( () => rewardClaimed );
        rew.Consume();
        anim.SetTrigger("close");
        yield return new WaitForSeconds(0.5f);
    }

    public void ClaimPressed() {
        rewardClaimed = true;
    }

    public bool ClaimedAllRewards() {
        return claimedAllRewards;
    }
}
