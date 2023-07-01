using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_EnemySpawner : Singleton<M_EnemySpawner>
{
	public GameObject playerToFollow;
	public Transform spawnArea;
	public Data_Ranged[] rangedDatas;
	public EnemyMelee meleeEnemyPrefab;
	public EnemyRanged rangedEnemyPrefab;
	public LayerMask spawningFieldLayerM;
    public Transform fieldMelee;
    public Transform fieldRanged0;
    public Transform fieldRanged1;
    public Transform fieldRanged2;
	public float viewDistance;
    public float maxSpawnDistance;
    public float spawnInterval;//每隔几秒生成一只怪

    private Vector2 nextSpawnPos;
	private float timerFunc;



    void Start()
	{
		timerFunc = 0;  
	}

	void Update()
    {
        timerFunc += Time.deltaTime;

        if (CanSpawnEnemy())
        {
            GetRandomSpawnLocation();
            SpawnEnemyAccordingToPlayerPos();
        }
    }

    private void SpawnEnemyAccordingToPlayerPos()
    {
        float distanceToMeleeField = Vector2.Distance(playerToFollow.transform.position, fieldMelee.position);
        float distanceToRangedField0 = Vector2.Distance(playerToFollow.transform.position, fieldRanged0.position);
        float distanceToRangedField1 = Vector2.Distance(playerToFollow.transform.position, fieldRanged1.position);
        float distanceToRangedField2 = Vector2.Distance(playerToFollow.transform.position, fieldRanged2.position);

        if (distanceToMeleeField < distanceToRangedField0 && distanceToMeleeField < distanceToRangedField1 && distanceToMeleeField < distanceToRangedField2)
        {
            Instantiate(meleeEnemyPrefab, nextSpawnPos, Quaternion.identity);
            Debug.Log("生成了近战型");
        }
        else if (distanceToRangedField0 < distanceToMeleeField && distanceToRangedField0 < distanceToRangedField1 && distanceToRangedField0< distanceToRangedField2)
        {
            EnemyRanged enemyRanged = Instantiate(rangedEnemyPrefab, nextSpawnPos, Quaternion.identity);
            enemyRanged.SetEnemyInfo(rangedDatas[0]);
            Debug.Log("生成了远程型1");
        }
        else if (distanceToRangedField1 < distanceToMeleeField && distanceToRangedField1 < distanceToRangedField0 && distanceToRangedField1 < distanceToRangedField2)
        {
            EnemyRanged enemyRanged = Instantiate(rangedEnemyPrefab, nextSpawnPos, Quaternion.identity);
            enemyRanged.SetEnemyInfo(rangedDatas[1]);
            Debug.Log("生成了远程型2");
        }
        else if (distanceToRangedField2 < distanceToMeleeField && distanceToRangedField2 < distanceToRangedField0 && distanceToRangedField2 < distanceToRangedField1)
        {
            EnemyRanged enemyRanged = Instantiate(rangedEnemyPrefab, nextSpawnPos, Quaternion.identity);
            enemyRanged.SetEnemyInfo(rangedDatas[2]);
            Debug.Log("生成了远程型3");
        }
    }


    private bool CanSpawnEnemy()
    {
        bool spawnIsAGo = false;

        if (Physics2D.OverlapCircle(playerToFollow.transform.position, 0.2f, spawningFieldLayerM))
        {
            if (timerFunc > spawnInterval)
            {
                timerFunc = 0f;
                spawnIsAGo = true;
            }
        }

        return spawnIsAGo;
    }

    private void GetRandomSpawnLocation()
    {
        nextSpawnPos = GetRandomLocation();
        while (IsDistanceTooNear(playerToFollow.transform.position, nextSpawnPos, viewDistance))
        {
            nextSpawnPos = GetRandomLocation();
        }


    }

    Vector2 GetRandomLocation()
	{
		float randomX = Random.Range(playerToFollow.transform.position.x - maxSpawnDistance, playerToFollow.transform.position.x + maxSpawnDistance);
		float randomY = Random.Range(playerToFollow.transform.position.y - maxSpawnDistance, playerToFollow.transform.position.y + maxSpawnDistance);
		Vector2 randomPosition = new Vector2(randomX, randomY);
		return randomPosition;
	}

	private bool IsDistanceTooNear(Vector2 playerPosition, Vector2 impactPoint, float setDistance)
	{
		float distanceToSpawn = Vector2.Distance(playerPosition, impactPoint);

		if (distanceToSpawn > setDistance) return false;
		else return true;
	}
}
