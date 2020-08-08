using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class UnitAbility
{
    //DATA
    public string displayName;
    public string shortDesc;
    public string longDesc;
    public Collider inputArea;
    public MeshRenderer inputAreaDisplay;
    public ValidPlayArea validPlayArea;
    public Sprite abilitySprite;
    public int cooldown;
    public UnitAction action;

    //STATE
    [HideInInspector] public float CDTimer;
    [HideInInspector] public bool disabled = false;

    //AI
    public Targeting AI_Targeting;
    public float AI_TriggerDistance;
    public DecisionType decisionType;
    public int randomness;

    protected UnitDecision AI_Decision;


    public bool Use(BattleState s, Unit u, int index)
    {
        if(disabled)
        {
            return false;
        }

        if (CDTimer <= 0)
        {
            if (Decision(s, u))
            {
                CDTimer = cooldown;
                if (randomness != 0)
                {
                    CDTimer += Random.Range(-randomness, randomness);
                }
                if(u.type == UnitType.RaidBoss || u.type == UnitType.Building)
                {
                    u.photonView.RPC("ResetCD", RpcTarget.Others, index, Mathf.RoundToInt(CDTimer));
                }
                action.Execute(u, u.targetInfo, index);
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

    bool Decision(BattleState s, Unit u)
    {
        if(decisionType == DecisionType.None)
        {
            return true;
        }
        else if (decisionType == DecisionType.AlwaysFalse)
        {
            return false;
        }
        else if(AI_Decision == null)
        {
            switch (decisionType)
            {
                case DecisionType.DistanceCheck:
                    AI_Decision = new DistanceCheck();
                    break;
                case DecisionType.ChargeCheck:
                    AI_Decision = new ChargeCheck();
                    break;
                case DecisionType.TeleportAway:
                    AI_Decision = new TeleportAwayCheck();
                    break;
                case DecisionType.LowHealthAllyCheck:
                    AI_Decision = new LowHealthAllyCheck();
                    break;
                case DecisionType.NoEnemiesCheck:
                    AI_Decision = new NoEnemiesCheck();
                    break;
            }
        }

        return AI_Decision.Check(s, u, AI_Targeting, AI_TriggerDistance);
    }


}
