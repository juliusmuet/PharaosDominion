using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnter : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer doorRenderer;
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;
    [SerializeField] private AudioClip audioDoorFX;


    void Start()
    {
        PlayerMovement.Instance.InitialisePositon(startTransform, false);
        StartCoroutine(WaitCoroutine(0.5f, () => OpenDoor()));
    }

    private IEnumerator WaitCoroutine(float duration, System.Action onWaitComplete)
    {
        yield return new WaitForSeconds(duration);
        onWaitComplete?.Invoke();
    }

    private void PlayerMoveIntoLevel()
    {
        StartCoroutine(PlayerMovement.Instance.EnterLevel(startTransform, endTransform, 2f, () => StartLevel()));
    }

    private void StartLevel()
    {
        CloseDoor();

        LevelManager.instance.StartLevel();
    }

    private void OpenDoor()
    {
        animator.SetBool("Open", true);
        doorRenderer.sortingLayerName = "Entities";
        StartCoroutine(WaitCoroutine(1f, () => PlayerMoveIntoLevel()));
        AudioManager.instance.PlaySoundFX(audioDoorFX);
    }

    private void CloseDoor()
    {
        animator.SetBool("Open", false);
        doorRenderer.sortingLayerName = "Walls";
        AudioManager.instance.PlaySoundFX(audioDoorFX);
    }

}
