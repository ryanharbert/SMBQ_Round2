using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour {

    protected virtual void FindObjective(BattleState s, Unit u)
    {
        if (u.targetInfo.distance > (u.aggroRange + u.radius))
        {
            if (u.teamID == 0 && s.enemyObjectives.Count != 0)
            {
                float objDistance = 0;
                for (int i = 0; i < s.enemyObjectives.Count; i++)
                {
                    float newDistance = Vector3.Distance(u.transform.position, s.enemyObjectives[i].transform.position);
                    if (objDistance == 0 || objDistance > newDistance)
                    {
                        objDistance = newDistance;
                        u.targetInfo.targetUnit = s.enemyObjectives[i];
                        u.targetInfo.position = s.enemyObjectives[i].transform.position;
                    }
                }
                u.targetInfo.distance = objDistance;
            }
            else if (u.teamID == 1 && s.playerObjectives.Count != 0)
            {
                float objDistance = 0;
                for (int i = 0; i < s.playerObjectives.Count; i++)
                {
                    float newDistance = Vector3.Distance(u.transform.position, s.playerObjectives[i].transform.position);
                    if (objDistance == 0 || objDistance > newDistance)
                    {
                        objDistance = newDistance;
                        u.targetInfo.targetUnit = s.playerObjectives[i];
                        u.targetInfo.position = s.playerObjectives[i].transform.position;
                    }
                }
                u.targetInfo.distance = objDistance;
            }
            else
            {
                u.targetInfo.targetUnit = null;
            }
        }
    }

    public virtual void Execute(BattleState s, Unit u)
    {
        FindObjective(s, u);

        if (u.targetInfo.targetUnit == null && u.targetInfo.position == Vector3.zero)
        {
            Idle(u);
        }
        else if (u.speed != UnitSpeed.Immobile)
        {
            MoveUnit(u);
        }
    }

    protected void Idle(Unit u)
    {
		if(!u.flying)
		{
			u.nav.destination = u.transform.position;
			u.nav.velocity = Vector3.zero;
			u.nav.isStopped = true;
		}
		if (u.state != 0)
		{
			u.photonView.RPC("TriggerAnimationRPC", Photon.Pun.RpcTarget.All, 0);
			u.state = 0;
		}
    }

    protected void MoveUnit(Unit u)
    {
        if (u.state != 1)
        {
            u.photonView.RPC("TriggerAnimationRPC", Photon.Pun.RpcTarget.All, 1);
            u.state = 1;
        }
        if(u.nav == null)
        {
            Debug.Log(u);
            Debug.Log(u.name);
        }
        if (u.targetInfo.targetUnit != null)
        {
            RaycastHit hit;
            Vector3 rayDirection = ((u.targetInfo.targetUnit.transform.position - u.transform.position).normalized);
            Ray ray = new Ray(u.transform.position, rayDirection);

            if (u.targetInfo.targetUnit.capsule != null && u.targetInfo.targetUnit.capsule.Raycast(ray, out hit, 10f))
            {
				u.targetInfo.position = hit.point;
            }
            else
			{
				u.targetInfo.position = u.targetInfo.targetUnit.transform.position;
			}
        }

		if(!u.flying)
		{
			u.nav.isStopped = false;
			u.nav.SetDestination(u.targetInfo.position);
		}
		else
		{
			u.transform.position = Vector3.MoveTowards(u.transform.position, u.targetInfo.position, Time.deltaTime * u.nav.speed);
			UnitSystem.TurnTowardsEnemy(u, u.targetInfo.position);
		}
    }

    public virtual void Exit(BattleState s, Unit u)
    {

    }
}
