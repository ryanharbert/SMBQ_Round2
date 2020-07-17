using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HeroUnit : Unit
{
    public int cooldown;

    protected override void Awake()
    {
        base.Awake();
        for(int i = 0; i < abilities.Length; i++)
        {
            if (abilities[i].inputAreaDisplay != null)
            {
                abilities[i].inputAreaDisplay.enabled = false;
            }
        }
    }

    [PunRPC]
    public void Cast(Vector3 abilityPosition, int index)
    {
        targetInfo = new TargetInfo();
        targetInfo.position = abilityPosition;
        abilities[index].action.Execute(this, targetInfo, index);
    }

    [PunRPC]
    public void PlayerControlled()
    {
        AI = false;
    }
}
