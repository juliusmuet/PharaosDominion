using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowUpgrades : MonoBehaviour
{

    [SerializeField] private Transform scrollContainer;
    [SerializeField] private ShowTeam showTeam;

    private List<GameObject> buttons = new List<GameObject>();

    public static ShowUpgrades instance;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    void Start()
    {
        //UpgradeManager.instance.PlayerFullHeal(null, 0);   //player is always healed after level - He is not lol!
        //InitialiseList();
    }

    private void OnEnable()
    {
        ResetList();
    }

    public void InitialiseList()
    {
        GameObject templateButton = scrollContainer.GetChild(0).gameObject;
        templateButton.SetActive(true);

        List<Upgrade> upgrades = UpgradeManager.instance.purchgableUpgrades;

        foreach (Upgrade upgrade in upgrades)
        {
            if (!upgrade.IsPurchasable()) continue; //do not show upgrade if maxed
            GameObject element = Instantiate(templateButton, scrollContainer);
            TMP_Text[] texts = element.GetComponentsInChildren<TMP_Text>();
            texts[0].text = upgrade.type.GetName();
            if (upgrade.hasLevels) texts[1].text = upgrade.upgradeSteps[upgrade.level].ToString() +
                    "x -> " + upgrade.upgradeSteps[upgrade.level + 1].ToString() + "x";
            else texts[1].text = "";
            texts[2].text = upgrade.CurrentPrice.ToString();
            element.GetComponent<Button>().onClick.AddListener(upgrade.Purchase);
            buttons.Add(element);
        }

        templateButton.SetActive(false);
    }

    public void ResetList()
    {
        foreach (GameObject button in buttons)
        {
            Destroy(button);
        }

        buttons.Clear();

        InitialiseList();
    }

}
