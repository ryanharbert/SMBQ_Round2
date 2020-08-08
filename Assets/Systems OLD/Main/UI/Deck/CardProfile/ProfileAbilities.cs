using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileAbilities : MonoBehaviour
{
    public AbilityProfileDisplay[] abilities;

    private void OnEnable()
    {
        for(int i = 0; i < abilities.Length; i++)
        {
            abilities[i].Set(CardProfile.instance.card.Unit.abilities[i]);
        }
    }
}
