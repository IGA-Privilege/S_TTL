using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class M_LevelManager : Singleton<M_LevelManager>
{
    [SerializeField] private O_Character player;
    [FormerlySerializedAs("initialRegions")] [SerializeField] private List<Vector2Int> walkableRegions;
    [SerializeField] private O_Region regionPrefab;
    [SerializeField] private Transform gridStartPoint;
    [SerializeField] private TMP_Text nextRegionSpawnText;
    private O_Region[,] _gridSystem;
    private float _newRegionSpawnTimer = 0f;
    private readonly float _newRegionSpawnInterval = 15f;
    private readonly int initialGridSize = 11;


    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        TickNextRegionSpawn();
    }

    private void TickNextRegionSpawn()
    {
        _newRegionSpawnTimer += Time.deltaTime;
        if (_newRegionSpawnTimer > _newRegionSpawnInterval)
        {
            _newRegionSpawnTimer = 0f;
            SpawnRandomNewWalkableRegion();
        }
        nextRegionSpawnText.text = "Next Region Spawn In: " + Mathf.CeilToInt(_newRegionSpawnInterval - _newRegionSpawnTimer) + " sec";
    }

    private void SpawnRandomNewWalkableRegion()
    {
        Vector2Int newRegionCoords = Vector2Int.zero;
        bool hasFoundSpawnLocation = false;
        O_Region adjacentRegion = null;
        TraverseDirection portalDirection = TraverseDirection.Leftwards;
        int loopCounter = 0;
        while (!hasFoundSpawnLocation)
        {
            loopCounter++;
            if (loopCounter > 50)
            {
                break;
            }
            hasFoundSpawnLocation = TryFindSuitableSpawnCoords(ref newRegionCoords, ref adjacentRegion, ref portalDirection);
        }

        if (hasFoundSpawnLocation)
        {
            Debug.Log("Spawned new region! At: " + newRegionCoords, adjacentRegion.gameObject);
            ChangeVoidRegionToWalkable(newRegionCoords);
        }
    }

    private void ChangeVoidRegionToWalkable(Vector2Int regionCoords)
    {
        O_Region newWalkableRegion = _gridSystem[regionCoords.x, regionCoords.y];
        newWalkableRegion.IsVoid = false;
        walkableRegions.Add(newWalkableRegion.CoordsInGrid);

        if (walkableRegions.Contains(regionCoords + new Vector2Int(2, 0)))
        {
            O_Region adjacentRegion = _gridSystem[(regionCoords + new Vector2Int(2, 0)).x, (regionCoords + new Vector2Int(2, 0)).y];
            adjacentRegion.SetPortalOpen(TraverseDirection.Leftwards, newWalkableRegion);
            newWalkableRegion.SetPortalOpen(TraverseDirection.Rightwards, adjacentRegion);
        }
        if (walkableRegions.Contains(regionCoords + new Vector2Int(0, 2)))
        {
            O_Region adjacentRegion = _gridSystem[(regionCoords + new Vector2Int(0, 2)).x, (regionCoords + new Vector2Int(0, 2)).y];
            adjacentRegion.SetPortalOpen(TraverseDirection.Downwards, newWalkableRegion);
            newWalkableRegion.SetPortalOpen(TraverseDirection.Upwards, adjacentRegion);
        }
        if (walkableRegions.Contains(regionCoords + new Vector2Int(-2, 0)))
        {
            O_Region adjacentRegion = _gridSystem[(regionCoords + new Vector2Int(-2, 0)).x, (regionCoords + new Vector2Int(-2, 0)).y];
            adjacentRegion.SetPortalOpen(TraverseDirection.Rightwards, newWalkableRegion);
            newWalkableRegion.SetPortalOpen(TraverseDirection.Leftwards, adjacentRegion);
        }
        if (walkableRegions.Contains(regionCoords + new Vector2Int(0, -2)))
        {
            O_Region adjacentRegion = _gridSystem[(regionCoords + new Vector2Int(0, -2)).x, (regionCoords + new Vector2Int(0, -2)).y];
            adjacentRegion.SetPortalOpen(TraverseDirection.Upwards, newWalkableRegion);
            newWalkableRegion.SetPortalOpen(TraverseDirection.Downwards, adjacentRegion);
        }
    }

    private bool TryFindSuitableSpawnCoords(ref Vector2Int newRegionCoords, ref O_Region adjacentRegion, ref TraverseDirection portalDirection)
    {
        int randomIndex = UnityEngine.Random.Range(0, walkableRegions.Count);
        adjacentRegion = _gridSystem[walkableRegions[randomIndex].x, walkableRegions[randomIndex].y];
        int randomDirection = UnityEngine.Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0: newRegionCoords = adjacentRegion.CoordsInGrid + new Vector2Int(2, 0); portalDirection = TraverseDirection.Rightwards;  break;
            case 1: newRegionCoords = adjacentRegion.CoordsInGrid + new Vector2Int(0, 2); portalDirection = TraverseDirection.Upwards; break;
            case 2: newRegionCoords = adjacentRegion.CoordsInGrid + new Vector2Int(-2, 0); portalDirection = TraverseDirection.Leftwards; break;
            case 3: newRegionCoords = adjacentRegion.CoordsInGrid + new Vector2Int(0, -2); portalDirection = TraverseDirection.Downwards; break;
        }

        bool hasFound = true;

        if (newRegionCoords.x < 0 || newRegionCoords.x >= initialGridSize || newRegionCoords.y < 0 || newRegionCoords.y >= initialGridSize)
        {
            hasFound = false;
        }

        foreach (Vector2Int walkableRegion in walkableRegions)
        {
            if (Vector2Int.Distance(walkableRegion, newRegionCoords) < 2)
            {
                hasFound = false; break;
            }
        }

        return hasFound;
    }

    private void GenerateGrid()
    {
        _gridSystem = new O_Region[initialGridSize, initialGridSize];
        for (int i = 0; i < _gridSystem.GetLength(0); i++)
        {
            for (int j = 0; j < _gridSystem.GetLength(1); j++)
            {
                Vector2Int coords = new Vector2Int(i, j);
                if (walkableRegions.Contains(coords))
                {
                    O_Region region = Instantiate(regionPrefab, GetWorldPositionFromCoords(coords), Quaternion.identity);
                    region.Intialize(false, coords);
                    _gridSystem[i, j] = region;
                    player.transform.position = region.playerSpawnPoint.position;
                }
                else
                {
                    O_Region region = Instantiate(regionPrefab, GetWorldPositionFromCoords(coords), Quaternion.identity);
                    region.Intialize(true, coords);
                    _gridSystem[i, j] = region;
                }
            }
        }
    }

    public Vector3 GetWorldPositionFromCoords(Vector2Int coords)
    {
        float xOffset = coords.x * 2f * regionPrefab.HalfWidth;
        float yOffset = coords.x * 2f * regionPrefab.HalfWidth;
        return new Vector3(gridStartPoint.position.x + xOffset, gridStartPoint.position.y + yOffset, 0f) ;
    }

    public Vector3 GetRegionBoundsMin(Vector2Int coords)
    {
        float xOffset = (coords.x * 2 - 1) * regionPrefab.HalfWidth;
        float yOffset = (coords.y * 2 - 1) * regionPrefab.HalfWidth;
        return new Vector3(gridStartPoint.position.x + xOffset, gridStartPoint.position.y + yOffset, 0f);
    }

    public Vector3 GetRegionBoundsMax(Vector2Int coords)
    {
        float xOffset = (coords.x * 2 + 1) * regionPrefab.HalfWidth;
        float yOffset = (coords.y * 2 + 1) * regionPrefab.HalfWidth;
        return new Vector3(gridStartPoint.position.x + xOffset, gridStartPoint.position.y + yOffset, 0f);
    }

    public void HandlePlayerTraverse(O_Region toRegion, TraverseDirection fromDirection)
    {
        float offset = 1f;
        player.transform.position = toRegion.GetPortalPosition(fromDirection);
        switch (fromDirection)
        {
            case TraverseDirection.Leftwards: player.transform.position += new Vector3(offset, 0, 0); break;
            case TraverseDirection.Rightwards: player.transform.position += new Vector3(-offset, 0, 0); break;
            case TraverseDirection.Upwards: player.transform.position += new Vector3(0, -offset, 0); break;
            case TraverseDirection.Downwards: player.transform.position += new Vector3(0, offset, 0); break;
        }
    }
}