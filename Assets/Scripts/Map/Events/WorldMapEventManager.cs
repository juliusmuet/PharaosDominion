using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WorldMapEventManager : MonoBehaviour
{
    public static WorldMapEventManager Instance { get; private set; }

    [SerializeField] private EventOptionButton[] options;
    [SerializeField] private EnhancedTexts mainText;

    [SerializeField] private Animator anim;
    [SerializeField] private Animator headerAnim;
    [SerializeField] private Animator mainPaperAnim;

    [SerializeField] private float paperMaterialiseTime = 2f;

    private WorldMapEvent currentEvent = null;

    private float animOpenTime, animCloseTime;
    
    public void Awake() {
        Instance = this;

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip c in clips) {
            if (c.name == "EventBoardIn") {
                animOpenTime = c.length;
            } else if (c.name == "EventBoardOut") {
                animCloseTime = c.length;
            }
        }

        this.gameObject.SetActive(false);
    }

    public void Open() {
        MapNode.nodesInteractable = false;

        anim.SetTrigger("open");

        mainText.SetText("");

        currentEvent = WorldMapEvent.GenerateEvent();

        this.gameObject.SetActive(true);

        for (int i = 0; i < options.Length; i++)
            SetOptionActive(options[i], false);


        currentEvent.optionButtons = options;
        currentEvent.mainText = mainText;

        currentEvent.Build();

        mainPaperAnim.Play("EventTextPaperFadeIn");
        headerAnim.Play("EventHeaderFadeIn");

        AudioManager.instance.PlaySoundFX(AudioManager.instance.audioPaperFX);

        StartCoroutine(OpenCoroutine());
    }

    private IEnumerator OpenCoroutine() {
        yield return new WaitForSeconds(animOpenTime);

        for(int i = 0; i < currentEvent.OptionButtonsUsed(); i++) {
            options[i].anim.Play("EventOptionFadeIn");
            SetOptionActive(options[i], true);
        }
        

        yield return new WaitForSeconds(paperMaterialiseTime);

        mainText.StartTyping();

        while(!mainText.FinishedTyping())
            yield return null;

        for(int i = 0; i < currentEvent.OptionButtonsUsed(); i++) 
            if (currentEvent.optionButtons[i].gameObject.activeSelf)
                currentEvent.optionButtons[i].txt.StartTyping();

        /*bool allFinished = true;
        do {
            foreach (EventOptionButton btn in options)
                if (btn.gameObject.activeSelf)
                    allFinished = allFinished && btn.txt.FinishedTyping();
                   
        }while(!allFinished);*/
        
    }

    public void Close() {
        anim.SetTrigger("close");
        MapNode.nodesInteractable = true;
        StartCoroutine(DisableAfterTime(animCloseTime));
    }

    private IEnumerator DisableAfterTime(float time) {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }

    public void Update() {
        if (currentEvent == null)
            return;

        if (!currentEvent.GetFinished())
            currentEvent.Update();
        else
            Close();
    }


    public static void SetOptionActive(EventOptionButton option, bool flag) {
        option.transform.gameObject.SetActive(flag);
    }
}
