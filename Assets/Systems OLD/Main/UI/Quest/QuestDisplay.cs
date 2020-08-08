using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class QuestDisplay : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text rewardAmountText;
    [SerializeField] private Text questProgressText;
    [SerializeField] private GameObject collectButton;
    [SerializeField] private GameObject progressObject;
    [SerializeField] private GameObject newQuestObject;
    [SerializeField] private int index;

    QuestData data;
    Quests quests;

    public void Set(QuestData data)
    {
        this.data = data;

        if (data.Type == "WorldTarget")
        {
            CardData c = Resources.Load<CardData>("Cards/" + data.Target);
            image.sprite = c.cardDisplay;
            descriptionText.text = "Defeat " + data.Complete + " " + c.displayName;
        }
        else if (data.Type == "WorldAny")
        {
            image.sprite = Resources.Load<Sprite>("QuestIcons/QuestWorldAny");
            descriptionText.text = "Defeat " + data.Complete + " enemies";
        }
        else if (data.Type == "RaidAny")
        {
            image.sprite = Resources.Load<Sprite>("QuestIcons/RaidAny");
            descriptionText.text = "Defeat " + data.Complete + " raids";
        }
        else if (data.Type == "AsyncPvP")
        {
            image.sprite = Resources.Load<Sprite>("QuestIcons/AsyncPvP");
            descriptionText.text = "Win " + data.Complete + " Arena battles";
        }

        rewardAmountText.text = "x" + data.Reward;

        if (data.Progress < data.Complete)
        {
            collectButton.SetActive(false);
            progressObject.SetActive(true);
            questProgressText.text = data.Progress + " / " + data.Complete;
        }
        else
        {
            collectButton.SetActive(true);
            progressObject.SetActive(false);
            transform.SetAsFirstSibling();
        }
    }

    public void Set(QuestData data, int index, Quests quests)
    {
        this.index = index;
        this.quests = quests;

        Set(data);
    }

    public void NewTag(bool isNew)
    {
        if(isNew)
        {
            gameObject.transform.SetAsFirstSibling();
            newQuestObject.SetActive(true);
        }
        else
        {
            newQuestObject.SetActive(false);
        }
    }

    public virtual void CollectRewards()
    {
        collectButton.SetActive(false);
        quests.CollectRewards(data, index);
    }
}
