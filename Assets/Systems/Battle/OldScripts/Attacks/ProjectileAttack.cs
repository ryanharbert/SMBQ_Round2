using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileAttack : BasicAttack
{
	public Projectile projectile;
	public Transform projectileStartLocation;

	protected override void DealDamage(BattleState s, Unit u)
	{
        u.photonView.RPC("SpawnProjectile", RpcTarget.All, u.targetInfo.targetUnit.photonView.ViewID, u.photonView.ViewID);
	}

    [PunRPC]
    public void SpawnProjectile(int enemyID, int unitID)
    {
        Unit u = PhotonView.Find(unitID).GetComponent<Unit>();
        Unit enemy = PhotonView.Find(enemyID).GetComponent<Unit>();

        Projectile p = Instantiate(projectile, new Vector3(projectileStartLocation.position.x, u.transform.position.y, projectileStartLocation.position.z), u.transform.rotation).GetComponent<Projectile>();
        p.displayObject.transform.position = projectileStartLocation.position;
        p.displayObject.transform.LookAt(enemy.transform.position + new Vector3(0f, enemy.projectileHeight, 0f));

        p.source = u;
        p.sourcePos = u.transform.position;
        p.sourceHeight = u.projectileHeight;

        p.enemy = enemy;
        p.enemyPos = enemy.transform.position;
        p.enemyHeight = enemy.projectileHeight;

        p.damage = u.attackDamage;

        Battle.state.projectiles.Add(p);
    }
}
