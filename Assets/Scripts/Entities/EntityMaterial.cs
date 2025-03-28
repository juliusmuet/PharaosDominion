using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMaterial : MonoBehaviour
{
    [SerializeField] private float damageTime = 0.25f;

    private Material material;

    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    public void startDamageFlash()
    {
        StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        float flashAmount = 0f;
        float elapsedTime = 0f;
        while(elapsedTime < damageTime)
        {
            elapsedTime += Time.deltaTime;
            flashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / damageTime));
            material.SetFloat("_FlashAmount", flashAmount);
            yield return null;
        }
    }
}
