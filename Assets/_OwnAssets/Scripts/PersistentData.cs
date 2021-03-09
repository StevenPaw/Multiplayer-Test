using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData PD;

    public bool[] allSkins;

    public int mySkin;

    private void OnEnable()
    {
        if (PersistentData.PD == null)
        {
            PersistentData.PD = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void SkinsStringToData(string skinsIn)
    {
        for (int i = 0; i < skinsIn.Length; i++)
        {
            if (int.Parse(skinsIn[i].ToString()) > 0)
            {
                allSkins[i] = true;
            }
            else
            {
                allSkins[i] = false;
            }
        }

        MenuController.MC.SetUpStore();
    }
    
    public string SkinsDataToString()
    {
        string toString = "";
        for(int i = 0; i < allSkins.Length; i++)
        {
            if (allSkins[i] == true)
            {
                toString += "1";
            }
            else
            {
                toString += "0";
            }
        }

        return toString;
    }
}
