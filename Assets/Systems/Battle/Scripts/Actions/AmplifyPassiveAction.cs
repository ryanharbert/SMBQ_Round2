using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AmplifyPassiveAction : UnitAction
{
    public bool amplify;
    public int damageMult;
    public int radiusMult;

    private Unit u;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        AmplifyDisplay a = GetComponentInParent<AmplifyDisplay>();

        if (amplify)
        {
            a.AOE.radius = a.AOE.radius * radiusMult;
            a.AOE.amount = a.AOE.amount * damageMult;

            ParticleSystem.ShapeModule shape = a.particle.shape;
            shape.radius = shape.radius * radiusMult;

            ParticleSystem.MainModule main = a.particle.main;
            main.startSize = main.startSize.constant * radiusMult;
        }
        else
        {
            a.AOE.radius = a.AOE.radius / radiusMult;
            a.AOE.amount = a.AOE.amount / damageMult;

            ParticleSystem.ShapeModule shape = a.particle.shape;
            shape.radius = shape.radius / radiusMult;

            ParticleSystem.MainModule main = a.particle.main;
            main.startSize = main.startSize.constant / radiusMult;
        }
    }
}
