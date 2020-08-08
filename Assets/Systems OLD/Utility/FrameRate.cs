using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    public static FrameRate instance;

	public int avgFrameRate = 0;

	float avgTimer = 0;
	float eventTimer = 0;
    List<int> frameRates = new List<int>();
	int avgCount = 0;
	long avgTotal = 0;


    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(transform.gameObject);
    }

    private void Update()
    {
		avgTimer += Time.deltaTime;
		eventTimer += Time.deltaTime;
		frameRates.Add(Mathf.RoundToInt(1 / Time.deltaTime));
        if (avgTimer > 10f)
        {
            float frameRatesTotal = 0;
            foreach (int i in frameRates)
            {
				frameRatesTotal += i;
            }
            avgTotal += Mathf.RoundToInt(frameRatesTotal / frameRates.Count);
			avgCount++;
			avgFrameRate = Mathf.RoundToInt(avgTotal / avgCount);
			frameRates = new List<int>();
			avgTimer = 0;
		}
		if (eventTimer > 60f)
		{
			Dictionary<string, object> bodyDict = new Dictionary<string, object>() { { "Average", avgFrameRate } };
			PlayFab.ClientModels.WriteClientPlayerEventRequest request = new PlayFab.ClientModels.WriteClientPlayerEventRequest() { EventName = "FrameRate", Body = bodyDict };
			PlayFab.PlayFabClientAPI.WritePlayerEvent(request, Response, Error);
			eventTimer = 0;
		}
	}

    public void LossBattle()
	{
		Dictionary<string, object> bodyDict = new Dictionary<string, object>() { { "Average", avgFrameRate }, { "Enemy", Data.instance.battle.enemyName } };
		PlayFab.ClientModels.WriteClientPlayerEventRequest request = new PlayFab.ClientModels.WriteClientPlayerEventRequest() { EventName = "LossBattle", Body = bodyDict };
        PlayFab.PlayFabClientAPI.WritePlayerEvent(request, Response, Error);
    }

    void Response(PlayFab.ClientModels.WriteEventResponse response)
    {

    }

    void Error(PlayFab.PlayFabError error)
    {

    }
}
