using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapHealthBar : MonoBehaviour
{
    public static WorldMapHealthBar Instance {  get; private set; }

    [SerializeField] private TMPro.TextMeshProUGUI healthText;
    [SerializeField] private UnityEngine.UI.Image healthBar;

    public void Awake() {
        Instance = this;
        //this.gameObject.SetActive(false);
    }

    public void Show() {
        Update();
        this.gameObject.SetActive(true);
    }

    public void Hide() {
        this.gameObject.SetActive(false);
    }

    public void Update() {
        int playerHealth = PlayerTeamManager.instance.playerCurrentHP;
        int playerMaxHealth = PlayerTeamManager.instance.playerMaxHp;

        healthBar.fillAmount = playerHealth / (float)playerMaxHealth;
        healthText.text = $"{playerHealth}/{playerMaxHealth}";
    }


}
