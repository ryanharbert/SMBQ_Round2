using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossSteveFireballsAction : UnitAction
{
    public Projectile projectile;
    public int damage;
    public List<Transform> projectileStartLocations;

    [HideInInspector] public FireBallCastArea castArea;

    bool inRange;

    private void Awake()
    {
        castArea = FindObjectOfType<FireBallCastArea>();
    }

    public override void Execute(Unit u, TargetInfo t, int i)
    {
        base.Execute(u, t, i);

        u.nav.isStopped = false;
        u.nav.destination = castArea.transform.position;
        u.targetInfo.position = castArea.transform.position;
        inRange = false;
        u.photonView.RPC("TriggerAnimationRPC", Photon.Pun.RpcTarget.All, 1);
    }

    public override void Channel(BattleState s, Unit u)
    {
        if (inRange)
        {
            base.Channel(s, u);
        }
        else if (!inRange && Vector3.Distance(transform.position, castArea.transform.position) < 0.5f)
        {
            inRange = true;

            u.nav.destination = u.transform.position;
            u.nav.velocity = Vector3.zero;
            u.nav.isStopped = true;

            u.photonView.RPC("TriggerAnimationRPC", Photon.Pun.RpcTarget.All, 4);
        }
    }

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        List<Unit> enemies = new List<Unit>();

        for(int i = 0; i < Battle.state.units.Count; i++)
        {
            if(Battle.state.units[i].teamID != u.teamID)
            {
                enemies.Add(Battle.state.units[i]);
            }
        }

        for(int i = 0; i < projectileStartLocations.Count; i++)
        {
            Unit enemy = enemies[Random.Range(0, enemies.Count - 1)];
            u.photonView.RPC("SpawnFireBalls", RpcTarget.All, enemy.photonView.ViewID, i);
            if (projectileStartLocations.Count <= (enemies.Count - i))
			{
				enemies.Remove(enemy);
			}
        }
    }

    [PunRPC]
    public void SpawnFireBalls(int enemyID, int index)
    {
        Unit u = GetComponent<Unit>();
        Unit enemy = PhotonView.Find(enemyID).GetComponent<Unit>();
        float targetHeight = enemy.projectileHeight;    

        Projectile p = Instantiate(projectile, new Vector3(projectileStartLocations[index].position.x, projectileStartLocations[index].position.y, projectileStartLocations[index].position.z), u.transform.rotation).GetComponent<Projectile>();
        p.displayObject.transform.position = projectileStartLocations[index].position;
        p.displayObject.transform.LookAt(enemy.transform.position + new Vector3(0f, targetHeight, 0f));

        p.source = u;
        p.sourcePos = projectileStartLocations[index].position;
        p.sourceHeight = projectileStartLocations[index].position.y;
        p.teamID = u.teamID;

        p.enemy = enemy;
        p.enemyPos = enemy.transform.position;
        p.enemyHeight = targetHeight;
        
        p.damage = u.LevelandTypeBonus(damage);

        Battle.state.projectiles.Add(p);
    }
}
