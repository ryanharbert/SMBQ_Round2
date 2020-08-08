using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbushMove : UnitMove
{
    public string getUpTrigger;
    public float getUpDelay;

    bool waiting;
    bool gettingUp;
    float getUpTimer;

    private void Awake()
    {
        waiting = true;
        gettingUp = false;
        getUpTimer = 0;
        GetComponent<Unit>().anim.Play("Ambush", 0, 1);
    }

    public override void Execute(BattleState s, Unit u)
    {
        if(waiting)
        {
            //TargetInfo t = new TargetInfo();

            //t = ClosestEnemy(s, u, t, u.targeting);
            if (u.targetInfo.targetUnit != null && u.targetInfo.distance <= (u.aggroRange + u.radius))
            {
                GetComponent<Unit>().anim.SetTrigger(getUpTrigger);
                waiting = false;
                gettingUp = true;
            }
        }
        else if(gettingUp)
        {
            getUpTimer += Time.deltaTime;
            if (getUpTimer > getUpDelay)
            {
                gettingUp = false;
            }
        }
        else
        {
            base.Execute(s, u);
        }
    }
}
