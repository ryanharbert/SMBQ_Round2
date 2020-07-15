using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardMove : UnitMove
{
    [HideInInspector] public Vector3 startPos;
    [HideInInspector] public Quaternion startRot;
    [HideInInspector] public bool disabled;

    private void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        disabled = false;
    }

    protected override void FindObjective(BattleState s, Unit u)
    {
        if (s.gameTimer > s.gameLength)
        {
            base.FindObjective(s, u);
        }
        else if (u.targetInfo.distance > (u.aggroRange + u.radius))
        {
            u.targetInfo.targetUnit = null;
        }
    }

    public override void Execute(BattleState s, Unit u)
    {
        FindObjective(s, u);

        if (u.targetInfo.targetUnit != null)
        {
            MoveUnit(u);
        }
        else if (Vector3.Distance(startPos, transform.position) > 0.5f && !disabled)
        {
            u.targetInfo.position = startPos;
            u.targetInfo.targetUnit = null;
            MoveUnit(u);
        }
        else
        {
            Idle(u);
            if (transform.rotation != startRot && !disabled)
            {
                u.transform.rotation = Quaternion.RotateTowards(u.transform.rotation, startRot, 200f * Time.deltaTime);
            }
        }
    }
}
