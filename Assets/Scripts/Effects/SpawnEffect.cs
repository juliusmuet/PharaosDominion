using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AudioClip audioSpawnFX;

    void Start()
    {
        AudioManager.instance.PlaySoundFX(audioSpawnFX);
    }

    void Update()
    {
        if (IsAnimationFinished()) Destroy(gameObject);
    }

    private bool IsAnimationFinished()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        return stateInfo.normalizedTime >= 1 && !animator.IsInTransition(0);
    }
}
