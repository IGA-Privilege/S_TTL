using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[RequireComponent(typeof(BoxCollider2D))]
public class O_Region : MonoBehaviour
{
    [SerializeField] private SpriteRenderer mainSpriteRenderer;
    [SerializeField] private SpriteRenderer neonTexture;
    [SerializeField] private BoxCollider2D boxCollider;
    public O_Portal leftPortal;
    public O_Portal rightPortal;
    public O_Portal upPortal;
    public O_Portal downPortal;
    public Transform playerSpawnPoint;
    [HideInInspector]
    public bool IsVoid;
    [HideInInspector]
    public RegionEnemyType EnemyType;
    [HideInInspector]
    public Vector2Int CoordsInGrid;
    public float HalfWidth { get { return 10f; } }
    public float HalfHeight { get { return 5.14f; } }
    public Bounds SpriteBounds { get { return mainSpriteRenderer.bounds; } }

    private float _enemySpawnTimer;
    private readonly float _enemySpawnInterval = 10f;
    private Grid _gridObj;

    private void Awake()
    {
        _gridObj = GetComponentInChildren<Grid>(); ;
    }

    private void Start()
    {
        _enemySpawnTimer = 0f;
    }

    private void Update()
    {
        if (!IsVoid)
        {
            TickEnemySpawn();
        }
    }

    public void SetPortalOpen(TraverseDirection whichPortal, O_Region regionLinkedToPortal)
    {
        switch (whichPortal)
        {
            case TraverseDirection.Leftwards: leftPortal.isOpen = true; leftPortal.OpenAndLinkRegion(regionLinkedToPortal); break;
            case TraverseDirection.Rightwards: rightPortal.isOpen = true; rightPortal.OpenAndLinkRegion(regionLinkedToPortal); break;
            case TraverseDirection.Upwards: upPortal.isOpen = true; upPortal.OpenAndLinkRegion(regionLinkedToPortal); break;
            case TraverseDirection.Downwards: downPortal.isOpen = true; downPortal.OpenAndLinkRegion(regionLinkedToPortal); break;
        }

        UpdateSpriteAndCollider();
    }

    public Vector3 GetPortalPosition(TraverseDirection whichPortal)
    {
        switch (whichPortal)
        {
            case TraverseDirection.Leftwards: return leftPortal.transform.position;
            case TraverseDirection.Rightwards: return rightPortal.transform.position;
            case TraverseDirection.Upwards: return upPortal.transform.position;
            case TraverseDirection.Downwards: return downPortal.transform.position;
        }
        return Vector3.zero;
    }

    public void PlayerTryTraverse(O_Region regionTraverseTo, TraverseDirection fromDirection)
    {
        M_LevelManager.Instance.HandlePlayerTraverse(regionTraverseTo, fromDirection);
    }

    private void TickEnemySpawn()
    {
        _enemySpawnTimer += Time.deltaTime;
        if (_enemySpawnTimer > _enemySpawnInterval)
        {
            _enemySpawnTimer = 0f;
            RequestAnEnemySpawn();
        }
    }

    private void RequestAnEnemySpawn()
    {
        M_EnemySpawner.Instance.HandleSpawnEnemyRequest(this, EnemyType);
    }

    public void Intialize(bool isVoid, Vector2Int coordsInGrid)
    {
        IsVoid = isVoid;
        CoordsInGrid = coordsInGrid;
        UpdateSpriteAndCollider();

        Vector3 center = 0.5f * (M_LevelManager.Instance.GetRegionBoundsMax(CoordsInGrid) + M_LevelManager.Instance.GetRegionBoundsMin(CoordsInGrid));
        // Spirte素材的pixelPerUnit需要为图片的长的二分之一，并将素材的长宽比同步到此脚本的HalfWidth、HalfHeight
        transform.position = center;
        Vector3 regionBoundsSize = 0.5f * (M_LevelManager.Instance.GetRegionBoundsMax(CoordsInGrid) - M_LevelManager.Instance.GetRegionBoundsMin(CoordsInGrid));
        transform.localScale = new Vector3(regionBoundsSize.x, regionBoundsSize.x, 1f);
    }

    private void UpdateSpriteAndCollider()
    {
        if (IsVoid)
        {
            _gridObj.gameObject.SetActive(false);
            neonTexture.enabled = false;
            boxCollider.isTrigger = false;
        }
        else
        {
            _gridObj.gameObject.SetActive(true);
            mainSpriteRenderer.enabled = true;
            neonTexture.enabled = true;
            boxCollider.isTrigger = true;
        }
    }

    public static TraverseDirection GetTraverseDirectionOpposite(TraverseDirection direction)
    {
        switch (direction)
        {
            case TraverseDirection.Leftwards: return TraverseDirection.Rightwards;
                case TraverseDirection.Rightwards: return TraverseDirection.Leftwards;
                case TraverseDirection.Upwards: return TraverseDirection.Downwards;
                case TraverseDirection.Downwards: return TraverseDirection.Upwards;
        }
        return TraverseDirection.Leftwards;
    }
}

public enum TraverseDirection
{
    Leftwards, Rightwards, Upwards, Downwards
}

public enum RegionEnemyType 
{
    Melee, RangedA
}
