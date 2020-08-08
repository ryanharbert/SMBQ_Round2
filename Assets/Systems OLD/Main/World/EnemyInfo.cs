using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfo : MonoBehaviour {

    public MeshRenderer foundEnemy;
    public TMPro.TextMeshPro nameText;
    public TMPro.TextMeshPro levelText;
    public Color tough;
    public Color normal;
    public Color easy;
    public int maxLevelDiff;
    public int easyLevelDiff;
    public GameObject star1;
    public GameObject star2;
	
	public void SetEnemyInfo(int level, string enemyName, float rectY, string id)
    {
        transform.localPosition = new Vector3(0, rectY, 0);
        levelText.text = level.ToString();
        nameText.text = enemyName;
        float levelDiff = 0f;
        if (Data.instance != null)
        {
            levelDiff = level - Data.instance.currency.playerLevel;
        }
        else
        {
            levelDiff = level - 1;
        }
        Color textColor = normal;
        if (levelDiff > easyLevelDiff)
        {
            textColor = Color.Lerp(normal, tough, Mathf.Min(Mathf.Abs((levelDiff - easyLevelDiff) / (maxLevelDiff - easyLevelDiff)), 1));
        }
        else if (levelDiff > 0)
        {
            textColor = Color.Lerp(easy, normal, Mathf.Min(Mathf.Abs(levelDiff / easyLevelDiff), 1));
        }
        else
        {
            textColor = easy;
        }
        nameText.color = textColor;
        levelText.color = textColor;


        //CardData card;
        //if(Data.instance.collection.allCards.TryGetValue(id, out card) && (card.type == CardType.Hero || card.type == CardType.Stronghold))
        //{
        //    star1.SetActive(true);
        //    star2.SetActive(true);
        //}
        //levelText.gameObject.SetActive(true);
    }
}
