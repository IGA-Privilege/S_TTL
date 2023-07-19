using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class O_Region : MonoBehaviour
{
    [SerializeField] private Sprite regionSprite;
    [SerializeField] private Sprite voidSprite;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private O_Portal leftPortal;
    [SerializeField] private O_Portal rightPortal;
    [SerializeField] private O_Portal upPortal;
    [SerializeField] private O_Portal downPortal;
    [HideInInspector]
    public bool IsVoid;
    [HideInInspector]
    public Vector2Int CoordsInGrid;
    public float HalfWidth { get { return 10f; } }
    public Bounds SpriteBounds { get { return spriteRenderer.bounds; } }

    private float _enemySpawnTimer;
    private readonly float _enemySpawnInterval = 10f;

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
        M_EnemySpawner.Instance.HandleSpawnEnemyRequest(this);
    }

    public void Intialize(bool isVoid, Vector2Int coordsInGrid)
    {
        IsVoid = isVoid;
        CoordsInGrid = coordsInGrid;
        if (IsVoid)
        {
            spriteRenderer.sprite = voidSprite;
            boxCollider.isTrigger = false;
        }
        else
        {
            spriteRenderer.sprite = regionSprite;
            boxCollider.isTrigger = true;
        }

        Vector3 center = 0.5f * (M_LevelManager.Instance.GetRegionBoundsMax(CoordsInGrid) + M_LevelManager.Instance.GetRegionBoundsMin(CoordsInGrid));
        // Spirte素材的pixelPerUnit需要为图片的长宽（像素）的二分之一
        transform.position = center;
        transform.localScale = 0.5f * (M_LevelManager.Instance.GetRegionBoundsMax(CoordsInGrid) - M_LevelManager.Instance.GetRegionBoundsMin(CoordsInGrid));
    }
}

public enum TraverseDirection
{
    Leftwards, Rightwards, Upwards, Downwards
}
