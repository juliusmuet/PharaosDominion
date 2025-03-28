using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioDamageFXs;

    public void PlayRandomDamageSound()
    {
        int randomIndex = Random.Range(0, audioDamageFXs.Length);
        AudioClip selectedClip = audioDamageFXs[randomIndex];
        AudioManager.instance.PlaySoundFXEnemies(selectedClip);
    }
}
