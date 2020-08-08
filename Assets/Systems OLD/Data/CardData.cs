using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class CardData : ScriptableObject {

	public Unit[] Units
    {
        get
        {
            return starPowers[starLevel].units;
        }
    }

	public Vector3[] Offsets
    {
        get
        {
            if(starPowers[starLevel].offsets != null && starPowers[starLevel].offsets.Length > 0)
            {
                return starPowers[starLevel].offsets;
            }
            else
            {
                return starPowers[0].offsets;
            }
        }
    }

    public Unit Unit
    {
        get
        {
            return starPowers[starLevel].units[0];
        }
    }

    public int ManaCost
    {
        get
        {
            if (starPowers[starLevel].manaCost != 0)
            {
                return starPowers[starLevel].manaCost;
            }
            else
            {
                return starPowers[0].manaCost;
            }
        }
    }

    public StarPowerData[] starPowers;


    public Sprite cardDisplay;
    public CardType type;
	public Faction faction;
	public ValidPlayArea validPlayArea;
	public string description;

	// Hero/Building
	[TextArea]
	public string shortDesc;

	[HideInInspector] public string itemID;
	[HideInInspector] public string displayName;
	[HideInInspector] public int amountOwned;
	[HideInInspector] public int level;
    [HideInInspector] public int starLevel;
    [HideInInspector] public bool typeBuff;

	public int AttackDamage
	{
		get
		{
			return GetStatAtLevel(Unit.baseAttack, level);
		}
	}
	public int Health
	{
		get
		{
			return GetStatAtLevel(Unit.baseHealth, level);
		}
	}

    public bool MaxLevel
    {
        get
        {
            return (level > Data.instance.values.upgradeCost.Length);
        }
    }

	public Stat[] stats;

	public int AmountNeeded
	{
		get
		{
            return Data.instance.values.upgradeCost[(level - 1)];
		}
	}

	public int GoldNeeded
	{
		get
		{
            return Data.instance.values.upgradeGoldCost[(level - 1)];
        }
	}

	public bool Upgradeable
	{
		get
		{
            if(!MaxLevel)
            {
                return (amountOwned >= AmountNeeded);
            }
            else
            {
                return false;
            }
		}
	}

	public int GetStatAtLevel(int stat, int level)
	{
		stat = Mathf.RoundToInt(stat * Mathf.Pow(1.1f, level - 1));
		return stat;
	}


    public void PlayCard(BattleState s, Vector3 position, int teamID, int level, int starLevel)
    {
        for (int i = 0; i < Units.Length; i++)
        {
            Vector3 offset = Offsets[i];
            Quaternion rotation = s.unitRotation.rotation;
            if (teamID == 1)
            {
                offset.x = offset.x * -1;
                rotation = Quaternion.Inverse(s.unitRotation.rotation);
            }

			Unit u = Photon.Pun.PhotonNetwork.Instantiate("Units/" + starLevel + "/" + Units[i].gameObject.name, position + offset, rotation, 0).GetComponent<Unit>();
            u.photonView.RPC("SetRPC", Photon.Pun.RpcTarget.All, teamID, level);
        }
    }

    public void PlayCard(BattleState s, Vector3 position, int teamID, int level)
    {
        PlayCard(s, position, teamID, level, starLevel);
    }
}
