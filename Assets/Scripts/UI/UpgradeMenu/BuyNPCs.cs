using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate ()
            {
                OnClick(param);
            });
    }
}

public class BuyNPCs : MonoBehaviour
{
    [SerializeField] private bool reachedFromWorldMap = false;


    [SerializeField] private Transform scrollContainer;
    [SerializeField] private ShowTeam showTeam;

    private List<GameObject> buttons = new List<GameObject>();

    void OnEnable()
    {
        InitialiseList();
    }

    private void InitialiseList()
    {
        GameObject templateNPCButton = scrollContainer.GetChild(0).gameObject;

        List<GameObject> NPCs;

        if (!reachedFromWorldMap) {
            NPCs = LevelManager.instance.enemies;
        } else {
            NPCs = WorldMapFactory.Instance.GetRandomEnemies(3);
        }
        buttons.Clear();

        int i = 0;
        foreach (GameObject npc in NPCs)
        {
            GameObject element = Instantiate(templateNPCButton, scrollContainer);
            element.transform.GetChild(0).GetComponent<TMP_Text>().text = AddSpacesToCamelCase(npc.name);
            element.transform.GetChild(1).GetComponent<TMP_Text>().text = npc.GetComponent<Entity>().GetCosts().ToString();
            element.transform.GetChild(2).GetComponent<Image>().sprite = npc.GetComponent<SpriteRenderer>().sprite;
            element.GetComponent<Button>().AddEventListener((i, NPCs[i]), OnUpgradeButtonClicked);
            buttons.Add(element);
            i += 1;
        }

        Destroy(templateNPCButton);
    }

    private void OnUpgradeButtonClicked((int buttonIndex, GameObject npc) data)
    {
        //GameObject npc = LevelManager.instance.enemies[i];

        if (PlayerTeamManager.instance.playerTeamNPCs.Count >= PlayerTeamManager.MAX_TEAM_COUNT)
        {
            PurchaseDialogManager.instance.ShowErrorDialog($"You cannot have more than {PlayerTeamManager.MAX_TEAM_COUNT} team members!");
            return;
        }

        int costs = data.npc.GetComponent<Entity>().GetCosts();
        if(costs > MoneyManager.instance.GetMoney())
        {
            PurchaseDialogManager.instance.ShowErrorDialog("You do not have enough money!");
            return;
        }

        PurchaseDialogManager.instance.ShowPurchaseDialog(
            AddSpacesToCamelCase(data.npc.name),
            () => PurchaseUpgrade(data.buttonIndex, data.npc, costs),
            () => Debug.Log("Upgrade canceled")
        );
    }

    private void PurchaseUpgrade(int i, GameObject npc, int costs)
    {
        MoneyManager.instance.SubtractFromMoney(costs);
        PlayerTeamManager.instance.AddNPC(npc);
        Destroy(buttons[i]);
        showTeam.ResetList();
    }

    private string AddSpacesToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        // Use Regex to insert spaces before uppercase letters
        return Regex.Replace(input, "(?<!^)([A-Z])", " $1");
    }
}
