using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overrun : MonoBehaviour
{
	public Text stateText;
	public Text timerText;

	float gameLength;

	private void Update()
	{
		if (!Battle.instance.setup)
			return;

		if (Battle.instance.battleType != BattleType.LivePvP)
		{
			gameLength = Battle.state.gameLength - Battle.state.gameTimer;

			if (/*gameLength > 60f || */gameLength <= 1f)
			{
				timerText.text = "";
			}
			else
			{
				SetTimerText();
			}


			CountdownTimer();

			if (gameLength <= 1f)
			{
				timerText.color = Color.red;
				timerText.text = "Reinforcements Incoming";
			}
		}
		else
		{
			timerText.text = "";
		}
	}

	void SetTimerText()
	{
		int minutes = Mathf.RoundToInt(Mathf.Floor(gameLength / 60));
		int seconds = Mathf.RoundToInt(Mathf.Floor(gameLength % 60));


		if(minutes == 0)
		{
			timerText.text = ":" + seconds;
		}
		else if(seconds >= 10)
		{
			timerText.text = minutes + ":" + seconds;
		}
		else
		{
			timerText.text = minutes + ":0" + seconds;
		}
	}

	void CountdownTimer()
	{
		stateText.text = "";

		if (gameLength <= 60f && gameLength > 57f)
		{
			stateText.text = "Reinforcements in 60 Seconds";
		}

		if (gameLength <= 30f && gameLength > 27f)
		{
			stateText.text = "Reinforcements in 30 Seconds";
		}

		if (gameLength <= 10f && gameLength > 7f)
		{
			stateText.text = "Reinforcements in 10 Seconds";
		}
	}
}
