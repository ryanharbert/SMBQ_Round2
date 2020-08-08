using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLevel : MonoBehaviour
{
    public Slider uIProgressBar;
    public Text level;
    public Text progress;

    private void Awake()
    {
        level.text = Data.instance.currency.playerLevel.ToString();
        int exp = Data.instance.currency.exp;

        if(Data.instance.currency.playerLevel < 40)
        {
            int levelUpCost = Data.instance.values.GetLevelUpCost(Data.instance.currency.playerLevel);
            progress.text = exp + " / " + levelUpCost;
            uIProgressBar.value = (float)exp / levelUpCost;
        }
        else
        {
            progress.text = "Max Level";
            uIProgressBar.value = 1;
        }
    }
}
