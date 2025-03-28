using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] private AudioSource buttonClickSource;
    [SerializeField] private AudioClip buttonClickSound;

    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button b in buttons) b.onClick.AddListener(ButtonSound);
    }
    private void ButtonSound()
    {
        buttonClickSource.PlayOneShot(buttonClickSound, 0.5f);
    }
}
