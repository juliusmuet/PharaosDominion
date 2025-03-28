using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Coin : MonoBehaviour
{
    [SerializeField] public int coinValue = 1;

    //event for when coin is collected
    public static UnityEvent<int> OnCoinCollected = new UnityEvent<int>();

    //blinking and disappearance
    [SerializeField] private bool canDisappear = true;
    [SerializeField] private float timeToStartBlinking = 8f;
    [SerializeField] private float blinkInterval = 0.2f;
    [SerializeField] private float timeToDestroy = 13f;
    private SpriteRenderer spriteRenderer;
    private bool isBlinking = false;
    private float elapsedTime = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) CollectCoin();
    }

    public void CollectCoin()
    {
        AudioManager.instance.PlaySoundFX(AudioManager.instance.audioCoinFX);
        OnCoinCollected.Invoke(coinValue);

        Destroy(gameObject);
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (canDisappear) elapsedTime += Time.deltaTime;

        if (elapsedTime >= timeToStartBlinking && !isBlinking)
        {
            StartCoroutine(BlinkObject());
            isBlinking = true;
        }

        if (elapsedTime >= timeToDestroy)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator BlinkObject()
    {
        while (elapsedTime < timeToDestroy)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        spriteRenderer.enabled = true;
    }
}
