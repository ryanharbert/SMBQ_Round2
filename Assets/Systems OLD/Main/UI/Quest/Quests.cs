using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class Quests : MonoBehaviour
{
    [SerializeField] private Text newQuestTimerText;
    [SerializeField] private Text questAmountText;
    [SerializeField] private GameObject questsFullObject;

    [SerializeField] private QuestDisplay[] quests;

    [SerializeField] private GameObject loading;
    [SerializeField] private GameObject questDisplayObject;
    [SerializeField] private GameObject newQuestsTomorrowObject;

    bool gettingNewQuests;

    public void CollectRewards(QuestData data, int index)
    {
        OfferConfirmation.instance.QuestReward(data.Reward);
        Data.instance.quest.CollectQuestReward(index, RewardsReceived);
    }

    void RewardsReceived()
    {
        OfferConfirmation.instance.QuestRewardReceived();
        SetQuests();

        NavBar.instance.SetQuestNotification();
    }

    private void Start()
    {
        gettingNewQuests = false;
        SetQuests();
        CheckNewQuests();
    }

    void Update ()
    {
        if (gettingNewQuests)
            return;

        CheckNewQuests();
    }

    void CheckNewQuests()
    {
        TimeSpan t;
        if (Data.instance.quest.TimeForNewQuests(out t))
        {
            newQuestTimerText.text = "New Quests in: " + TimeSpanDisplay.Format(t);
        }
        else
        {
            newQuestTimerText.text = "Getting new quests...";
            GetNewQuests();
        }
    }

    void SetQuests()
    {
        SetQuests(new List<int>());
    }

    void SetQuests(List<int> newQuests)
    {
        List<QuestData> questList = Data.instance.quest.quests;

        loading.SetActive(false);
        questAmountText.text = questList.Count + " / 15";
        if (14 < questList.Count)
        {
            questsFullObject.SetActive(true);
            newQuestsTomorrowObject.SetActive(false);
        }
        else if(questList.Count == 0)
        {
            questsFullObject.SetActive(false);
            newQuestsTomorrowObject.SetActive(true);
        }
        else
        {
            questsFullObject.SetActive(false);
            newQuestsTomorrowObject.SetActive(false);
        }
        for (int i = 0; i < quests.Length; i++)
        {
            if (newQuests.Contains(i))
            {
                quests[i].NewTag(true);
            }
            else
            {
                quests[i].NewTag(false);
            }
        }
        for (int i = 0; i < quests.Length; i++)
        {
            if(i < questList.Count)
            {
                quests[i].gameObject.SetActive(true);
                quests[i].Set(questList[i], i, this);
            }
            else
            {
                quests[i].gameObject.SetActive(false);
            }
        }
    }

    void GetNewQuests()
    {
        gettingNewQuests = true;
        loading.SetActive(true);
        newQuestsTomorrowObject.SetActive(false);
        questDisplayObject.SetActive(false);
        Data.instance.quest.GetNewQuests(NewQuestsReceived);
    }

    void NewQuestsReceived(List<int> newQuests)
    {
        SetQuests(newQuests);
        gettingNewQuests = false;
        loading.SetActive(false);
        questDisplayObject.SetActive(true);            
        NavBar.instance.SetQuestNotification();
    }
}
