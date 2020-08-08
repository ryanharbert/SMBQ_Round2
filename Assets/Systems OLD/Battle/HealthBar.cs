using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
	public Color player;
	public Color enemy;

	public RectTransform rectTransform;
	public TextMeshProUGUI levelText;
    //public TMPro.TextMeshProUGUI levelText;
	public Slider slider;
	public Image fillImage;
	public RectTransform sliderRectTransform;
    

	Quaternion HealthBarRotation;
	Vector2 size;

	int maxHealth;

	public void SetHealthBar(int level, int maxHealth, int startingHealth, int teamId, float healthBarY, bool typeBonus, UnitType type)
	{
		levelText.text = level.ToString();

        //float height = 5f;

        //if(type == UnitType.Building)
        //{
        //	height = 7f;
        //}
        levelText.fontMaterial.renderQueue = 4000;


        if(type == UnitType.RaidBoss)
        {
            rectTransform.localScale = rectTransform.localScale * 2;
        }
        else if (type == UnitType.RaidMinion)
        {
            rectTransform.localScale = rectTransform.localScale * 1.3f;
        }

        size = new Vector2((Mathf.Min(startingHealth, 1000f) / 1000f) * 30f + 4, sliderRectTransform.rect.height);
		sliderRectTransform.sizeDelta = size;

		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, healthBarY);

		if(type == UnitType.Building)
		{
			slider.gameObject.SetActive(false);
		}

		if(teamId == 0)
		{
			fillImage.sprite = Resources.Load<Sprite>("UI/HealthBarPlayer");
			//levelText.color = player;
		}
		else
		{
			fillImage.sprite = Resources.Load<Sprite>("UI/HealthBarEnemy");
			//levelText.color = enemy;
		}

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
		this.maxHealth = maxHealth;
    }

    public void UpdateHealth(int currentHealth)
    {
		if(slider.value == maxHealth)
		{
			slider.gameObject.SetActive(true);
		}

        slider.value = currentHealth;
    }
}
