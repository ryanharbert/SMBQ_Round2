using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleData
{
	public string enemyName;
	public string enemyHero;
	public string[] enemyObjectives;
	public string[] enemyDeck;
	public string battleScene;
	public int level;
	public DeckLevelsData enemyLevels;
    public DeckStarsData enemyStars;
    public string nodeLocation;
}
