using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PvPOpponent : MonoBehaviour
{
	public Text nameText;
	public Text rankText;
	public Text pointsText;
    public CardDisplay heroDisplay;
    public Text heroLevelText;
    public CardDisplay strongholdDisplay;
	public Text strongholdLevelText;
	public CardDisplay[] deckDisplay;
	public Text[] deckLevelsText;

	public DeckData deck;
	public DeckLevelsData levels;
    public DeckStarsData stars;

	public void StartFight()
	{
		Data.instance.pvpBattle.enemyName = nameText.text;
		Data.instance.pvpBattle.enemyHero = deck.heroes[0];
        Data.instance.pvpBattle.enemyDeck = deck.deck;
		Data.instance.pvpBattle.enemyObjectives = new string[1] { deck.stronghold };
		Data.instance.pvpBattle.enemyLevels = levels;
        Data.instance.pvpBattle.enemyStars = stars;

        SceneLoader.OfflineBattle(Data.instance.pvpBattle, BattleType.AsyncPvP, Data.instance.pvpBattle.battleScene);
	}
}
