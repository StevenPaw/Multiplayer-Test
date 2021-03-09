using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController MC;

    public GameObject shopPanel;

    public GameObject[] buttonLocks;
    public Button[] unlockedButtons;

    private void OnEnable()
    {
        MC = this;
    }

    private void Start()
    {
        SetUpStore();
    }

    public void SetUpStore()
    {
        for (int i = 0; i < PersistentData.PD.allSkins.Length; i++)
        {
            buttonLocks[i].SetActive(!PersistentData.PD.allSkins[i]);
            unlockedButtons[i].interactable = PersistentData.PD.allSkins[i];
        }
    }

    public void UnlockSkin(int index)
    {
        PersistentData.PD.allSkins[index] = true;
        PlayFabController.PFC.SetUserData(PersistentData.PD.SkinsDataToString());
        SetUpStore();
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);
    }

    public void SetMySkin(int whichSkin)
    {
        PersistentData.PD.mySkin = whichSkin;
    }
}
