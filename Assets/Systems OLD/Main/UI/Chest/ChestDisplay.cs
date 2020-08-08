using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestDisplay : MonoBehaviour
{
    public CardDisplay[] cards;
    public CardDisplay jackpotCard;

	public Text displayName;
    public Text cardAmount;
    public Text goldAmount;
    public Text starAmountText;

    public void Set(string chestName)
    {
        ChestData chestData = Data.instance.chests.dict[chestName];

        for (int i = 0; i < chestData.pool.Count; i++)
        {
            cards[i].SetCardDisplay(chestData.pool[i]);
        }
        jackpotCard.SetCardDisplay(chestData.jackpotPool[0]);
        cardAmount.text = "Cards x" + chestData.amount;
        goldAmount.text = "x" + chestData.gold;
		if(displayName != null)
		{
			displayName.text = chestData.displayName;
        }

        if (chestData.starChance > 99)
        {
            if (chestData.starMin == chestData.starMax)
            {
                starAmountText.text = chestData.starMin + " in chest";
            }
            else
            {
                starAmountText.text = chestData.starMin + " to " + chestData.starMax + System.Environment.NewLine + "in chest";
            }
        }
        else
        {
            starAmountText.text = "1 in " + Mathf.RoundToInt(100 / chestData.starChance) + System.Environment.NewLine + "chance";
        }
    }
}
