using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchaseDialogManager : MonoBehaviour
{
    [SerializeField] public GameObject purchaseDialog;
    [SerializeField] public TMP_Text upgradeText;
    [SerializeField] public Button yesButton;
    [SerializeField] public Button noButton;

    [SerializeField] public GameObject errorDialog;
    [SerializeField] public TMP_Text errorText;

    public static PurchaseDialogManager instance;    //singleton

    private void Start()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        else instance = this;

        purchaseDialog.SetActive(false);
        errorDialog.SetActive(false);
    }

    public void ShowPurchaseDialog(string text, Action onYes, Action onNo)
    {
        purchaseDialog.SetActive(true);

        upgradeText.text = text;

        yesButton.onClick.AddListener(() =>
        {
            onYes?.Invoke();
            purchaseDialog.SetActive(false);
            AudioManager.instance.PlaySoundFX(AudioManager.instance.audioPurchaseFX);
            yesButton.onClick.RemoveAllListeners();
        });

        noButton.onClick.AddListener(() =>
        {
            onNo?.Invoke();
            purchaseDialog.SetActive(false);
            yesButton.onClick.RemoveAllListeners();
        });
    }

    public void ShowErrorDialog(string message)
    {
        errorDialog.SetActive(true);
        errorText.text = message;
    }

    public void CloseErrorDialog()
    {
        errorDialog.SetActive(false);
    }
}
