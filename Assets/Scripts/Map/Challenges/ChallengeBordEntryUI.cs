using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeBordEntryUI : MonoBehaviour
{

    public TMPro.TextMeshProUGUI description;
    public TMPro.TextMeshProUGUI rewMultiplier;
    public Image rewardImage;

    public Animator anim;

    public Rect hitbox;

    private Challenge challenge;
    
    public void Show(Challenge challenge) {
        this.challenge = challenge;
        description.text = challenge.GetDescription();
        rewardImage.sprite = challenge.GetChallengeReward().icon;
        rewMultiplier.text = "x" + challenge.GetChallengeReward().multiplier;
    }

    public void Update() {
        if (hitbox.Contains(CameraController.Instance.GetMouseWorld() - transform.position)) {
            anim.SetBool("hover", true);
            if (Input.GetMouseButtonDown(0)) {

                if (Tutorial.tutorialActivated && Tutorial.GetCurrentStep() == 11) {
                    Tutorial.Instance.IncreaseStep();
                }

                transform.parent = null;
                anim.SetTrigger("select");
                AudioManager.instance.PlaySoundFX(AudioManager.instance.audioButtonFX);
                ChallengeController.Instance.AddChallenge(challenge);
                Destroy(this.gameObject, 1f);
                this.enabled = false;
            }
        } else {
            anim.SetBool("hover", false);
        }
    }

    public void OnDrawGizmosSelected() {
        Gizmos.DrawWireCube(transform.position + new Vector3(hitbox.center.x, hitbox.center.y), new Vector3(hitbox.size.x, hitbox.size.y, 0.1f));
    }

    public Challenge GetChallenge() {
        return challenge;
    }
}
