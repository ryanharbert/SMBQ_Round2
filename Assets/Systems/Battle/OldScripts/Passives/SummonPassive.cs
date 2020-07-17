using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonPassive : PassiveType
{
	public CardData card;
	public Transform spawnPosition;

    public bool random = false;
    public int randomness = 0;

    protected override void Awake()
    {
        base.Awake();
        if (random)
        {
        }
    }

    public override void Use(BattleState s, Unit u)
	{
		base.Use(s, u);

		card.PlayCard(s, spawnPosition.position, u.teamID, u.level);
        if(random)
        {
        }
	}
}