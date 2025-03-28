using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private GameObject[] coins;

    public int totalMoney = 0;
    public static MoneyManager instance;    //singleton

    private Coroutine moneyTextUpdateCoroutine;
    private float textUpdateDuration = 0.5f;    //duration for interpolation in seconds

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        SetMoney(PlayerPrefs.GetInt("TotalMoney", 0), false);
    }

    public int GetMoney()
    {
        return totalMoney;
    }

    public void SetMoney(int value, bool animate = true)
    {
        if (value >= 0)
        {
            totalMoney = value;
            PlayerPrefs.SetInt("TotalMoney", totalMoney);
            if (animate) StartMoneyTextUpdate();
            else SetMoneyText(totalMoney);
        }
        else Debug.LogWarning("Total money cannot be negative.");
    }

    public void AddToMoney(int value)
    {
        SetMoney(GetMoney() + value);
    }

    public void SubtractFromMoney(int value)
    {
        SetMoney(GetMoney() - value);
    }

    private void SetMoneyText(int value)
    {
        if (moneyText != null) moneyText.text = value.ToString();
    }

    private void StartMoneyTextUpdate()
    {
        //stop ongoing interpolation coroutine
        if (moneyTextUpdateCoroutine != null) StopCoroutine(moneyTextUpdateCoroutine);

        //start new interpolation coroutine
        moneyTextUpdateCoroutine = StartCoroutine(UpdateMoneyText(totalMoney));
    }

    private IEnumerator UpdateMoneyText(int targetMoney)
    {
        //get current displayed money amount
        int currentMoneyDisplayed = int.Parse(moneyText.text);

        //if current money matches target, no need to animate
        if (currentMoneyDisplayed == targetMoney) yield break;

        float elapsed = 0f;

        //interpolate displayed money over time
        while (elapsed < textUpdateDuration)
        {
            elapsed += Time.deltaTime;
            int interpolatedValue = (int) Mathf.Lerp(currentMoneyDisplayed, targetMoney, elapsed / textUpdateDuration);
            SetMoneyText(interpolatedValue);
            yield return null;
        }

        //ensure final value is exact target
        SetMoneyText(targetMoney);
    }

    public GameObject GetRandomCoin()
    {
        int randomIndex = Random.Range(0, coins.Length);
        return coins[randomIndex];
    }

    public void CreateRandomCoinAtPosition(Transform transform)
    {
        GameObject coin = GetRandomCoin();
        Instantiate(coin, transform.position, Quaternion.identity);
    }

    public void CreateCoinAtPosition(GameObject coin, Transform transform)
    {
        Instantiate(coin, transform.position, Quaternion.identity);
    }

    public void ResetMoney()
    {
        SetMoney(0, false);
    }

    public void CollectAllCoins()
    {
        Coin[] coins = Object.FindObjectsOfType<Coin>();
        foreach (Coin coin in coins)
        {
            coin.CollectCoin();
        }
    }

    private void OnEnable()
    {
        Coin.OnCoinCollected.AddListener(AddToMoney);
    }

    private void OnDisable()
    {
        Coin.OnCoinCollected.RemoveListener(AddToMoney);
    }

}
