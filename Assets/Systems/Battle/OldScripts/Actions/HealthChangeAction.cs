using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HealthChangeAction : UnitAction
{
    public bool self;
    public bool percentage = false;
    public int amount;

    public GameObject displayObject;
    public GameObject selfDisplayObject;

    public override void Trigger(Unit u, TargetInfo t, ActionType type, int index)
    {
        if(self)
        {
            ChangeHealth(u, u);
        }
        else
        {
            ChangeHealth(u, t.targetUnit);
        }
    }

    void ChangeHealth(Unit u, Unit target)
    {
        if (target.health > 0 && (target.health < target.maxHealth || amount < 0))
        {
            if(!percentage)
            {
                target.health += u.LevelandTypeBonus(amount);
            }
            else
            {
                target.health += Mathf.RoundToInt(target.maxHealth * amount * 0.01f);
            }
        }

        if(displayObject != null || selfDisplayObject != null)
        {
            u.photonView.RPC("AbilityDisplayPosition", RpcTarget.All, type, index, target.transform.position);
        }
    }

    public override void PositionDisplay(Vector3 position)
    {
        if (displayObject != null)
        {
            GameObject display = Instantiate(displayObject);
            display.transform.position = position;
            display.transform.rotation = Quaternion.identity;
            Destroy(display, 2f);
        }

        if(selfDisplayObject != null)
        {
            selfDisplayObject.SetActive(true);
            Invoke("DisableSelfDisplay", 2f);
        }
    }

    void DisableSelfDisplay()
    {
        selfDisplayObject.SetActive(false);
    }
}
