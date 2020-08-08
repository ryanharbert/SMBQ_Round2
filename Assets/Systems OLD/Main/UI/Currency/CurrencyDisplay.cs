using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyDisplay : MonoBehaviour
{
	public Text diamonds;
	public Text gold;
	public Text energy;
	public Text secondsToRecharge;
    public Text scrolls;
    public Text scrollsSecToRecharge;

    public float goldShopYPos;
    public float gemsShopYPos;
    public float energyShopYPos;
    public float scrollsShopYPos;

    public Toggle shopToggle;
    public GameObject shopObject;
    public RectTransform shopRect;

    void Awake ()
	{
		UpdateTimers();
	}
	
	void FixedUpdate ()
	{
		UpdateTimers();
	}

	void UpdateTimers()
	{
		diamonds.text = Data.instance.currency.gems.ToString();
		gold.text = Data.instance.currency.gold.ToString();
		energy.text = Data.instance.currency.energy + " / " + Data.instance.currency.energyMax;
		if(Data.instance.currency.energy < Data.instance.currency.energyMax)
		{
			secondsToRecharge.text = EnergyTimerText();
		}
		else
		{
			secondsToRecharge.text = "";
        }
        if(scrolls != null)
        {
            scrolls.text = Data.instance.currency.scrolls + " / " + Data.instance.currency.scrollsMax;
            if (Data.instance.currency.scrolls < Data.instance.currency.scrollsMax)
            {
                scrollsSecToRecharge.text = ScrollTimerText();
            }
            else
            {
                scrollsSecToRecharge.text = "";
            }
        }
    }

	string EnergyTimerText ()
	{
		string secondsToRechargeText = "";
		int minutes = Mathf.FloorToInt(Data.instance.currency.secondsToRecharge / 60);
		int seconds = Mathf.CeilToInt(Data.instance.currency.secondsToRecharge % 60);

		if(seconds == 60)
		{
			seconds = 0;
			minutes += 1;
		}

		if (Data.instance.currency.secondsToRecharge > 59)
		{
			secondsToRechargeText = minutes + "m " + seconds + "s";
		}
		else
		{
			secondsToRechargeText = seconds + "s";
		}

		return secondsToRechargeText;
    }

    string ScrollTimerText()
    {
        string secondsToRechargeText = "";
        int minutes = Mathf.FloorToInt(Data.instance.currency.scrollsSecToRecharge / 60);
        int seconds = Mathf.CeilToInt(Data.instance.currency.scrollsSecToRecharge % 60);

        if (seconds == 60)
        {
            seconds = 0;
            minutes += 1;
        }

        if (Data.instance.currency.scrollsSecToRecharge > 59)
        {
            secondsToRechargeText = minutes + "m " + seconds + "s";
        }
        else
        {
            secondsToRechargeText = seconds + "s";
        }

        return secondsToRechargeText;
    }

    void PurchasePopup(float y)
    {
        if(shopToggle != null)
        {
            shopToggle.isOn = true;
        }
        else
        {
            shopObject.SetActive(true);
        }

        if(MainShop.instance.currencies != null)
        {
            MainShop.instance.currencies.toggle.isOn = true;
        }
        shopRect.anchoredPosition = new Vector2(shopRect.anchoredPosition.x, y);
    }

    public void GoldPurchasePopup()
    {
        PurchasePopup(goldShopYPos);
    }

    public void EnergyPurchasePopup()
    {
        PurchasePopup(energyShopYPos);
    }

    public void GemPurchasePopup()
    {
        PurchasePopup(gemsShopYPos);
    }

    public void ScrollsPurchasePopup()
    {
        PurchasePopup(scrollsShopYPos);
    }


}
