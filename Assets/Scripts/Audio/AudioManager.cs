using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioSource audioSourceSoundFXEnemies;

    [SerializeField] public AudioSource audioSourceSoundFX;
    [SerializeField] public RandomSoundFX randomSoundFX;

    [SerializeField] public AudioSource audioSourceBGM;
    [SerializeField] public AudioClip audioLevelBGM;
    [SerializeField] public AudioClip audioUpgradeMenuBGM;

    [SerializeField] public AudioClip audioVictoryFX;
    [SerializeField] public AudioClip audioDeathFX;
    [SerializeField] public AudioClip audioPurchaseFX;
    [SerializeField] public AudioClip audioCoinFX;
    [SerializeField] public AudioClip audioButtonFX;
    [SerializeField] public AudioClip audioPaperFX;

    [HideInInspector] public static AudioManager instance;

    void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    void Start()
    {
        audioSourceBGM.volume = PlayerPrefs.GetFloat("VolumeBGM", 1f);
        audioSourceSoundFX.volume = PlayerPrefs.GetFloat("VolumeFX", 1f);
        audioSourceSoundFXEnemies.volume = PlayerPrefs.GetFloat("VolumeEnemies", 1f);

        if(!LevelManager.instance) PlayBGM(audioLevelBGM);
    }

    public void PlayBGM(AudioClip clip)
    {
        audioSourceBGM.clip = clip;
        audioSourceBGM.Play();
    }

    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }

    public void PlaySoundFX(AudioClip clip)
    {
        audioSourceSoundFX.PlayOneShot(clip);
    }

    public void PlaySoundFXEnemies(AudioClip clip)
    {
        audioSourceSoundFXEnemies.PlayOneShot(clip);
    }
}
