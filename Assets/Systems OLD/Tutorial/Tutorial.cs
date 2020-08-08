using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public WorldManager worldManager;

    public GameObject wormHero;
    public GameObject medusaHero;
    public GameObject mageHero;
    public GameObject enemySkeleton;

    public GameObject moveClick1;
    public GameObject moveClick2;
    public GameObject moveClick3;

    public GameObject boat;
    public GameObject[] fires;

    public RectTransform clickToMove;
    public TextMeshProUGUI instructionText;

    public Animator cutScene;

    public Camera animationCam;
    public Camera followCam;

    bool mute = false;
    int moveClickPos = 0;

    private void Start()
    {
        if (!Data.instance.tutorial.steps["BattleComplete"])
        {
            CutSceneStart();
        }
        else
        {
            AfterBattleStart();
        }
    }

    private void CutSceneStart()
    {
        switch (Data.instance.collection.deck.heroes[0])
        {
            case "QueenWorm":
                medusaHero.gameObject.SetActive(false);
                mageHero.gameObject.SetActive(false);
                break;
            case "Medusa":
                wormHero.gameObject.SetActive(false);
                mageHero.gameObject.SetActive(false);
                break;
            case "MightyMage":
                wormHero.gameObject.SetActive(false);
                medusaHero.gameObject.SetActive(false);
                break;
        }

        clickToMove.gameObject.SetActive(false);

        string sfx = PlayerPrefs.GetString("SFX");
        if (sfx == "off")
        {
            mute = true;
        }
        else
        {
            mute = false;
            PlayerPrefs.SetString("Music", "off");
            AudioManager.instance.Setup();
        }
    }

    private void AfterBattleStart()
    {
        wormHero.SetActive(false);
        medusaHero.SetActive(false);
        mageHero.SetActive(false);
        enemySkeleton.SetActive(false);

        cutScene.enabled = false;

        foreach (GameObject fire in fires)
        {
            fire.SetActive(true);
        }

        clickToMove.gameObject.SetActive(true);
        moveClickPos = 3;
        instructionText.text = "get on boat";

        animationCam.gameObject.SetActive(false);
        followCam.gameObject.SetActive(true);

        boat.SetActive(true);

        worldManager.Setup();
    }


    public void CutSceneOver()
    {
        wormHero.SetActive(false);
        medusaHero.SetActive(false);
        mageHero.SetActive(false);
        enemySkeleton.SetActive(false);

        clickToMove.gameObject.SetActive(true);
        moveClickPos = 1;

        animationCam.gameObject.SetActive(false);
        followCam.gameObject.SetActive(true);

        
        if (!mute)
        {
            PlayerPrefs.SetString("Music", "on");
            AudioManager.instance.Setup();
        }

#if UNITY_EDITOR

#elif UNITY_WEBGL
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "kongInitialized", FunctionParameter = new { kongId = KongregateAPIBehaviour.instance.kongregateId } }, InitializedSuccess, TestFailure);
        Data.instance.initialized = true;

#else

#endif

        worldManager.Setup();
    }

    private void Update()
    {
        MoveNodeArrow();
    }

    void MoveNodeArrow()
    {
        if (moveClickPos == 1)
        {
            if(!animationCam.gameObject.activeSelf && Input.GetMouseButtonDown(0))
            {
                moveClickPos++;
                instructionText.text = "Move this way";
            }
            clickToMove.position = Camera.main.WorldToScreenPoint(moveClick1.transform.position);
        }
        else if (moveClickPos == 2)
        {
            //if(worldManager.disableWorld)
            //{
            //    moveClickPos = 0;
            //}
            clickToMove.position = Camera.main.WorldToScreenPoint(moveClick2.transform.position);
        }
        else if (moveClickPos == 3)
        {
            clickToMove.position = Camera.main.WorldToScreenPoint(moveClick3.transform.position);
        }
        else if (moveClickPos == 0 && clickToMove.gameObject.activeSelf)
        {
            clickToMove.gameObject.SetActive(false);
        }
    }

    private void InitializedSuccess(ExecuteCloudScriptResult result)
    {
        Debug.Log(result.FunctionResult);
    }

    private void TestFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
}
