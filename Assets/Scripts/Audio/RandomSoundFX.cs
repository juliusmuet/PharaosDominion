using System.Collections;
using UnityEngine;

public class RandomSoundFX : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundClips;
    [SerializeField] private float minTimeBetweenClips = 0.0f;
    [SerializeField] private float maxTimeBetweenClips = 4.0f;
    [SerializeField] private AudioSource audioSource;

    private Coroutine playRoutine;

    void Start()
    {
        if (soundClips.Length > 0) playRoutine = StartCoroutine(PlayRandomClips());
    }

    private IEnumerator PlayRandomClips()
    {
        while (true)
        {
            AudioClip randomClip = soundClips[Random.Range(0, soundClips.Length)];
            audioSource.PlayOneShot(randomClip);
            float waitTime = Random.Range(minTimeBetweenClips, maxTimeBetweenClips);
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void StartRandomSoundFX()
    {
        if (playRoutine == null) playRoutine = StartCoroutine(PlayRandomClips());
    }

    public void StopRandomSoundFX()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
    }
}
