using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BattleCard : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public int index;
    public CardDisplay cardDisplay;
	public CardData cardData;

    protected Animator anim;
	protected Image slotImage;
	protected RectTransform rectTransform;
	protected float selectionYAdjustment;
	protected Vector2 selectionAdjustment;

	public static bool touchingCard = false;

	protected void Awake()
    {
        anim = GetComponent<Animator>();
        rectTransform = GetComponent<RectTransform>();
        selectionAdjustment = rectTransform.anchoredPosition;
        selectionYAdjustment = rectTransform.rect.height / 5;
        slotImage = cardDisplay.cardImage;
    }

    protected virtual void Update()
    {
        HotKey();
        NotEnoughMana();
    }

    protected void NotEnoughMana()
    {
		if(Battle.instance.setup == false)
			return;

        if (Battle.state.currentMana < cardData.ManaCost)
        {
            slotImage.color = Color.grey;
        }
        else
        {
            slotImage.color = Color.white;
        }
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (!touchingCard)
        {
            BattleCardGroup.SelectCard(this);
			touchingCard = true;
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		touchingCard = false;
	}

    public void HotKey()
    {
        string input = "";

        switch (index)
        {
            case 0:
                input = "q";
                break;
            case 1:
                input = "w";
                break;
            case 2:
                input = "e";
                break;
            case 4:
                input = "r";
                break;
            case 5:
                input = "d";
                break;
            case 6:
                input = "f";
                break;
            default:
                //Debug.Log("Type Bonus is not supported");
                break;
        }

        if (!touchingCard && input != "" && Input.GetKeyDown(input))
        {
            BattleCardGroup.SelectCard(this);
        }
    }

	public virtual void Toggle(bool selected)
    {
        if(selected)
        {
            selectionAdjustment.y += selectionYAdjustment;
            rectTransform.anchoredPosition = selectionAdjustment;
            Battle.state.selectedCard = this;
            anim.SetTrigger("Selected");
			SetMesh(true);
            Battle.instance.hoverOverRect.anchoredPosition = rectTransform.anchoredPosition + new Vector2(0f, 160f);
        }
        else
        {
            selectionAdjustment.y -= selectionYAdjustment;
            rectTransform.anchoredPosition = selectionAdjustment;
            anim.SetTrigger("NotSelected");
            if (Battle.state.selectedCard == this)
            {
				Battle.state.selectedCard = null;
                SetMesh(false);
            }
        }
    }

	protected virtual void SetMesh(bool enabled)
	{
		if(enabled)
		{
			if(cardDisplay.cardData.validPlayArea == ValidPlayArea.YourSide)
			{
				if (Battle.state.teamID == 0)
				{
                    Battle.state.playerSpawnArea.display.SetActive(true);
				}
				else
                {
                    Battle.state.enemySpawnArea.display.SetActive(true);
                }
			}
			else
            {
                Battle.state.anywhereSpawnArea.display.SetActive(true);
            }
		}
		else
        {
            if (cardDisplay.cardData.validPlayArea == ValidPlayArea.YourSide)
            {
                if (Battle.state.teamID == 0)
                {
                    Battle.state.playerSpawnArea.display.SetActive(false);
                }
                else
                {
                    Battle.state.enemySpawnArea.display.SetActive(false);
                }
            }
            else
            {
                Battle.state.anywhereSpawnArea.display.SetActive(false);
            }
        }
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this is BattleHero)
            return;

        Battle.instance.hoverOverRect.gameObject.SetActive(true);
        Battle.instance.hoverOverRect.anchoredPosition = rectTransform.anchoredPosition + new Vector2(0f, 150f);
        HoverOverText();
    }

    protected virtual void HoverOverText()
    {
        Battle.instance.hoverOverText.text = cardData.shortDesc;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Battle.instance.hoverOverRect.gameObject.SetActive(false);
    }
}
