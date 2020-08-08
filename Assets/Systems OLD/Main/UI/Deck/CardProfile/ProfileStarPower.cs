using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileStarPower : MonoBehaviour
{
    public Text starAmount;
    public StarUpgradeDisplay[] starPowers;

    private void OnEnable()
    {
        starAmount.text = Data.instance.currency.stars.ToString();

        for(int i = 1; i < 4; i++)
        {
            starPowers[i - 1].Set(CardProfile.instance.card, i);
        }
    }
}
