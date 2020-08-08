using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class StarUpgradeDisplay : MonoBehaviour
{    
    public GameObject unlocked;
    public Text levelLocked;
    public GameObject readyToUnlock;
    public Text levelReq;
    public Text costToUnlock;
    public Text description;

    public GameObject comingSoon;

    int index;
    int cost;
    int level;

    public void Set(CardData c, int i)
    {
        if (c.starPowers.Length <= i)
        {
            unlocked.SetActive(false);
            readyToUnlock.SetActive(false);
            levelLocked.gameObject.SetActive(false);
            description.gameObject.SetActive(false);
            comingSoon.SetActive(true);
        }
        else
        {
            comingSoon.SetActive(false);
            description.gameObject.SetActive(true);
            index = i;

            description.text = c.starPowers[i].description;

            if (i == 1)
            {
                cost = 5;
                level = 5;
            }
            else if (i == 2)
            {
                cost = 10;
                level = 15;
            }
            else
            {
                cost = 20;
                level = 30;
            }

            if (c.type == CardType.Hero || c.type == CardType.Stronghold)
            {
                cost = cost * 2;
            }


            if (c.level < level)
            {
                unlocked.SetActive(false);
                readyToUnlock.SetActive(false);
                levelLocked.gameObject.SetActive(true);
                levelLocked.text = "Level " + level + " Req";
            }
            else if ((i - 1) > c.starLevel)
            {
                unlocked.SetActive(false);
                readyToUnlock.SetActive(false);
                levelLocked.gameObject.SetActive(true);
                levelLocked.text = "Unlock Previous";
            }
            else if (i <= c.starLevel)
            {
                unlocked.SetActive(true);
                readyToUnlock.SetActive(false);
                levelLocked.gameObject.SetActive(false);
            }
            else
            {
                unlocked.SetActive(false);
                readyToUnlock.SetActive(true);
                levelLocked.gameObject.SetActive(false);
                levelReq.text = "Level " + level;
                costToUnlock.text = cost.ToString();
            }
        }
    }


    public void Unlock()
    {
        if(Data.instance.currency.stars >= cost)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "increaseStarPower", FunctionParameter = new { card = CardProfile.instance.card.itemID } }, StarPowerUnlocked, Data.instance.GetDataFailure);
            Data.instance.currency.stars -= cost;
            CardProfile.instance.card.starLevel += 1;
            Set(CardProfile.instance.card, index);
            CardProfile.instance.ResetCardProfile();
        }
        else
        {
            Warning.instance.Activate("Need more stars to unlock");
        }
    }

    private void StarPowerUnlocked(ExecuteCloudScriptResult result)
    {
    }
}
