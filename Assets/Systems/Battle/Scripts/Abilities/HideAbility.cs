using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HideAbility : AbilityType
{
    bool active = false;

    public override void StartCast(Vector3 abilityPosition)
    {
        base.StartCast(abilityPosition);
        if (active)
        {
            CDTimer = 0;
        }
    }

    public override IEnumerator Casting()
    {
        if(!active)
        {
            active = true;

            //u.state = UnitState.Idle;
            if (PhotonNetwork.IsMasterClient)
            {
                Battle.state.units.Remove(u);
            }
            while (active)
            {
                if (PhotonNetwork.IsMasterClient && u.health <= 0)
                {
                    //Battle.instance.unitSystem.DeathAnimation(u);
                }
                yield return null;
            }
        }
        else
        {
            active = false;

            if (PhotonNetwork.IsMasterClient)
            {
                Battle.state.units.Add(u);
            }

            while (durationTimer < castDuration)
            {
                durationTimer += Time.deltaTime;
                yield return null;
            }

            //u.state = UnitState.Idle;
        }
    }

    public override bool Decision(BattleState s, Unit u, TargetInfo t, int index)
    {
        return false;
    }
}
