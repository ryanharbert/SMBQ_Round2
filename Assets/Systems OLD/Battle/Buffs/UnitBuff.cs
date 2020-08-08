using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class UnitBuff : MonoBehaviour
{
    public bool startOnCooldown = false;
    public UnitAction onStart;
    public UnitAction onTick;
    public UnitAction onEnd;
    public float tickRate;
    public int duration;
    public GameObject displayObject;

    [HideInInspector] public UnitAction displayAction;
    [HideInInspector] public Unit u;

    protected float tickTimer;
    protected float durationTimer;

    //AI
    public Targeting AI_Targeting;
    public float AI_TriggerDistance;
    public DecisionType decisionType;

    protected UnitDecision AI_Decision;

    public void Create(Unit u)
    {
        u.buffs.Add(this);
        if (onStart != null)
        {
            TriggerAction(u, onStart, ActionType.BuffStart);
        }

        if(startOnCooldown)
        {
            tickTimer = tickRate;
        }
        else
        {
            tickTimer = -1;
        }

        durationTimer = duration;

        ScaleBuff(u);
    }

    public virtual void Run(BattleState s, Unit u)
    {
        if(tickRate != -1 && onTick != null)
        {
            tickTimer -= Time.deltaTime;
            if (tickTimer < 0)
            {
                TriggerAction(u, onTick, ActionType.BuffTick);
                tickTimer = tickRate;
            }
        }

        if(duration != -1)
        {
            durationTimer -= Time.deltaTime;
            if (durationTimer < 0)
            {
                RemoveBuff(u);
            }
        }
    }

    public void RemoveBuff(Unit u)
    {
        u.photonView.RPC("RemoveBuff", Photon.Pun.RpcTarget.All, name);
    }

    public virtual void End(Unit u)
    {
        if (onEnd != null)
        {
            TriggerAction(u, onEnd, ActionType.BuffEnd);
        }
    }

    bool Decision(BattleState s, Unit u)
    {
        if (decisionType == DecisionType.None)
        {
            return true;
        }
        else if (AI_Decision == null)
        {
            switch (decisionType)
            {
                case DecisionType.DistanceCheck:
                    AI_Decision = new DistanceCheck();
                    break;
                case DecisionType.ChargeCheck:
                    AI_Decision = new ChargeCheck();
                    break;
            }
        }

        return AI_Decision.Check(s, u, AI_Targeting, AI_TriggerDistance);
    }

    public void TriggerAction(Unit u, UnitAction a, ActionType type)
    {
        for (int i = 0; i < u.buffs.Count; i++)
        {
            if (u.buffs[i].name == name)
            {
                displayAction = a;
                a.Trigger(u, u.targetInfo, type, i);
                break;
            }
        }
    }

    protected void ScaleBuff(Unit u)
    {
        if(displayObject != null)
        {
            switch (u.size)
            {
                case UnitSize.Tiny:
                    displayObject.transform.localScale = displayObject.transform.localScale * 0.8f;
                    break;
                case UnitSize.Small:
                    displayObject.transform.localScale = displayObject.transform.localScale * 1f;
                    break;
                case UnitSize.Medium:
                    displayObject.transform.localScale = displayObject.transform.localScale * 1.2f;
                    break;
                case UnitSize.Large:
                    displayObject.transform.localScale = displayObject.transform.localScale * 1.7f;
                    break;
                case UnitSize.Giant:
                    displayObject.transform.localScale = displayObject.transform.localScale * 2.3f;
                    break;
                case UnitSize.Building:
                    displayObject.transform.localScale = displayObject.transform.localScale * 3.5f;
                    break;
                case UnitSize.Stronghold:
                    displayObject.transform.localScale = displayObject.transform.localScale * 5f;
                    break;
            }
        }
    }
}
