using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SceneLoader : MonoBehaviour
{
	public static SceneLoader thisScript;
	public static bool loadingInProgress = false;
	public static Scene activeScene;
	public static AsyncOperation sceneLoad;
	public static string sceneToLoad;

	public float fadeSpeed = 8f;
	AsyncOperation sceneUnload;
	LoadingScene loadingScene;
	
	bool additionalSceneLoadingInProgress = false;
    BattleData battleData;

	BattleType battleType = BattleType.World;

	private void Awake()
	{
		DontDestroyOnLoad(transform.gameObject);
		thisScript = this;		
	}

    public static void ChangeScenes(string sceneName)
	{
        thisScript.battleType = BattleType.World;

		Scene checkForLoadScene = SceneManager.GetSceneByName("Loading");
        if (checkForLoadScene.IsValid())
        {
            return;
        }

		loadingInProgress = true;
		sceneToLoad = sceneName;
		thisScript.StartCoroutine("SceneLoading");		
	}

    static void EnterBattle(BattleData battleData, BattleType type, string battleScene)
    {
        thisScript.battleData = battleData;
        thisScript.battleType = type;

        Scene checkForLoadScene = SceneManager.GetSceneByName("Loading");
        if (checkForLoadScene.IsValid())
        {
            return;
        }

        loadingInProgress = true;
        sceneToLoad = "BattleTest";
        thisScript.StartCoroutine("SceneLoading");
    }

    public static void LiveBattle(BattleData battleData, BattleType type, string battleScene)
    {
        EnterBattle(battleData, type, battleScene);
    }

    public static void OfflineBattle(BattleData battleData, BattleType type, string battleScene)
    {
        EnterBattle(battleData, type, battleScene);
    }

	IEnumerator SceneLoading()
	{
        activeScene = SceneManager.GetActiveScene();
        sceneLoad = SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);

        while (!sceneLoad.isDone)
		{
			yield return null;
		}

		loadingScene = (LoadingScene)FindObjectOfType(typeof(LoadingScene));

        SetLoadingSceneImage(sceneToLoad);

        yield return StartCoroutine (FadeIn());

		sceneUnload = SceneManager.UnloadSceneAsync(activeScene);
		sceneLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
		sceneLoad.completed += Action;

		while(loadingInProgress == true || !sceneUnload.isDone)
		{
			yield return null;
		}

		Resources.UnloadUnusedAssets();
		System.GC.Collect();

		if(sceneToLoad == "WorldMap")
		{
			additionalSceneLoadingInProgress = true;

			AsyncOperation additionalSceneLoad = SceneManager.LoadSceneAsync(Data.instance.world.Island, LoadSceneMode.Additive);
			additionalSceneLoad.completed += AdditionalSceneLoadingFinished;
			while (additionalSceneLoadingInProgress == true || !additionalSceneLoad.isDone)
			{
				yield return null;
			}
			SceneManager.MergeScenes(SceneManager.GetSceneByName(sceneToLoad), SceneManager.GetSceneByName(Data.instance.world.Island));
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(Data.instance.world.Island));
			WorldManager worldManager = FindObjectOfType(typeof(WorldManager)) as WorldManager;
			worldManager.Setup();
		}
		else if(sceneToLoad == "BattleTest")
		{
			additionalSceneLoadingInProgress = true;
			string battleScene;
			Battle.instance.battleType = battleType;
			if (battleType == BattleType.World)
			{
				battleScene = Data.instance.battle.battleScene;
                if(Data.instance.battle.enemyName == "Tutorial")
                {
                    SceneManager.LoadSceneAsync("TutorialBattle", LoadSceneMode.Additive);
                }
			}
			else if(battleType == BattleType.Raid)
			{
				battleScene = Data.instance.raidBattle.battleScene;
			}
			else if (battleType == BattleType.LiveRaid)
			{
				battleScene = Data.instance.raidBattle.battleScene;
			}
			else if(battleType == BattleType.LivePvP)
            {
				battleScene = "Mountain";
                Data.instance.battle.battleScene = "Mountain";
            }
            else
            {
				battleScene = RandomScene();
                Data.instance.pvpBattle.battleScene = battleScene;
            }
			AsyncOperation additionalSceneLoad = SceneManager.LoadSceneAsync(battleScene, LoadSceneMode.Additive);
            additionalSceneLoad.completed += AdditionalSceneLoadingFinished;
			while(additionalSceneLoadingInProgress == true || !additionalSceneLoad.isDone)
			{
				yield return null;
			}
            if(battleType == BattleType.LivePvP || battleType == BattleType.LiveRaid)
            {
                while (Battle.instance == null)
                {
                    yield return null;
                }
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
                hash.Add("BattleOpened", true);
                Photon.Pun.PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
                while (!Photon.Pun.PhotonNetwork.PlayerListOthers[0].CustomProperties.ContainsKey("BattleOpened"))
                {
                    yield return null;
                }
            }
            SceneManager.MergeScenes(SceneManager.GetSceneByName(sceneToLoad), SceneManager.GetSceneByName(battleScene));
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(battleScene));
            Battle battle = FindObjectOfType(typeof(Battle)) as Battle;
			battle.Setup(thisScript.battleData);
            while(!Battle.instance.setup)
            {
                yield return null;
            }
		}

		yield return new WaitForFixedUpdate();

		yield return StartCoroutine (FadeOut());

		sceneUnload = SceneManager.UnloadSceneAsync("Loading");
	}

	public void Action(AsyncOperation async)
	{
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneToLoad));
		loadingInProgress = false;
	}

	public void AdditionalSceneLoadingFinished(AsyncOperation async)
	{
		additionalSceneLoadingInProgress = false;
	}

    void SetLoadingSceneImage(string sceneToLoad)
    {
        if (sceneToLoad == "BattleTest")
        {
			if (battleType == BattleType.World)
			{
				loadingScene.image.sprite = Resources.Load<Sprite>("Loading/" + Data.instance.battle.battleScene);
			}
			else if (battleType == BattleType.Raid || battleType == BattleType.LiveRaid)
			{
				loadingScene.image.sprite = Resources.Load<Sprite>("Loading/" + Data.instance.raidBattle.battleScene);
            }
            else if (battleType == BattleType.AsyncPvP)
            {
                loadingScene.image.sprite = Resources.Load<Sprite>("Loading/" + Data.instance.pvpBattle.battleScene);
            }
            else
			{
				loadingScene.image.sprite = Resources.Load<Sprite>("Loading/Forest");
			}
		}
		//else if(sceneToLoad == "WorldMap")
		//{
		//	loadingScene.image.sprite = Resources.Load<Sprite>("Loading/BigRockIsle");
		//}
		else
		{
			loadingScene.image.sprite = Resources.Load<Sprite>("Loading/" + sceneToLoad);
		}

		if (loadingScene.image.sprite == null)
        {
            loadingScene.image.sprite = Resources.Load<Sprite>("Loading/WorldMap");
        }
    }

	IEnumerator FadeIn()
	{
		loadingScene.loadingObject.SetActive(true);
		loadingScene.image.color = Color.clear;
		while(loadingScene.image.color != Color.white)
		{
			loadingScene.image.color = Color.Lerp (loadingScene.image.color, Color.white, fadeSpeed * Time.deltaTime);
			yield return null;
		}
	}

	IEnumerator FadeOut()
	{
		loadingScene.loadingObject.SetActive(false);
		loadingScene.image.color = Color.white;
		while(loadingScene.image.color != Color.clear)
		{
			loadingScene.image.color = Color.Lerp (loadingScene.image.color, Color.clear, fadeSpeed * Time.deltaTime);
			yield return null;
		}
		
	}

    string RandomScene()
    {
        int i = Random.Range(0,4);
        string scene = "";

        switch (i)
        {
            case 0:
                scene = "Forest";
                break;
            case 1:
                scene = "Beach";
                break;
            case 2:
                scene = "Graveyard";
                break;
            case 3:
                scene = "Desert";
                break;
            case 4:
                scene = "Mountain";
                break;
        }

        return scene;
    }
}
