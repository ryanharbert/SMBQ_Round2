using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmplifyPassiveAbility : AbilityType
{
    public int amplifyDuration;
    public int damageMult;
    public int radiusMult;

    public ParticleSystem particle;
    public AOEPassive passive;

    float amplifyTimer;
    

    public override void Effect()
	{
        if(!triggered)
        {
            StartCoroutine("Amplifying");
            triggered = true;
        }
    }

    public virtual IEnumerator Amplifying()
    {
        amplifyTimer = 0;

        passive.radius = passive.radius * radiusMult;
        passive.damage = passive.damage * damageMult;

        //var emission = particle.emission;
        //emission.rateOverTime = 1000;
        //var shape = particle.shape;
        //shape.radius = 3;
        //var main = particle.main;
        //main.startSize = 2;


        ParticleSystem.ShapeModule shape = particle.shape;
        shape.radius = shape.radius * radiusMult;

        ParticleSystem.MainModule main = particle.main;
        main.startSize = main.startSize.constant * radiusMult;

        //var emission = particle.emission;
        //var emissionRate =  emission.rateOverTime;
        //emissionRate.curveMultiplier = 3;

        while (amplifyTimer < amplifyDuration)
        {
            amplifyTimer += Time.deltaTime;
            yield return null;
        }

        passive.radius = passive.radius / radiusMult;
        passive.damage = passive.damage / damageMult;
        if(particle != null)
        {
            shape.radius = shape.radius / radiusMult;
            main.startSize = main.startSize.constant / radiusMult;
        }

        //emission.rateOverTime = 300;
        //shape.radius = 2;
        //startSize.curveMultiplier = 1;

        //startSize.curveMultiplier = 1;
        //emissionRate.curveMultiplier = 1;


    }

    protected override bool AfterDistanceDecision(BattleState s, Unit u, TargetInfo t, int index)
    {
        ((HeroUnit)u).photonView.RPC("Cast", RpcTarget.All, Vector3.zero, index);
        return true;
    }
}
