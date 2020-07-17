using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class UnitAction : MonoBehaviour
{
    //DATA
    public bool channel = true;
    public float delay;
    public float duration;
    public bool needsTarget;

    //STATE
    protected bool triggered;
    protected float durationTimer;

    protected int index;
    protected bool ability;
    protected ActionType type;

    public virtual void Execute(Unit u, TargetInfo t, int i)
    {
        index = i;
        ability = true;

        if (channel)
        {
            durationTimer = 0;
            triggered = false;

            if(u.nav != null && !u.flying)
            {
                u.nav.destination = u.transform.position;
                u.nav.velocity = Vector3.zero;
                u.nav.isStopped = true;
            }

            if(i == -1)
            {
                u.state = 3;
                type = ActionType.Attack;
            }
            else
            {
                u.state = i + 4;
                type = ActionType.Ability;
            }
            if(this is SummonAction && u.type == UnitType.Building && u.passiveAnim != null)
            {
                u.photonView.RPC("TriggerAnimationRPC", Photon.Pun.RpcTarget.All, -1);
            }
            else if(u.anim != null)
            {
                u.photonView.RPC("TriggerAnimationRPC", Photon.Pun.RpcTarget.All, u.state);
            }
        }
        else
        {
            if (i == -1)
            {
                type = ActionType.Attack;
            }
            else
            {
                type = ActionType.Ability;
            }
            Trigger(u, t, type, index);
        }
    }

    public virtual void Channel(BattleState s, Unit u)
	{
		if (durationTimer < duration)
        {
            durationTimer += Time.deltaTime;
            if (durationTimer > delay)
            {
                Effect(u);
            }
            else if(needsTarget && u.targetInfo.targetUnit == null)
            {
                u.state = 0;
            }
        }
        else
        {
            u.state = 0;
		}
		UnitSystem.TurnTowardsEnemy(u, u.targetInfo.position);
	}

    public virtual void Effect(Unit u)
    {
        if (!triggered)
        {
            triggered = true;
            Trigger(u, u.targetInfo, type, index);
        }
    }

    public virtual void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {

    }

    public virtual void PositionDisplay(Vector3 position)
    {

    }

    public virtual void BoolDisplay(bool active)
    {

    }

    public virtual void SpawnProjectile(int enemyID, Vector3 enemyPosition, int unitID)
    {

    }
}
