using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UnitSystem : BaseSystem
{
    public override void Run(BattleState s)
	{
        if ((Battle.instance.battleType == BattleType.LivePvP || Battle.instance.battleType == BattleType.LiveRaid) && s.gameOver)
            return;
        
		for(int i = 0; i < s.enemyObjectives.Count; i++)
		{
			if(s.enemyObjectives[i] == null)
            {
                s.enemyObjectives.RemoveAt(i);
			}
		}

		for (int i = 0; i < s.playerObjectives.Count; i++)
		{
			if (s.playerObjectives[i] == null)
            {
                s.playerObjectives.RemoveAt(i);
			}
		}

        if (!PhotonNetwork.IsMasterClient)
            return;

        for (int i = 0; i < s.units.Count; i++)
		{
			if (s.units[i] == null)
            {
                s.units.RemoveAt(i);
                i--;
                continue;
            }

			if (s.units[i].health <= 0)
            {
                if (!s.units[i].flying && s.units[i].type != UnitType.Building)
                {
                    s.units[i].photonView.RPC("UnitDied", RpcTarget.All, true);
                    s.units.RemoveAt(i);
                    i--;
                }
                else
                {
                    s.units[i].photonView.RPC("UnitDied", RpcTarget.All, false);
                }
                continue;
            }

            for (int h = 0; h < s.units[i].buffs.Count; h++)
            {
                s.units[i].buffs[h].Run(s, s.units[i]);
            }

            if (s.units[i].state > 2)
            {
                if (s.units[i].state == 3)
                {
                    s.units[i].attack.Channel(s, s.units[i]);
                }
                else
                {
                    s.units[i].abilities[s.units[i].state - 4].action.Channel(s, s.units[i]);
                }
            }
            else
            {
                s.units[i].targetInfo = ClosestEnemy(s, s.units[i], s.units[i].targeting);

                if (!AbilityDecisions(s, s.units[i]))
                {
                    if (!AttackDecision(s, s.units[i]))
                    {
                        s.units[i].move.Execute(s, s.units[i]);
                    }
                    else
                    {
                        s.units[i].move.Exit(s, s.units[i]);
                    }
                }
                else
                {
                    s.units[i].move.Exit(s, s.units[i]);
                }
            }
		}
	}

    protected bool AbilityDecisions(BattleState s, Unit u)
    {
        if (u.AI)
        {
            for (int i = 0; i < u.abilities.Length; i++)
            {
                if (u.abilities[i].Use(s, u, i))
                {
                    return true;
                }
            }
        }
        return false;
    }

    protected bool AttackDecision(BattleState s, Unit u)
    {
        if (u.attack != null && u.targetInfo.targetUnit != null && u.targetInfo.distance <= (u.attackRange + u.radius))
        {
            if (!CurrentEnemyInRange(s, u, u.enemy))
            {
                u.enemy = u.targetInfo.targetUnit;
            }
            else
            {
                u.targetInfo.targetUnit = u.enemy;
				u.targetInfo.position = u.enemy.transform.position;
            }
            u.attack.Execute(u, u.targetInfo, -1);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static TargetInfo ClosestEnemy(BattleState s, Unit u, Targeting targeting)
    {
        TargetInfo t = new TargetInfo();
        for (int i = 0; i < s.units.Count; i++)
        {
            if (s.units[i] == null)
            {
                continue;
            }

            if (s.units[i].teamID != u.teamID && TargetValid(s, u, s.units[i], targeting))
            {
                float possibleDistance = Vector3.Distance(u.transform.position, s.units[i].transform.position) - s.units[i].radius;
                if (t.distance == 0 || possibleDistance < t.distance)
                {
                    t.distance = possibleDistance;
                    t.targetUnit = s.units[i];
                    t.position = t.targetUnit.transform.position;
                }
            }
        }
        return t;
    }

    public static bool TargetValid(BattleState s, Unit u, Unit possibleEnemy, Targeting targeting)
    {
        var validTarget = false;

        switch (targeting)
        {
            case Targeting.All:
                validTarget = true;
                break;

            case Targeting.Ground:
                if (!possibleEnemy.flying)
                {
                    validTarget = true;
                }
                break;

            case Targeting.Air:
                if (possibleEnemy.flying)
                {
                    validTarget = true;
                }
                break;

            case Targeting.ObjectiveOnly:
                if (possibleEnemy.type == UnitType.Building || possibleEnemy.type == UnitType.RaidBoss || possibleEnemy.type == UnitType.RaidMinion)
                {
                    validTarget = true;
                }
                break;
        }

        return validTarget;
    }

    public static bool CurrentEnemyInRange(BattleState s, Unit u, Unit target)
    {
        if (target != null)
        {
            var distance = Vector3.Distance(u.transform.position, target.transform.position);
            distance -= target.radius;
            if (distance <= u.attackRange + u.radius && s.units.Contains(target))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    }

    public static void TurnTowardsEnemy(Unit u, Vector3 targetPosition)
    {
        if (u == null)
            return;

        Transform t = u.transform;
        if (u.anim != null && u.type == UnitType.Building)
        {
            t = u.anim.transform;
        }
        else if (u.anim == null)
        {
            return;
        }

        //Vector3 targetPostition = new Vector3(enemyPosition.x, t.position.y, enemyPosition.z);
        //t.LookAt(targetPostition);

        Vector3 lookPos = targetPosition - t.position;
        lookPos.y = 0;
        if (lookPos != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            t.rotation = Quaternion.Slerp(t.rotation, rotation, Time.deltaTime * 10f);
        }
    }
}
