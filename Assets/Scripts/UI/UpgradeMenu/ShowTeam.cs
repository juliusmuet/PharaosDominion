using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowTeam : MonoBehaviour
{
    [SerializeField] private Sprite playerBaseSprite;
    [SerializeField] private Transform scrollContainer;
    [SerializeField] public TMP_Text heading;

    private List<GameObject> panels = new List<GameObject>();

    public static ShowTeam instance;

    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;
    }

    void OnEnable()
    {
        ResetList();
    }

    private void InitialiseList()
    {
        GameObject templateNPCPanel = scrollContainer.GetChild(0).gameObject;
        templateNPCPanel.SetActive(true);

        int i = 0;
        GameObject element = Instantiate(templateNPCPanel, scrollContainer);
        GameObject player = GameObject.FindWithTag("Player");

        element.transform.GetChild(0).GetComponent<Image>().sprite = playerBaseSprite;

        element.transform.GetChild(1).GetComponent<TMP_Text>().text = "HP: " +
            PlayerTeamManager.instance.playerCurrentHP.ToString() +
            " / " +
            PlayerTeamManager.instance.playerMaxHp.ToString();
        
        panels.Add(element);
        i += 1;

        List<PlayerTeamNPCData> teamMembers = PlayerTeamManager.instance.playerTeamNPCs;
        foreach (PlayerTeamNPCData member in teamMembers)
        {
            element = Instantiate(templateNPCPanel, scrollContainer);
            element.transform.GetChild(0).GetComponent<Image>().sprite = member.prefab.GetComponent<SpriteRenderer>().sprite;
            element.transform.GetChild(1).GetComponent<TMP_Text>().text = "HP: " +
                member.health +
                " / " +
                member.prefab.GetComponent<Entity>().GetMaxHP().ToString();
            panels.Add(element);
            i += 1;
        }

        heading.text = "Team (" + (i - 1).ToString() + "/10)";

        templateNPCPanel.SetActive(false);
    }

    public void ResetList()
    {
        foreach(GameObject panel in panels)
        {
            Destroy(panel);
        }

        panels.Clear();

        InitialiseList();
    }
}
