using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityProfileDisplay : MonoBehaviour
{
    public Text abilityName;
    public Text cooldown;
    public Text desc;
    public CardDisplay cardDisplay;
	
    public void Set(UnitAbility a)
    {
        abilityName.text = a.displayName;
        cooldown.text = a.cooldown.ToString();
        desc.text = a.longDesc;
        cardDisplay.cardImage.sprite = a.abilitySprite;
    }
}
