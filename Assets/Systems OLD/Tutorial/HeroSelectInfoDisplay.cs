using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectInfoDisplay : MonoBehaviour
{
    GameValues v = new GameValues();

    public GameObject displayObject;

    public Text nameText;
    public Text factionText;
    public Text heroDescText;
	public Text startingUnitText;
	public Text stratDescText;

	public void Open(HeroSelection hero)
    {
        displayObject.SetActive(true);

        nameText.text = hero.displayName;
        nameText.color = GameValues.GetFactionColor(hero.faction);

        factionText.text = hero.faction.ToString();
        factionText.color = GameValues.GetFactionColor(hero.faction);

        heroDescText.text = hero.description;
		startingUnitText.text = hero.startingUnits;
		stratDescText.text = hero.stratDesc;
	}
}
