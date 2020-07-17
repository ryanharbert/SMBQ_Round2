using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Battle Layout", menuName = "Battle Layout")]
public class BattleLayout : ScriptableObject
{
	public Vector3[] playerStrongholdLocations;
	public Vector3[] playerAutoSpawnPositions;
	public Vector3 enemyStrongholdLocation;
	public Vector3[] enemyBuildingLocations;
    public int enemyStrongholdRotation;
    public int[] enemyBuildingRotations;
	public Vector3[] spawnPositions;
}
