using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : BaseSystem
{

	public override void Init(BattleState s)
	{
        SpawnArea[] spawnAreas = FindObjectsOfType<SpawnArea>();
        foreach(SpawnArea a in spawnAreas)
        {
            if(a.type == SpawnAreaType.Player)
            {
                s.playerSpawnArea = a;
            }
            else if(a.type == SpawnAreaType.All)
            {
                s.anywhereSpawnArea = a;
            }
            else if(a.type == SpawnAreaType.Enemy)
            {
                s.enemySpawnArea = a;
            }
        }
        s.playerSpawnArea.display.SetActive(false);
        s.anywhereSpawnArea.display.SetActive(false);

        if(s.enemySpawnArea != null)
        {
            s.enemySpawnArea.display.SetActive(false);
        }

		s.mouseDownAnimation = Instantiate(Battle.instance.mouseDownAnimation);
		s.mouseDownAnimation.SetActive(false);
	}

	public override void Run(BattleState s)
	{
		if(Time.timeScale == 0)
			return;

        if (BattleCardGroup.selectedCard != null && BattleCardGroup.selectedCard is BattleAbility && !((BattleAbility)BattleCardGroup.selectedCard).hero.alive)
		{
			BattleCardGroup.selectedCard = null;
		}

		if (s.selectedCard is BattleAbility && s.playerHero[((BattleAbility)s.selectedCard).hero.index].Unit.abilities[((BattleAbility)s.selectedCard).index].validPlayArea == ValidPlayArea.OnClick && s.hero[((BattleAbility)s.selectedCard).hero.index] != null && s.hero[((BattleAbility)s.selectedCard).hero.index].abilities[((BattleAbility)s.selectedCard).index].CDTimer <= 0)
		{
			s.playCard = true;
			s.spawnPosition = Vector3.up;
            return;
		}

		if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && s.playCard == false)
		{
			s.ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (GetSpawnPoint(s))
			{
				if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
				{
					s.mouseDownAnimation.transform.position = s.hit.point;
					s.mouseDownAnimation.SetActive(true);
				}
				else
				{
					s.playCard = true;
					s.mouseDownAnimation.SetActive(false);
				}
			}
			else
			{
				s.mouseDownAnimation.SetActive(false);
			}
		}
	}



	bool GetSpawnPoint(BattleState s)
	{
		if (s.selectedCard is BattleAbility)
		{
            BattleAbility a = ((BattleAbility)s.selectedCard);
            if (Battle.state.hero[a.hero.index] != null)
            {
                ValidPlayArea validPlayArea = ValidPlayArea.OtherCollider;
                s.spawnCollider = Battle.state.hero[a.hero.index].abilities[a.index].inputArea;
                validPlayArea = Battle.state.playerHero[a.hero.index].Unit.abilities[a.index].validPlayArea;

                if (validPlayArea == ValidPlayArea.OtherCollider)
                {
                    if (!s.anywhereSpawnArea.collider.Raycast(s.ray, out s.hit, 100f))
                    {
                        return false;
                    }
                }
                else if (validPlayArea == ValidPlayArea.YourSide)
                {
                    if (Battle.state.teamID == 0)
                    {
                        s.spawnCollider = s.playerSpawnArea.collider;
                    }
                    else
                    {
                        s.spawnCollider = s.enemySpawnArea.collider;
                    }
                }
                else
                {
                    s.spawnCollider = s.anywhereSpawnArea.collider;
                }
            }
		}
		else if (BattleCardGroup.selectedCard == null)
		{
            return false;
        }
        else if (BattleCardGroup.selectedCard.cardData.validPlayArea == ValidPlayArea.Anywhere)
        {
            s.spawnCollider = s.anywhereSpawnArea.collider;
        }
        else
		{
			if (Battle.state.teamID == 0)
			{
				s.spawnCollider = s.playerSpawnArea.collider;
			}
			else
			{
				s.spawnCollider = s.enemySpawnArea.collider;
			}
		}

		if (s.spawnCollider != null)
		{
			if (s.spawnCollider.Raycast(s.ray, out s.hit, 100f))
			{
				s.spawnPosition = s.hit.point;
				return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
}
