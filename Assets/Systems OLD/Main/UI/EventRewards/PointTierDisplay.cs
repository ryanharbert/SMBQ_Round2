using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointTierDisplay : MonoBehaviour {

    public Text tier;
    public Text progress;
    public Text currencyQty;
	public Image currencyImg;

	public GameObject arenaPointsIcon;
	public GameObject raidPointsIcon;

	public void Set(int tierCount, PointTierData data, int points, string type)
    {
        tier.text = "#" + tierCount;
		if(points < data.Req)
		{
			progress.text = points + " / " + data.Req;
		}
		else
		{
			progress.text = "Complete";
		}
		if(type == "pvp")
		{
			arenaPointsIcon.SetActive(true);
			raidPointsIcon.SetActive(false);
		}
		else
		{
			arenaPointsIcon.SetActive(false);
			raidPointsIcon.SetActive(true);
		}
		if(data.Rew == "GO")
		{
			currencyImg.sprite = Resources.Load<Sprite>("CurrencyOffers/GoldPile");
		}
		else
		{
			currencyImg.sprite = Resources.Load<Sprite>("CurrencyOffers/Star");
		}
		currencyQty.text = "x" + data.Amt;
    }
}
