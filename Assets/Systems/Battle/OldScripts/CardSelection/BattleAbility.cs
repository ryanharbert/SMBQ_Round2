using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleAbility : BattleCard
{
	public Text cooldownText;
	public RectTransform rect;
	public Vector2 alivePosition;
	public Vector2 notAlivePosition;

	[HideInInspector] public bool onCooldown = false;

	bool heroAlive = true;
	CardData heroData;

	private void Start()
	{
		heroData = Data.instance.collection.inventory[Data.instance.collection.deckData.hero];
		if(index == 5)
		{
			cardDisplay.cardImage.sprite = ((HeroUnit)heroData.Unit).abilities[0].abilitySprite;
		}
		else if(index == 6)
		{
			cardDisplay.cardImage.sprite = ((HeroUnit)heroData.Unit).abilities[1].abilitySprite;
		}
	}

	protected override void Update()
	{
		if (!Battle.instance.setup)
			return;

        HotKey();

        int cooldown = 0;
		if (index == 5)
		{
            if(Battle.state.hero != null)
            {
                cooldown = Mathf.CeilToInt(Battle.state.hero.abilities[0].CDTimer);
            }
		}
		else if (index == 6)
        {
            if (Battle.state.hero != null)
            {
                cooldown = Mathf.CeilToInt(Battle.state.hero.abilities[1].CDTimer);
            }
        }

		if (cooldown > 0)
		{
			cooldownText.text = (cooldown).ToString();
			onCooldown = true;
		}
		else
		{
			cooldownText.text = "";
			onCooldown = false;
		}

		DisplayUpdate();
		AbilitiesActive();
	}

	void DisplayUpdate()
	{
        if (Battle.state == null)
            return;

        if(index == 6 && BattleHero.alive && Battle.state.hero != null && Battle.state.hero.abilities[1].disabled)
        {
            slotImage.color = Color.grey;
        }
        else if(index == 5 && BattleHero.alive && Battle.state.hero != null && Battle.state.hero.abilities[0].disabled)
        {
            slotImage.color = Color.grey;
        }
        else if (!onCooldown && BattleHero.alive && Battle.state.hero != null && Battle.state.hero.state < 4)
		{
            slotImage.color = Color.white;
		}
		else
		{
			slotImage.color = Color.grey;
		}
	}

	void AbilitiesActive()
	{
		if (BattleHero.alive && !heroAlive)
		{
			rect.anchoredPosition = alivePosition;
			anim.SetTrigger("NotSelected");
			heroAlive = true;
		}
		else if(!BattleHero.alive && heroAlive)
		{
			rect.anchoredPosition = notAlivePosition;
			anim.SetTrigger("Disabled");
			heroAlive = false;
		}
	}

	public override void Toggle(bool selected)
	{
		if (!BattleHero.alive && selected)
		{
			return;
		}

		base.Toggle(selected);
	}

	protected override void SetMesh(bool enabled)
	{
        if (index == 5 && Battle.state.hero.abilities[0].validPlayArea == ValidPlayArea.OnClick)
        {
            return;
        }
        else if (index == 6 && Battle.state.hero.abilities[1].validPlayArea == ValidPlayArea.OnClick)
        {
            return;
        }

		if (enabled)
		{
			if (index == 5)
			{
                Battle.state.hero.abilities[0].inputAreaDisplay.enabled = true;
			}
			else if (index == 6)
			{
                Battle.state.hero.abilities[1].inputAreaDisplay.enabled = true;
			}
		}
		else
		{
			if (index == 5)
			{
                Battle.state.hero.abilities[0].inputAreaDisplay.enabled = false;
			}
			else if (index == 6)
			{
                Battle.state.hero.abilities[1].inputAreaDisplay.enabled = false;
			}
		}
	}

    protected override void HoverOverText()
    {
        if(BattleHero.alive)
        {
            if (index == 5)
            {
                Battle.instance.hoverOverText.text = Battle.state.hero.abilities[0].shortDesc;
            }
            else if (index == 6)
            {
                Battle.instance.hoverOverText.text = Battle.state.hero.abilities[1].shortDesc;
            }
        }
        else
        {
            Battle.instance.hoverOverRect.gameObject.SetActive(false);
        }
    }
}
