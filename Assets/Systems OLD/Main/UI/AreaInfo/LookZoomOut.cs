using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookZoomOut : MonoBehaviour {

    public static LookZoomOut instance;

    public Text islandName;
    public Text zoneName;
    public Text possibleLevels;
    public CardDisplay[] possibleEnemies;

    public GameObject mainCanvas;
    public GameObject zoomOutCanvas;

	float normalCamSize;
	float zoomOutSize;

	public int camZoomedOut = 0;

    private void Awake()
    {
        instance = this;
    }

    public void Setup()
    {
        //camZoomedOut = PlayerPrefs.GetInt("Zoom");
        //if (camZoomedOut == 1)
        //{
        //    WorldManager.instance.defaultCameraSize = WorldManager.instance.defaultCameraSize * 1.5f;
        //}
        //else if (camZoomedOut == 2)
        //{
        //    WorldManager.instance.defaultCameraSize = WorldManager.instance.defaultCameraSize * 1.5f;
        //    WorldManager.instance.defaultCameraSize = WorldManager.instance.defaultCameraSize * 4 / 3f;
        //}
    }

    public void Zoom()
	{
		//if(camZoomedOut == 0)
		//{
		//	WorldManager.instance.defaultCameraSize = WorldManager.instance.defaultCameraSize * 1.5f;
		//	camZoomedOut = 1;
  //          PlayerPrefs.SetInt("Zoom", 1);
		//}
		//else if (camZoomedOut == 1)
		//{
		//	WorldManager.instance.defaultCameraSize = WorldManager.instance.defaultCameraSize * 4/3f;
		//	camZoomedOut = 2;
  //          PlayerPrefs.SetInt("Zoom", 2);
  //      }
		//else if (camZoomedOut == 2)
		//{
		//	WorldManager.instance.defaultCameraSize = WorldManager.instance.defaultCameraSize / 2;
		//	camZoomedOut = 0;
  //          PlayerPrefs.SetInt("Zoom", 0);
  //      }
	}

	public void LookButton()
	{
		WorldManager.instance.gameObject.SetActive(false);
		mainCanvas.SetActive(false);
		StartCoroutine("ZoomOutCam");

	}

	IEnumerator ZoomOutCam()
	{
		yield return null;
		//normalCamSize = Camera.main.orthographicSize;
		//zoomOutSize = Camera.main.orthographicSize * 2;

		//while (Camera.main.orthographicSize < zoomOutSize)
		//{
		//	Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, zoomOutSize, Time.smoothDeltaTime * 20);
		//	yield return null;
		//}
		zoomOutCanvas.SetActive(true);
        islandName.text = Data.instance.world.GetCurrentIslandName();
        ZoneData zone = Data.instance.world.GetCurrentZone();
        zoneName.text = zone.Name;
        string possibleLevelsString = "Possible Levels: " + zone.PossibleLevels[0];
        for(int i = 1; i < zone.PossibleLevels.Length; i++)
        {
            possibleLevelsString += ", " + zone.PossibleLevels[i];
        }
        possibleLevels.text = possibleLevelsString;

        int h = 0;
        for(int i = 0; i < possibleEnemies.Length; i++)
        {
            if (h < zone.PossibleEnemies.Count)
            {
                bool validEnemy = false;

                while (validEnemy == false && h < zone.PossibleEnemies.Count)
                {
                    validEnemy = true;
                    for (int j = 0; j < i; j++)
                    {
                        if (possibleEnemies[j].cardData.itemID == zone.PossibleEnemies[h])
                        {
                            h++;
                            validEnemy = false;
                            break;
                        }
                    }
                }

                if(validEnemy)
                {
                    possibleEnemies[i].gameObject.SetActive(true);
                    possibleEnemies[i].SetCardDisplay(zone.PossibleEnemies[h]);
                }
                else
                {
                    possibleEnemies[i].gameObject.SetActive(false);
                }
            }
            else
            {
                possibleEnemies[i].gameObject.SetActive(false);
            }
            h++;
        }
	}

	public void BackButton()
	{
		zoomOutCanvas.SetActive(false);
		StartCoroutine("ZoomInCam");
	}

	IEnumerator ZoomInCam()
	{
		yield return null;
		//while (Camera.main.orthographicSize > normalCamSize)
		//{
		//	Camera.main.orthographicSize = Mathf.MoveTowards(Camera.main.orthographicSize, normalCamSize, Time.smoothDeltaTime * 20);
		//	yield return null;
		//}
		mainCanvas.SetActive(true);
		WorldManager.instance.gameObject.SetActive(true);
	}
}
