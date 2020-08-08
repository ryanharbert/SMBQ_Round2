using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHero : BattleCard
{
    public BattleAbility ability;
    public TMPro.TextMeshProUGUI cooldownText;
	public RectTransform rect;
	public Vector2 alivePosition;
	public Vector2 notAlivePosition;

    [HideInInspector] public CardData heroData;
    [HideInInspector] public bool alive = false;
    [HideInInspector] public bool onCooldown = false;


	private void Start()
	{
		heroData = Data.instance.collection.inventory[Data.instance.collection.deck.heroes[index]];
		cardData = heroData;
		cardDisplay.SetCardDisplay(heroData.itemID);

        ability.hero = this;
        ability.cardDisplay.SetCardDisplay(heroData.itemID);
        ability.cardDisplay.cardImage.sprite = ((HeroUnit)heroData.Unit).abilities[ability.index].abilitySprite;
        ability.gameObject.SetActive(false);
    }

	protected override void Update()
	{
		if (!Battle.instance.setup)
			return;

        HotKey();

        if (Battle.state.hero[index] != null)
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

		if (Battle.state.heroCD[index] > 0)
        {
            cooldownText.text = (Mathf.CeilToInt(Battle.state.heroCD[index]).ToString());
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
            ability.gameObject.SetActive(true);
        }
		else
		{
			alive = false;
			rect.anchoredPosition = notAlivePosition;
			anim.SetTrigger("NotSelected");
            ability.gameObject.SetActive(false);
        }
	}

	void DisplayUpdate()
    {
        if (Battle.instance.setup == false)
            return;

        if (!onCooldown && !alive && Battle.state.currentMana >= cardData.ManaCost)
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
