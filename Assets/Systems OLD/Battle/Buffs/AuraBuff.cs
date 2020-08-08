using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AuraBuff : UnitBuff
{
	public UnitBuff auraBuff;
	public bool friendly;
	public bool restricted;
	public List<string> unitRestrictions;
    public int radius;

	List<Unit> buffedUnits = new List<Unit>();

    public override void Run(BattleState s, Unit u)
    {
        base.Run(s, u);
        for(int i = 0; i < s.units.Count; i++)
        {
            if (s.units[i] != null && Vector3.Distance(s.units[i].transform.position, u.transform.position) < radius)
            {
                if ((friendly && s.units[i].teamID == u.teamID) || (!friendly && s.units[i].teamID != u.teamID))
                {
                    if (!buffedUnits.Contains(s.units[i]) && !RestrictedUnit(s.units[i]))
                    {
                        bool alreadyHaveBuff = false;
                        for (int h = 0; h < u.buffs.Count; h++)
                        {
                            if (u.buffs[h].name == auraBuff.name)
                            {
                                alreadyHaveBuff = true;
                            }
                        }
                        if (!alreadyHaveBuff)
                        {
                            s.units[i].photonView.RPC("AddBuff", RpcTarget.All, auraBuff.name);
                            buffedUnits.Add(s.units[i]);
                        }
                    }
                }
            }
            else
            {
                if(buffedUnits.Contains(s.units[i]))
                {
                    s.units[i].photonView.RPC("RemoveBuff", RpcTarget.All, auraBuff.name);
                    buffedUnits.Remove(s.units[i]);
                }
            }
        }
    }

    protected bool RestrictedUnit(Unit u)
    {
        if(restricted)
        {
            for(int i = 0; i < unitRestrictions.Count; i++)
            {
                if(u.name.Contains(unitRestrictions[i]))
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void End(Unit u)
    {
        base.End(u);

        for (int i = 0; i < buffedUnits.Count; i++)
        {
            if (buffedUnits[i] != null)
            {
                buffedUnits[i].RemoveBuff(auraBuff.name);
                buffedUnits.Remove(buffedUnits[i]);
            }
        }

    }
}
