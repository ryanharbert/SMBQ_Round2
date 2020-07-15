using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackType : MonoBehaviour
{
	public void Attack(BattleState s, Unit u)
	{
		//if (u.attackTimer == 0)
  //      {
  //          u.photonView.RPC("TriggerAnimationRPC", Photon.Pun.RpcTarget.All, UnitState.Attacking);
  //      }

		//u.attackTimer += Time.deltaTime;

		//if (u.attackLength < u.attackTimer)
		//{
		//	u.state = UnitState.Idle;
		//}
		//else if (!u.attacked)
		//{
		//	if (u.enemy == null)
		//	{
		//		u.state = UnitState.Idle;
		//		return;
		//	}

		//	UnitSystem.TurnTowardsEnemy(u, u.enemy.transform.position);

		//	if (u.attackDelay < u.attackTimer)
		//	{
		//		u.attacked = true;
		//		DealDamage(s, u);
		//	}
		//}
	}

	protected virtual void DealDamage(BattleState s, Unit u)
	{
       // u.targetInfo.targetUnit.TakeDamage(new DamageData() { amount = u.attackDamage, source = u, type = DamageType.Ranged });
    }
}
