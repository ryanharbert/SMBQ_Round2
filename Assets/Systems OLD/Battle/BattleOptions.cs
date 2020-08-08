using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleOptions : MonoBehaviour
{
	public GameObject settingsButton;
	public GameObject rightSideOptions;

	public GameObject autoPlayOn;
	public GameObject autoPlayOff;

	public Image autoImage;
	public Image gameSpeedImage;
	public Text autoText;
	public Text gameSpeedText;

	public GameObject pauseIcon;
	public GameObject playIcon;

	bool paused = false;

	float currentTimeScale;

	private void OnEnable()
	{
		autoPlayOn.gameObject.SetActive(false);
		autoPlayOff.gameObject.SetActive(true);

		gameSpeedText.text = "x1";

		if(Data.instance != null && Data.instance.currency.playerLevel < 3)
		{
			autoImage.color = new Color(autoImage.color.r, autoImage.color.g, autoImage.color.b, 0.5f);
			gameSpeedImage.color = new Color(gameSpeedImage.color.r, gameSpeedImage.color.g, gameSpeedImage.color.b, 0.5f);
			autoText.color = new Color(autoText.color.r, autoText.color.g, autoText.color.b, 0.5f);
			gameSpeedText.color = new Color(gameSpeedText.color.r, gameSpeedText.color.g, gameSpeedText.color.b, 0.5f);
		}
	}

	public void GameSpeed()
	{
		if (Data.instance.currency.playerLevel < 3)
		{
			Warning.instance.Activate("Reach Player Level 3 to Unlock Game Speed Increase");
		}
		else if (Time.timeScale == 1)
		{
			Time.timeScale = 2;
			gameSpeedText.text = "x2";
		}
		else if (Time.timeScale == 2)
		{
			Time.timeScale = 3;
			gameSpeedText.text = "x3";
		}
		else
		{
			Time.timeScale = 1;
			gameSpeedText.text = "x1";
		}
	}

	public void AutoOn()
	{
		autoPlayOn.SetActive(false);
		autoPlayOff.SetActive(true);
		Battle.state.autoPlay = false;
	}

	public void AutoOff()
	{
		if(Data.instance.currency.playerLevel >= 3)
		{
			autoPlayOn.SetActive(true);
			autoPlayOff.SetActive(false);
			Battle.state.autoPlay = true;
		}
		else
		{
			Warning.instance.Activate("Reach Player Level 3 to Unlock Auto Play");
		}
	}

	public void Pause()
	{
		if (paused)
		{
			settingsButton.SetActive(true);
			rightSideOptions.SetActive(true);
			pauseIcon.SetActive(true);
			playIcon.SetActive(false);
			paused = false;
			Warning.instance.warningText.enabled = false;
			Warning.instance.warningText.text = "";
			Time.timeScale = currentTimeScale;
		}
		else
		{
			settingsButton.SetActive(false);
			rightSideOptions.SetActive(false);
			pauseIcon.SetActive(false);
			playIcon.SetActive(true);
			paused = true;
			Warning.instance.warningText.enabled = true;
			Warning.instance.warningText.text = "Paused";
			currentTimeScale = Time.timeScale;
			Time.timeScale = 0;
		}
	}

	public void Concede()
	{
		for(int i = 0; i < Battle.state.playerObjectives.Count; i++)
		{
			Battle.state.playerObjectives[i].health = 0;
		}
	}
}
