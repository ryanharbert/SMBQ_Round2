using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleScene : MonoBehaviour
{
	public Transform[] playerStrongholds;
	public Transform[] playerSpawn;
	public Transform enemyStronghold;
	public Transform[] enemyBuildings;
	public Transform[] enemySpawn;

    public SpawnArea anywhereSpawnArea;
    public SpawnArea playerSpawnArea;
    public SpawnArea enemySpawnArea;

    private void Awake()
    {
        if(Data.instance == null)
        {
            StartCoroutine(TestScene());
        }

        TurnOffAll(playerStrongholds);
        TurnOffAll(playerSpawn);
        TurnOff(enemyStronghold);
        TurnOffAll(enemyBuildings);
        TurnOffAll(enemySpawn);
    }

    IEnumerator TestScene()
    {
        //Scene activeScene = SceneManager.GetActiveScene();
        string currentScene = SceneManager.GetActiveScene().name;
        AsyncOperation load = SceneManager.LoadSceneAsync("BattleTest", LoadSceneMode.Additive);

        while (!load.isDone)
        {
            yield return null;
        }

        SceneManager.MergeScenes(SceneManager.GetSceneByName("BattleTest"), SceneManager.GetSceneByName(currentScene));
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentScene));

        Battle b = FindObjectOfType<Battle>();
        b.Setup(new BattleData());
    }

        void TurnOffAll(Transform[] transforms)
    {
        foreach (Transform t in transforms)
        {
            TurnOff(t);
        }
    }

    void TurnOff(Transform t)
    {
        if (t == null)
            return;

        MeshRenderer m = t.GetComponent<MeshRenderer>();
        if(m != null)
        {
            m.enabled = false;
        }
    }
}
