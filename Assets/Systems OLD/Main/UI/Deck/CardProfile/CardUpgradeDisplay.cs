using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardUpgradeDisplay : MonoBehaviour
{
	public GameObject upgradeDisplayObject;
	public Text cardName;
	public CardDisplay cardDisplay;
	public CardProfileStat[] stats;
	public Animator levelAnim;
	public UIButton clickToContinue;

	bool animating = false;
	
	int amountNeeded;
	float amountNeededCountdown;
	bool animatingLevelIncrease = false;
	float[] increasingStat = new float[2];
    int[] oldStatValue = new int[2];
	int[] newStatValue = new int[2];
	int statChange;


	public void Setup(CardData cardData)
	{
		upgradeDisplayObject.SetActive(true);
		animating = true;
		clickToContinue.buttonFunction = ClickToContinue;

		cardName.text = cardData.displayName;
		amountNeeded = cardData.AmountNeeded;
		amountNeededCountdown = cardData.AmountNeeded;
		cardDisplay.SetCardDisplay(cardData);
		cardDisplay.uIProgressBar.value = 1.0f;
		cardDisplay.ownedAmountText.text = amountNeeded + " / " + cardData.AmountNeeded;

		foreach(CardProfileStat stat in stats)
		{
			stat.displayObject.SetActive(false);
		}

		//STAT INCREASE SETUP
		oldStatValue[0] = cardDisplay.cardData.AttackDamage;
        increasingStat[0] = oldStatValue[0];
        newStatValue[0] = Mathf.RoundToInt(cardDisplay.cardData.AttackDamage * 1.1f);

        oldStatValue[1] = cardDisplay.cardData.Health;
        increasingStat[1] = oldStatValue[1];
		newStatValue[1] = Mathf.RoundToInt(cardDisplay.cardData.Health * 1.1f);

		StartCoroutine(UpgradeDisplay());
	}

	IEnumerator UpgradeDisplay()
	{
		yield return StartCoroutine (UsingNeededCards());
		yield return StartCoroutine (IncreaseLevel());
		yield return StartCoroutine (StatIncreases());
	}

	IEnumerator UsingNeededCards()
	{
		while(CardProfile.serverUpgraded == false)
		{
			yield return null;
		}
	
		while (amountNeededCountdown != 0)
		{
			if(amountNeededCountdown > 0)
			{
				float rate = amountNeeded / 1.5f;
				amountNeededCountdown = Mathf.MoveTowards (amountNeededCountdown, -0.2f, Time.deltaTime * rate);
			}
			else
			{
				amountNeededCountdown = 0;
			}
			cardDisplay.uIProgressBar.value = amountNeededCountdown/amountNeeded;
			cardDisplay.ownedAmountText.text = Mathf.Round(amountNeededCountdown) + " / " + amountNeeded;

			yield return null;
		}
	}

	IEnumerator IncreaseLevel()
	{
		levelAnim.SetBool("LevelUp", true);
		animatingLevelIncrease = true;
		while(animatingLevelIncrease == true)
		{
			yield return null;
		}
		levelAnim.SetBool("LevelUp", false);
	}

	public void ChangeLevel()
	{
		cardDisplay.SetCardDisplay(cardDisplay.cardData);
	}

	public void LevelIncreaseAnimationOver()
	{
		animatingLevelIncrease = false;
	}

	IEnumerator StatIncreases()
	{
		stats[0].displayObject.SetActive(true);
		stats[0].statLabelTexts.text = "Damage";
		stats[0].statValueTexts.text = Mathf.Round(increasingStat[0]).ToString();
		statChange = Mathf.RoundToInt(newStatValue[0] - increasingStat[0]);
		stats[0].statGainTexts.text = "+" + statChange;
		while(increasingStat[0] < newStatValue[0])
		{
			float rate = statChange / 0.8f;
			increasingStat[0] = Mathf.MoveTowards(increasingStat[0], newStatValue[0], Time.deltaTime * rate);
			stats[0].statValueTexts.text = Mathf.Round(increasingStat[0]).ToString();
			yield return null;
		}
		
		stats[1].displayObject.SetActive(true);
		stats[1].statLabelTexts.text = "Health";
		stats[1].statValueTexts.text = Mathf.Round(increasingStat[1]).ToString();
		statChange = Mathf.RoundToInt(newStatValue[1] - increasingStat[1]);
		stats[1].statGainTexts.text = "+" + statChange;
		while(increasingStat[1] < newStatValue[1])
		{
			float rate = statChange / 0.8f;
			increasingStat[1] = Mathf.MoveTowards(increasingStat[1], newStatValue[1], Time.deltaTime * rate);
			stats[1].statValueTexts.text = Mathf.Round(increasingStat[1]).ToString();
			yield return null;
		}

		animating = false;
	}

	public void ClickToContinue()
	{
		if(CardProfile.serverUpgraded == false)
			return;

		if(animating == true)
		{
			StopAllCoroutines();
		
			cardDisplay.SetCardDisplay(cardDisplay.cardData);

			stats[0].displayObject.SetActive(true);
			stats[0].statLabelTexts.text = "Damage";
			stats[0].statValueTexts.text = Mathf.Round(newStatValue[0]).ToString();
			statChange = Mathf.RoundToInt(newStatValue[0] - oldStatValue[0]);
			stats[0].statGainTexts.text = "+" + statChange;

			stats[1].displayObject.SetActive(true);
			stats[1].statLabelTexts.text = "Health";
			stats[1].statValueTexts.text = Mathf.Round(newStatValue[1]).ToString();
			statChange = Mathf.RoundToInt(newStatValue[1] - oldStatValue[1]);
			stats[1].statGainTexts.text = "+" + statChange;

			animating = false;

			return;
		}

		upgradeDisplayObject.SetActive(false);
	}
}
