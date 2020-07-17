using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileAction : UnitAction
{
    public Projectile projectile;
    public Transform projectileStartLocation;
    public bool useAttackDamage = true;
    public int amount;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        int enemyID = -1;
        Vector3 enemyPos = t.position;
        if(t.targetUnit != null)
        {
            enemyID = t.targetUnit.photonView.ViewID;
        }
        u.photonView.RPC("SpawnProjectile", RpcTarget.All, type, index, enemyID, enemyPos, u.photonView.ViewID);
    }
    
    public override void SpawnProjectile(int enemyID, Vector3 targetPosition, int unitID)
    {
        Unit u = PhotonView.Find(unitID).GetComponent<Unit>();
        Unit enemy = null;
        float targetHeight = 1;
        if (enemyID != -1)
        {
            enemy = PhotonView.Find(enemyID).GetComponent<Unit>();
            targetHeight = enemy.projectileHeight;
        }


        Projectile p = Instantiate(projectile, new Vector3(projectileStartLocation.position.x, u.transform.position.y, projectileStartLocation.position.z), u.transform.rotation).GetComponent<Projectile>();
        p.displayObject.transform.position = projectileStartLocation.position;
        p.displayObject.transform.LookAt(targetPosition + new Vector3(0f, targetHeight, 0f));

		p.range = u.attackRange + u.radius;
        p.source = u;
		p.sourcePos = projectileStartLocation.position;
        p.sourceHeight = u.projectileHeight;
        p.teamID = u.teamID;

        p.enemy = enemy;
        p.enemyPos = targetPosition;
        p.enemyHeight = targetHeight;

        if(useAttackDamage)
        {
            p.damage = u.attackDamage;
        }
        else
        {
            p.damage = u.LevelandTypeBonus(amount);
        }

        Battle.state.projectiles.Add(p);
    }
}
