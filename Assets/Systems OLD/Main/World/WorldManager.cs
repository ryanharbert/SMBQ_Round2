using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    public MainUIManager uiManager;

    [Header("Basic World")]
    public WorldPlayer player;
    public List<WorldEnemy> enemies = new List<WorldEnemy>();
    public Dictionary<string, Node> nodes = new Dictionary<string, Node>();

    [Header("Node Interactions")]
    public List<InteractiveNode> interactiveNodes = new List<InteractiveNode>();
    public InteractiveNode currentInteractiveNode;
    
    [Header("Move Particle")]
    [SerializeField] private GameObject moveParticle;
    private GameObject spawnedMoveParticle;

    public bool receivedBattleData { get; private set; }

    bool setup = false;
    bool disableWorld = false;
    WorldCamera cameraManager;

    public void Setup()
    {
        instance = this;

        //FIND NODES
        Node[] foundNodes = FindObjectsOfType<Node>();

		foreach(Node n in foundNodes)
		{
			nodes.Add(n.name, n);
        }

        //HEROES
        WorldPlayer playerObject = Resources.Load<WorldPlayer>("Heroes/" + Data.instance.collection.deck.heroes[0]) as WorldPlayer;
        Node playerNode = nodes[Data.instance.world.CurrentPlayerNode];
        player = Instantiate(playerObject, playerNode.transform.position, Quaternion.identity);
        player.Setup();

        //CAMERA
        cameraManager = FindObjectOfType(typeof(WorldCamera)) as WorldCamera;
        cameraManager.Setup();

        //ENEMIES
        foreach (KeyValuePair<string, EnemyData> e in Data.instance.world.Enemies)
        {
            if (nodes.ContainsKey(e.Key))
            {
                SpawnEnemy(e);
            }
        }

        //FIND INTERACTIVE NODES
        InteractiveNode[] foundInteractiveNodes = FindObjectsOfType<InteractiveNode>();

        for(int i = 0; i < foundInteractiveNodes.Length; i++)
        {
            if(foundInteractiveNodes[i].Setup())
            {
                interactiveNodes.Add(foundInteractiveNodes[i]);
            }
        }

        receivedBattleData = false;
		setup = true;
	}

    public void SwitchHero(string heroName)
    {
        Destroy(player.gameObject);

        WorldPlayer playerObject = Resources.Load("Heroes/" + heroName) as WorldPlayer;
        Node playerNode = nodes[Data.instance.world.CurrentPlayerNode];
        player = Instantiate(playerObject, playerNode.transform.position, Quaternion.identity);
        player.Setup();
    }

    public void ToggleWorld(bool on)
    {
        if (NavBar.instance != null)
        {
            NavBar.instance.ToggleNavBar(on);
        }
        disableWorld = !on;
        cameraManager.ToggleZoom(!on);
    }

	void SpawnEnemy(KeyValuePair<string, EnemyData> e)
	{
        WorldEnemy enemy = Resources.Load<WorldEnemy>("Enemies/" + e.Value.Enemy) as WorldEnemy;
		EnemyNode enemyNode = (EnemyNode)nodes[e.Key];
        WorldEnemy newEnemy = Instantiate(enemy, enemyNode.transform.position, enemy.transform.rotation);
        newEnemy.Setup(enemyNode, e.Value.Enemy, e.Value.Level);        
        enemies.Add(newEnemy);
	}

	private void Update()
	{
		if(!setup)
			return;

        cameraManager.FollowPlayer();

        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;

        if (disableWorld)
            return;

        MoveHero();
        NodeInteractions();
        UpdateEnemies();
    }

    public void MoveHero()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray camRay = cameraManager.mainCamera.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(camRay, out hit, 500f))
            {
                player.Move(hit.point);

                if (spawnedMoveParticle == null)
                {
                    spawnedMoveParticle = Instantiate(moveParticle);
                }
                spawnedMoveParticle.SetActive(false);
                spawnedMoveParticle.SetActive(true);
                spawnedMoveParticle.transform.position = hit.point;
                spawnedMoveParticle.transform.rotation = Quaternion.LookRotation(hit.normal);
            }
        }
    }

    void NodeInteractions()
    {
        for(int i = 0; i < interactiveNodes.Count; i++)
        {
            if (Vector3.Distance(player.transform.position, interactiveNodes[i].transform.position) < 1.5f)
            {
                if (currentInteractiveNode == null)
                {
                    currentInteractiveNode = interactiveNodes[i];
                    interactiveNodes[i].EnterRange();
                }
                return;
            }
        }
        if(currentInteractiveNode != null)
        {
            currentInteractiveNode.ExitRange();
            currentInteractiveNode = null;
        }
    }

    void UpdateEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i].Aggroed(this))
            {
                BattleStarting(enemies[i]);
                break;
            }
            else
            {
                enemies[i].Roam(this);
            }
        }
    }

    void BattleStarting(WorldEnemy enemy)
    {
        enemy.Engage(player);

        player.Stop();

        Data.instance.battle.battleScene = enemy.node.battleScene;
        Data.instance.battle.level = enemy.level;
        Data.instance.battle.enemyName = enemy.name;
        Data.instance.battle.nodeLocation = enemy.node.name;

        ToggleWorld(false);
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "getBattleData", FunctionParameter = new { enemy = enemy.name } }, SetBattleData, ServerFailure);
    }

    void SetBattleData(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            JsonObject jsonResult = (JsonObject)result.FunctionResult;
            object objectivesObject;
            object deckObject;
            jsonResult.TryGetValue("objectives", out objectivesObject);
            jsonResult.TryGetValue("deck", out deckObject);

            string[] objectivesList = PlayFabSimpleJson.DeserializeObject<string[]>((string)objectivesObject);
            string[] deckList = PlayFabSimpleJson.DeserializeObject<string[]>((string)deckObject);

            Data.instance.battle.enemyObjectives = objectivesList;
            Data.instance.battle.enemyDeck = deckList;

            receivedBattleData = true;
        }
        else
        {
            Debug.LogError("Battle does not exist for this enemy.");
        }
    }


	public void EnterBattle()
    {
		SceneLoader.ChangeScenes("BattleTest");
	}

	public void ServerFailure(PlayFabError error)
	{
		Debug.LogError("Here's some debug information:");
		Debug.LogError(error.GenerateErrorReport());
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

    void KongInitialized()
    {
#if UNITY_WEBGL
        if(Data.instance.initialized == false)
        {
            PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "kongInitialized", FunctionParameter = new { kongId = KongregateAPIBehaviour.instance.kongregateId } }, InitializedSuccess, TestFailure);
            Data.instance.initialized = true;
        }
#endif
    }
}
