using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SelfHealAbility : AbilityType
{
    public float tickRate;
    public int tickHeal;

    //Display
    public GameObject displayObject;

    protected float secondaryTimer;

    private void Start()
    {
        displayObject.SetActive(false);
    }

    public override void Effect()
    {
        if (!triggered)
        {
            triggered = true;
            AbilityDisplay(true);
            StartCoroutine("Healing");
        }
    }

    public virtual IEnumerator Healing()
    {
        while (durationTimer < castDuration)
        {
            durationTimer += Time.deltaTime;
            secondaryTimer += Time.deltaTime;

            if (secondaryTimer > tickRate)
            {
                secondaryTimer = 0;

                if (PhotonNetwork.IsMasterClient)
                {
                    if (u.health > 0 && u.health < u.maxHealth)
                    {
                        u.health += u.LevelandTypeBonus(tickHeal);
                    }
                }
            }
            yield return null;
        }
        AbilityDisplay(false);
    }

    public void AbilityDisplay(bool active)
    {
        displayObject.SetActive(active);
    }

    public override bool Decision(BattleState s, Unit u, TargetInfo t, int index)
    {
        if (CDTimer <= 0)
        {
            if((u.maxHealth * 0.7f) > u.health)
            {
                ((HeroUnit)u).photonView.RPC("Cast", RpcTarget.All, Vector3.zero, index);
                return true;
            }
        }
        return false;
    }
}
