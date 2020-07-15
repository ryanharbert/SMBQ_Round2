using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TeleportAbility : AbilityType
{
	public GameObject particleEffect;

    //AI
    public bool AIOffensive;

	public override void Effect()
	{
		if(!triggered)
		{
			DisplayAbility(transform.position);

			transform.position = position;
            u.nav.Warp(position);
            if(!Photon.Pun.PhotonNetwork.IsMasterClient)
            {
                u.masterPosition = position;
            }

			DisplayAbility(transform.position);

			triggered = true;
        }
	}

	void DisplayAbility(Vector3 position)
	{
		if (particleEffect != null)
		{
			GameObject original = Instantiate(particleEffect, position, Quaternion.identity);
			Destroy(original, 4f);
		}
    }

    public override bool Decision(BattleState s, Unit u, TargetInfo t, int index)
    {
        if (CDTimer <= 0)
        {
            if(AIOffensive)
            {
                TargetInfo abilityTarget = new TargetInfo();
                if (AITargeting == u.targeting || AITriggerDistance == -1)
                {
                    abilityTarget = t;
                }
                else
                {
                    //abilityTarget = StratType.ClosestEnemy(s, u, abilityTarget, AITargeting);
                }

                if (t.targetUnit != null && (AITriggerDistance == -1 || (AITriggerDistance + u.radius) > abilityTarget.distance))
                {
                    return AfterDistanceDecision(s, u, abilityTarget, index);
                }
            }
            else
            {
                if ((u.maxHealth * 0.6f) > u.health)
                {
                    TargetInfo abilityTarget = new TargetInfo();
                    abilityTarget = ClosestAlly(s, abilityTarget);

                    if ((AITriggerDistance == -1 || (AITriggerDistance + u.radius) > abilityTarget.distance))
                    {
                        return AfterDistanceDecision(s, u, abilityTarget, index);
                    }
                }
            }
        }
        return false;
    }

    TargetInfo ClosestAlly(BattleState s, TargetInfo t)
    {
        for (int i = 0; i < s.units.Count; i++)
        {
            if (s.units[i] == null)
            {
                continue;
            }

            if (s.units[i].teamID == u.teamID && u != s.units[i])
            {
                float possibleDistance = Vector3.Distance(u.transform.position, s.units[i].transform.position) - s.units[i].radius;
                if (t.distance == 0 || possibleDistance < t.distance)
                {
                    t.distance = possibleDistance;
                    t.targetUnit = s.units[i];
                }
            }
        }
        return t;
    }

    protected override bool AfterDistanceDecision(BattleState s, Unit u, TargetInfo t, int index)
    {
        ((HeroUnit)u).photonView.RPC("Cast", RpcTarget.All, t.targetUnit.transform.position, index);
        return true;
    }
}
