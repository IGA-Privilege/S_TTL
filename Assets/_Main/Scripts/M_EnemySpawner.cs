using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_EnemySpawner : Singleton<M_EnemySpawner>
{
	public GameObject playerToFollow;
	public Transform spawnArea;
	public GameObject[] aiPrefabsToSpawn;
	public float[] spawnTimer;
	public float[] lastSpawnTimer;
	public Data_Ranged[] rangedDatas;

	Vector2 detectPos;

	bool spawnIsAGo;
	float timerFunc;

	public LayerMask layerMaskNoSpawn;
	public float setDistance;

	void Start()
	{
		timerFunc = 0;
		for (int arrayIndex = 0; arrayIndex < lastSpawnTimer.Length; arrayIndex++)
			lastSpawnTimer[arrayIndex] = Time.time;
	}

	void Update()
	{
		for (int arrayIndex = 0; arrayIndex < aiPrefabsToSpawn.Length; arrayIndex++)
		{
			if (timerFunc == 10)
			{
				detectPos = getRandomLocation();
				timerFunc = 0;
			}
			else timerFunc++;

			Vector2 impactPoint = Vector2.zero;

			if (Physics2D.OverlapCircle(detectPos, 1, layerMaskNoSpawn)) spawnIsAGo = false;
			else
			{
				impactPoint = detectPos;
				spawnIsAGo = true;
			}
			if (playerBufferArea(playerToFollow.transform.position, impactPoint, setDistance) == false) spawnIsAGo = false;

			if (spawnIsAGo)
			{
				if ((Time.time - lastSpawnTimer[arrayIndex]) > spawnTimer[arrayIndex])
				{
					Instantiate(aiPrefabsToSpawn[arrayIndex], new Vector2(impactPoint.x, impactPoint.y), Quaternion.identity);
					lastSpawnTimer[arrayIndex] = Time.time;
				}
			}
		}
	}

	Vector2 getRandomLocation()
	{
		float randomX = Random.Range(-spawnArea.localScale.x / 2, spawnArea.localScale.x / 2);
		float randomY = Random.Range(-spawnArea.localScale.y / 2, spawnArea.localScale.y / 2);
		Vector2 randomPosition = new Vector2(randomX, randomY);
		return randomPosition;
	}

	bool playerBufferArea(Vector2 playerPosition, Vector2 impactPoint, float setDistance)
	{
		float distanceToSpawn = Vector2.Distance(playerPosition, impactPoint);

		if (distanceToSpawn > setDistance) return true;
		else return false;
	}
}
