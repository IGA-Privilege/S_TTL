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

    public void HandleSpawnEnemyRequest(O_Region regionIn, RegionEnemyType enemyType)
    {
        GetRandomSpawnLocation(regionIn);
        SpawnEnemyAccordingToRegion(enemyType);
    }

    private void SpawnEnemyAccordingToRegion(RegionEnemyType enemyType)
    {
        BaseEnemy enemyToSpawn = null;
        switch (enemyType)
        {
            case RegionEnemyType.Melee:
                EnemyMelee meleeEnemy = Instantiate(meleeEnemyPrefab, nextSpawnPos, Quaternion.identity);
                enemyToSpawn = meleeEnemy;
                break;
            case RegionEnemyType.RangedA:
                EnemyRanged rangedEnemy = Instantiate(rangedEnemyPrefab, nextSpawnPos, Quaternion.identity);
                rangedEnemy.SetEnemyInfo(rangedDatas[0]);
                enemyToSpawn = rangedEnemy;
                break;
            default:
                break;
        }

        if (enemyToSpawn != null)
        {
            _spawnedEnemies.Add(enemyToSpawn);
        }
    }

    public void OnEnemyDie(BaseEnemy enemyDead)
    {
        if (_spawnedEnemies.Contains(enemyDead))
        {
            _spawnedEnemies.Remove(enemyDead);
        }
    }

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
