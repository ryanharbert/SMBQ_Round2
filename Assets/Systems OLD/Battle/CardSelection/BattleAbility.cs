using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleAbility : BattleCard
{
    public TMPro.TextMeshProUGUI cooldownText;
    public RectTransform rect;

    [HideInInspector] public BattleHero hero;
    [HideInInspector] public bool onCooldown = false;

	bool heroAlive = true;

	protected override void Update()
	{
		if (!Battle.instance.setup)
			return;

        HotKey();

        int cooldown = 0;
        if (Battle.state.hero[hero.index] != null)
        {
            cooldown = Mathf.CeilToInt(Battle.state.hero[hero.index].abilities[index].CDTimer);
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

        if(hero.alive && Battle.state.hero[hero.index] != null && Battle.state.hero[hero.index].abilities[index].disabled)
        {
            slotImage.color = Color.grey;
        }
        else if (!onCooldown && hero.alive && Battle.state.hero[hero.index] != null && Battle.state.hero[hero.index].state < 4)
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
		if (hero.alive && !heroAlive)
		{
            //rect.anchoredPosition = alivePosition;
            anim.SetTrigger("NotSelected");
			heroAlive = true;
		}
		else if(!hero.alive && heroAlive)
		{
            //rect.anchoredPosition = notAlivePosition;
            //anim.SetTrigger("Disabled");
            heroAlive = false;
		}
	}

	public override void Toggle(bool selected)
	{
		if (!hero.alive && selected)
		{
			return;
		}

		base.Toggle(selected);
	}

	protected override void SetMesh(bool enabled)
	{
        if (Battle.state.hero[hero.index].abilities[index].validPlayArea == ValidPlayArea.OnClick)
        {
            return;
        }

		if (enabled)
		{
            Battle.state.hero[hero.index].abilities[index].inputAreaDisplay.enabled = true;
		}
		else
		{
            Battle.state.hero[hero.index].abilities[index].inputAreaDisplay.enabled = false;
		}
	}

    protected override void HoverOverText()
    {
        if(hero.alive)
        {
            Battle.instance.hoverOverText.text = Battle.state.hero[hero.index].abilities[index].shortDesc;
        }
        else
        {
            Battle.instance.hoverOverRect.gameObject.SetActive(false);
        }
    }
}
