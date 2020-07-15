using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHero : BattleCard
{
	public static bool alive = false;
	public static CardData heroData;

	public Text cooldownText;
	public RectTransform rect;
	public Vector2 alivePosition;
	public Vector2 notAlivePosition;

	[HideInInspector] public bool onCooldown = false;


	private void Start()
	{
		heroData = Data.instance.collection.inventory[Data.instance.collection.deckData.hero];
		cardData = heroData;
		cardDisplay.SetCardDisplay(heroData.itemID);
	}

	protected override void Update()
	{
		if (!Battle.instance.setup)
			return;

        HotKey();

        if (Battle.state.hero != null)
		{
			if (!alive)
			{
				SetHeroAlive(true);
			}
		}
		else
		{
			if (alive)
			{
				SetHeroAlive(false);
			}
		}

		if (Battle.state.heroCD > 0)
		{
			cooldownText.text = (Mathf.CeilToInt(Battle.state.heroCD).ToString());
			onCooldown = true;
		}
		else if (onCooldown)
		{
			cooldownText.text = "";
			onCooldown = false;
		}

		DisplayUpdate();
	}

	void SetHeroAlive(bool heroAlive)
	{
		if (heroAlive)
		{
			alive = true;
			rect.anchoredPosition = alivePosition;
			anim.SetTrigger("Disabled");
		}
		else
		{
			alive = false;
			rect.anchoredPosition = notAlivePosition;
			anim.SetTrigger("NotSelected");
		}
	}

	void DisplayUpdate()
	{
		if (!onCooldown && !alive)
		{
			slotImage.color = Color.white;
		}
		else
		{
			slotImage.color = Color.grey;
		}
	}

	public override void Toggle(bool selected)
	{
		if(alive)
		{
			return;
		}

		base.Toggle(selected);
	}
}
