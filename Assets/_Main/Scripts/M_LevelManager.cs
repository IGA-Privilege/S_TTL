using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class M_LevelManager : Singleton<M_LevelManager>
{
    [SerializeField] private O_Character player;
    [FormerlySerializedAs("initialRegions")] [SerializeField] private List<Vector2Int> walkableRegions;
    [SerializeField] private Vector2Int playerStartRegion;
    [SerializeField] private O_Region regionPrefab;
    [SerializeField] private Transform gridStartPoint;
    private O_Region[,] _gridSystem;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        int initialGridSize = 9;
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
        player.transform.position = toRegion.GetPortalPosition(fromDirection);
        switch (fromDirection)
        {
            case TraverseDirection.Leftwards: player.transform.position += new Vector3(0.5f, 0, 0); break;
            case TraverseDirection.Rightwards: player.transform.position += new Vector3(-0.5f, 0, 0); break;
            case TraverseDirection.Upwards: player.transform.position += new Vector3(0, -0.5f, 0); break;
            case TraverseDirection.Downwards: player.transform.position += new Vector3(0, 0.5f, 0); break;
        }
    }
}
