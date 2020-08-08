using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileInfo : MonoBehaviour
{
	public Text typeText;
	public Text factionText;

	public CardProfileStat[] stats;

	private void OnEnable()
	{
		CardData card = CardProfile.instance.card;
		switch (card.faction)
		{
			case Faction.Growth:
				factionText.text = "GROWTH";
				factionText.color = Data.instance.values.growthColor;
				break;
			case Faction.Ruin:
				factionText.text = "RUIN";
				factionText.color = Data.instance.values.ruinColor;
				break;
			case Faction.Power:
				factionText.text = "POWER";
				factionText.color = Data.instance.values.powerColor;
				break;
			default:
				Debug.Log("Card Type is not supported");
				break;
		}

		switch (card.type)
		{
			case CardType.Melee:
				typeText.text = "MELEE";
				break;
			case CardType.Flying:
				typeText.text = "FLYING";
				break;
			case CardType.Ranged:
				typeText.text = "RANGED";
				break;
			case CardType.Hero:
				typeText.text = "HERO";
				break;
			case CardType.Stronghold:
				typeText.text = "STRONGHOLD";
				break;
			default:
				Debug.Log("Card Type is not supported");
				break;
		}

		for (int i = 0; i < stats.Length; i++)
		{
			if (i == 0)
			{
				stats[i].statLabelTexts.text = "Damage";
				stats[i].statValueTexts.text = card.AttackDamage.ToString();
				stats[i].statValueTexts.color = CardProfile.instance.textColor;
				if (card.Upgradeable)
				{
					stats[i].statGainTexts.text = "+" + Mathf.RoundToInt(card.AttackDamage * 1.1f - card.AttackDamage);
				}
				else
				{
					stats[i].statGainTexts.text = "";
				}
			}
			else if (i == 1)
			{
				stats[i].statLabelTexts.text = "Health";
				stats[i].statValueTexts.text = card.Health.ToString();
				stats[i].statValueTexts.color = CardProfile.instance.textColor;
				if (card.Upgradeable)
				{
					stats[i].statGainTexts.text = "+" + Mathf.RoundToInt(card.Health * 1.1f - card.Health);
				}
				else
				{
					stats[i].statGainTexts.text = "";
				}

			}
			else if (i == 2)
			{
				stats[i].statLabelTexts.text = "DPS";
				stats[i].statValueTexts.text = Mathf.RoundToInt(card.AttackDamage / card.Unit.attackLength).ToString();
				stats[i].statGainTexts.text = "";
			}
			else if (i == 3)
			{
				stats[i].statLabelTexts.text = "Atk Spd";
				stats[i].statValueTexts.text = Math.Round(card.Unit.attackLength, 1).ToString();
				stats[i].statGainTexts.text = "";
			}
			else if (i == 4)
			{
				stats[i].statLabelTexts.text = "Target";
				if (card.Unit.targeting == Targeting.All)
				{
					stats[i].statValueTexts.text = "Air & Ground";
				}
				else if (card.Unit.targeting == Targeting.Ground)
				{
					stats[i].statValueTexts.text = "Ground";
				}
				else if (card.Unit.targeting == Targeting.ObjectiveOnly)
				{
					stats[i].statValueTexts.text = "Buildings";
				}
				stats[i].statGainTexts.text = "";
			}
			else if (i == 5)
			{
				stats[i].statLabelTexts.text = "Speed";
				if (card.Unit.speed == UnitSpeed.Woah)
				{
					stats[i].statValueTexts.text = "Woah!";
				}
				else if (card.Unit.speed == UnitSpeed.Fast)
				{
					stats[i].statValueTexts.text = "Fast";
				}
				else if (card.Unit.speed == UnitSpeed.Normal)
				{
					stats[i].statValueTexts.text = "Normal";
				}
				else if (card.Unit.speed == UnitSpeed.Slow)
				{
					stats[i].statValueTexts.text = "Slow";
				}
				else if (card.Unit.speed == UnitSpeed.Immobile)
				{
					stats[i].gameObject.SetActive(false);
				}
				stats[i].statGainTexts.text = "";
			}
			else if (i == 6)
			{
				stats[i].statLabelTexts.text = "Range";
				if (card.Unit.attackRange < 2)
				{
					stats[i].statValueTexts.text = "Melee";
				}
				else
				{
					stats[i].statValueTexts.text = card.Unit.attackRange.ToString();
				}
				stats[i].statGainTexts.text = "";
			}
			else if (i == 7)
			{
				if (card.Units.Length > 1)
				{
					stats[i].statLabelTexts.text = "Count";
					stats[i].statValueTexts.text = card.Units.Length.ToString();
				}
				else
				{
					stats[i].gameObject.SetActive(false);
				}
				stats[i].statGainTexts.text = "";
			}
			//else if(editorCard.cardData.stats.Length > (i - 2))
			//{
			//	stats[i].statLabelTexts.text = editorCard.cardData.stats[i - 2].statName;
			//	stats[i].statValueTexts.text = editorCard.cardData.stats[i - 2].value;
			//	stats[i].statGainTexts.text = "";
			//}
			//else
			//{
			//	stats[i].gameObject.SetActive(false);
			//}

			if (CardProfile.instance.editorCard.placement == DeckEditorPlacement.Unowned)
			{
				foreach (CardProfileStat s in stats)
				{
					s.statGainTexts.text = "";
				}
			}
		}
	}
}
