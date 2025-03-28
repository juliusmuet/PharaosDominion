using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GlobalLightController : MonoBehaviour
{
    private Light2D globalLight;
    public static GlobalLightController instance;

    void Awake()
    {
        //singelton pattern
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        globalLight = GetComponent<Light2D>();
    }

    public void SetGlobalLightIntensity(float intensity)
    {
        globalLight.intensity = intensity;
    }
}
