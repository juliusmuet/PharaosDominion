using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private float fadeDelay = 0.25f;

    private void Start()
    {
        SetTextAlpha(1);
        StartCoroutine(FadeText());
    }

    private void SetTextAlpha(float alpha)
    {
        Color color = textMeshPro.color;
        color.a = alpha;
        textMeshPro.color = color;
    }

    private IEnumerator FadeText()
    {
        while (true)
        {
            yield return StartCoroutine(FadeTo(0, fadeInDuration));
            yield return new WaitForSeconds(fadeDelay);
            yield return StartCoroutine(FadeTo(1, fadeOutDuration));
            yield return new WaitForSeconds(fadeDelay);
        }
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = textMeshPro.color.a;
        float time = 0;

        while (time < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            SetTextAlpha(alpha);
            time += Time.deltaTime;
            yield return null;
        }

        SetTextAlpha(targetAlpha);
    }
}
