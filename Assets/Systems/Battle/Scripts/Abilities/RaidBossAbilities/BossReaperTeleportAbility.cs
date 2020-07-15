using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossReaperTeleportAbility : AbilityType
{
	public GameObject particleEffect;
    public int teleportDelay;
    public int teleportCount;

    float timer;
    bool waitingForAbilityEnd;

    protected override void Update()
    {
        base.Update();
    }

    public override void Effect()
    {
        if (!triggered)
        {
            triggered = true;
            StartCoroutine("Teleporting");
        }
    }

    public virtual IEnumerator Teleporting()
    {
        Light sunLight = FindObjectOfType<RaidSpecialObject>().GetComponent<Light>();
        Color defaultColor = sunLight.color;
        while(sunLight.color != Color.red)
        {
            sunLight.color = Color.Lerp(sunLight.color, Color.red, Time.deltaTime * 12f);
            yield return null;
        }

        if(PhotonNetwork.IsMasterClient)
        {
            GuardMove g = GetComponent<GuardMove>();
            g.disabled = true;

            //ENEMIES
            for (int i = 0; i < teleportCount; i++)
            {
                if (TeleportToRandomEnemy())
                {
                    timer = 0;
                    while (timer < teleportDelay)
                    {
                        timer += Time.deltaTime;
                        yield return null;
                    }
                }
                else
                {
                    break;
                }
            }

            //RETURN
            this.photonView.RPC("Teleport", RpcTarget.All, g.startPos);
            transform.rotation = g.startRot;

            g.disabled = false;
            this.photonView.RPC("EndReaperAbility", RpcTarget.Others);
        }
        else
        {
            waitingForAbilityEnd = true;
            while (waitingForAbilityEnd)
            {
                yield return null;
            }
        }

        while (sunLight.color != defaultColor)
        {
            sunLight.color = Color.Lerp(sunLight.color, defaultColor, Time.deltaTime * 12f);
            yield return null;
        }
    }

    public bool TeleportToRandomEnemy()
	{
        Unit randomEnemy = GetRandomEnemy();

        if(randomEnemy != null)
        {
            this.photonView.RPC("Teleport", RpcTarget.All, randomEnemy.transform.position);
            return true;
        }
        return false;
    }

    [PunRPC]
    public void EndReaperAbility()
    {
        waitingForAbilityEnd = false;
    }


    [PunRPC]
    public void Teleport(Vector3 position)
    {
        DisplayAbility(transform.position);

        transform.position = position;
        u.nav.Warp(position);
        if (!Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            u.masterPosition = position;
        }

        DisplayAbility(transform.position);
    }



    Unit GetRandomEnemy()
    {
        BattleState s = Battle.state;
        List<Unit> possibleTargets = new List<Unit>();
        for (int i = 0; i < s.units.Count; i++)
        {
            if (s.units[i] == null)
            {
                continue;
            }

            if (s.units[i].teamID != u.teamID && UnitSystem.TargetValid(s, u, s.units[i], AITargeting) && s.units[i].type != UnitType.Building)
            {
                possibleTargets.Add(s.units[i]);
            }
        }
        if(possibleTargets.Count > 0)
        {
            return possibleTargets[Random.Range(0, possibleTargets.Count-1)];
        }
        return null;
    }

    void DisplayAbility(Vector3 position)
	{
		if (particleEffect != null)
		{
			GameObject original = Instantiate(particleEffect, position, Quaternion.identity);
			Destroy(original, 4f);
		}
    }

    public override bool Decision(BattleState s, Unit u, TargetInfo t, int index)
    {
        if (CDTimer <= 0)
        {
            ((HeroUnit)u).photonView.RPC("Cast", RpcTarget.All, Vector3.zero, index);
            return true;
        }
        return false;
    }
}
