using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RandomLightFlicker : MonoBehaviour
{
    [SerializeField] public Light2D lightSource;
    [SerializeField] public float minIntensity = 0.75f;
    [SerializeField] public float maxIntensity = 1.5f;
    [SerializeField] public float flickerSpeed = 0.05f;
    [SerializeField] public float changeIntervalMin = 0.05f;
    [SerializeField] public float changeIntervalMax = 0.25f;
    [SerializeField] public bool enableFlicker = true;

    private float targetIntensity;
    private Coroutine flickerCoroutine;

    void Start()
    {
        if (lightSource == null) lightSource = GetComponent<Light2D>();

        if (lightSource != null && enableFlicker) flickerCoroutine = StartCoroutine(FlickerLightSmoothly());
        else Debug.LogWarning("Light source not assigned or flickering disabled!");
    }

    IEnumerator FlickerLightSmoothly()
    {
        while (enableFlicker)
        {
            targetIntensity = Random.Range(minIntensity, maxIntensity);

            float elapsedTime = 0f;
            float currentIntensity = lightSource.intensity;

            while (elapsedTime < flickerSpeed)
            {
                elapsedTime += Time.deltaTime;
                lightSource.intensity = Mathf.Lerp(currentIntensity, targetIntensity, elapsedTime / flickerSpeed);
                yield return null;
            }

            yield return new WaitForSeconds(Random.Range(changeIntervalMin, changeIntervalMax));
        }
    }

    public void StopFlickering()
    {
        enableFlicker = false;
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
        }
    }
}
