using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeBordUI : MonoBehaviour
{
    public static ChallengeBordUI Instance { get; private set; }
    
    [SerializeField] private ChallengeBordEntryUI[] entries;
    [SerializeField] private Animator anim;

    private void Awake() {
        Instance = this;
    }


    public void Open() {
        this.gameObject.SetActive(true);

        for (int i = 0; i < entries.Length; i++) {
            Challenge challenge = Challenge.GetRandomChallenge();
            bool failed = false;
            for (int i2 = 0; i2 < i; i2++) {
                if (entries[i2].GetChallenge().GetType() == challenge.GetType()) {
                    i--;
                    failed = true;
                    break;
                }
            }
            if (!failed)
                entries[i].Show(challenge);
        }
    }

    public void Close() {
        anim.SetTrigger("close");
        MapNode.nodesInteractable = true;
        Destroy(this.gameObject, 1f);
    }
}
