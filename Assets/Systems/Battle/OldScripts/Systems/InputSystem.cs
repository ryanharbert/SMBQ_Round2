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

		s.cardSelected = -1;
	}

	public override void Run(BattleState s)
	{
		if(Time.timeScale == 0)
			return;

        if (BattleCardGroup.selectedCard != null && BattleCardGroup.selectedCard.index > 4 && !BattleHero.alive)
		{
			BattleCardGroup.selectedCard = null;
		}

		if (s.cardSelected == 5 && s.playerHero.Unit.abilities[0].validPlayArea == ValidPlayArea.OnClick && s.hero != null && s.hero.abilities[0].CDTimer <= 0)
		{
			s.playCard = true;
			s.spawnPosition = Vector3.up;
            return;
		}
		else if (s.cardSelected == 6 && s.playerHero.Unit.abilities[1].validPlayArea == ValidPlayArea.OnClick && s.hero != null && s.hero.abilities[1].CDTimer <= 0)
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
		if (s.cardSelected > 4)
		{
            if(Battle.state.hero != null)
            {
                ValidPlayArea validPlayArea = ValidPlayArea.OtherCollider;
                if (s.cardSelected == 5)
                {
                    s.spawnCollider = Battle.state.hero.abilities[0].inputArea;
                    validPlayArea = Battle.state.playerHero.Unit.abilities[0].validPlayArea;
                }
                else if (s.cardSelected == 6)
                {
                    s.spawnCollider = Battle.state.hero.abilities[1].inputArea;
                    validPlayArea = Battle.state.playerHero.Unit.abilities[1].validPlayArea;
                }

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
