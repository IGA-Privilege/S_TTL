using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

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
    public TMP_Text enemiesNumberText;

    private Vector2 nextSpawnPos;
    private HashSet<BaseEnemy> _spawnedEnemies = new HashSet<BaseEnemy>();
    private readonly int _maxEnemiesNumber = 80;

    private void Update()
    {
        CheckEnemiesNumber();
    }

    private void CheckEnemiesNumber()
    {
        if (_spawnedEnemies.Count > _maxEnemiesNumber)
        {
            GameOver();
        }
        enemiesNumberText.text = "Enemies Number:" + _spawnedEnemies.Count.ToString();
    }

    private void GameOver()
    {
        Time.timeScale = 0f;
        Debug.LogError("Game Over! Enemies Too Many!");
    }

    public void HandleSpawnEnemyRequest(O_Region regionIn)
    {
        GetRandomSpawnLocation(regionIn);
        SpawnEnemyAccordingToRegion();
    }

    private void SpawnEnemyAccordingToRegion()
    {
        EnemyRanged enemyRanged = Instantiate(rangedEnemyPrefab, nextSpawnPos, Quaternion.identity);
        enemyRanged.SetEnemyInfo(rangedDatas[0]);
        _spawnedEnemies.Add(enemyRanged);
    }

    public void OnEnemyDie(BaseEnemy enemyDead)
    {
        if (_spawnedEnemies.Contains(enemyDead))
        {
            _spawnedEnemies.Remove(enemyDead);
        }
    }

    /**
    private void SpawnEnemyAccordingToRegion()
    {
        float distanceToMeleeField = Vector2.Distance(playerToFollow.transform.position, fieldMelee.position);
        float distanceToRangedField0 = Vector2.Distance(playerToFollow.transform.position, fieldRanged0.position);
        float distanceToRangedField1 = Vector2.Distance(playerToFollow.transform.position, fieldRanged1.position);
        float distanceToRangedField2 = Vector2.Distance(playerToFollow.transform.position, fieldRanged2.position);

        if (distanceToMeleeField < distanceToRangedField0 && distanceToMeleeField < distanceToRangedField1 && distanceToMeleeField < distanceToRangedField2)
        {
            Instantiate(meleeEnemyPrefab, nextSpawnPos, Quaternion.identity);
        }
        else if (distanceToRangedField0 < distanceToMeleeField && distanceToRangedField0 < distanceToRangedField1 && distanceToRangedField0< distanceToRangedField2)
        {
            EnemyRanged enemyRanged = Instantiate(rangedEnemyPrefab, nextSpawnPos, Quaternion.identity);
            enemyRanged.SetEnemyInfo(rangedDatas[0]);
        }
        else if (distanceToRangedField1 < distanceToMeleeField && distanceToRangedField1 < distanceToRangedField0 && distanceToRangedField1 < distanceToRangedField2)
        {
            EnemyRanged enemyRanged = Instantiate(rangedEnemyPrefab, nextSpawnPos, Quaternion.identity);
            enemyRanged.SetEnemyInfo(rangedDatas[1]);
        }
        else if (distanceToRangedField2 < distanceToMeleeField && distanceToRangedField2 < distanceToRangedField0 && distanceToRangedField2 < distanceToRangedField1)
        {
            EnemyRanged enemyRanged = Instantiate(rangedEnemyPrefab, nextSpawnPos, Quaternion.identity);
            enemyRanged.SetEnemyInfo(rangedDatas[2]);
        }
    }
    */



    private void GetRandomSpawnLocation(O_Region region)
    {
        nextSpawnPos = GetRandomLocation(region);
        while (IsDistanceTooNear(playerToFollow.transform.position, nextSpawnPos, viewDistance))
        {
            nextSpawnPos = GetRandomLocation(region);
        }
    }

    private Vector2 GetRandomLocation(O_Region region)
	{
		float randomX = Random.Range(region.SpriteBounds.min.x, region.SpriteBounds.max.x);
		float randomY = Random.Range(region.SpriteBounds.min.y, region.SpriteBounds.max.y);
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
