using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private Transform hoverPoint;
    [SerializeField] private Transform bigHead;
    [SerializeField] private Animator animator;
    [SerializeField] private LightningBolt2D laser;
    [SerializeField] private LightningBolt2D laserTargetHint;
    private float _poseChangeTimer = 0f;
    private readonly float _poseChangeInterval = 1.5f;
    private float _lazerShootTimer = 0f;
    private readonly float _lazerShootInterval = 7f;

    private void Awake()
    {
        laser.transform.position = transform.position;
        laserTargetHint.transform.position = transform.position;
        laser.gameObject.SetActive(false);
        laserTargetHint.gameObject.SetActive(false);
    }

    private void Update()
    {
        FollowPlayer();
        _poseChangeTimer += Time.deltaTime;
        if (_poseChangeTimer > _poseChangeInterval)
        {
            _poseChangeTimer = 0f;
            RandomlyChangeHoverPoint();
        }
        BigHeadFollowsHoverPoint();

        _lazerShootTimer += Time.deltaTime;
        if (_lazerShootTimer > _lazerShootInterval)
        {
            _lazerShootTimer = 0f;
            StartCoroutine(PerformLazerShoot());
        }
        else if (_lazerShootTimer > 0.9f * _lazerShootInterval)
        {
            laserTargetHint.gameObject.SetActive(true);
            laserTargetHint.startPoint = bigHead.transform.position;
            laserTargetHint.endPoint = O_Character.Instance.transform.position;
        }
        else
        {
            laserTargetHint.gameObject.SetActive(false);
        }
    }

    private IEnumerator PerformLazerShoot()
    {
        O_Character.Instance.SetStunned();
        laser.gameObject.SetActive(true);
        laser.startPoint = bigHead.transform.position;
        laser.endPoint = O_Character.Instance.transform.position;

        yield return new WaitForSeconds(1f);

        laser.gameObject.SetActive(false);
    }

    private void FollowPlayer()
    {
        float followSpeed = 1.0f;
        Vector3 distance = new Vector3(0, 7f, 0);
        transform.position = Vector3.Lerp(transform.position, O_Character.Instance.transform.position + distance, followSpeed * Time.deltaTime);
    }

    private void RandomlyChangeHoverPoint()
    {
        float randomX = UnityEngine.Random.Range(-4f, 4f);
        float randomY = UnityEngine.Random.Range(-1f, 1f);
        Vector3 randomOffset = new Vector2(randomX, randomY);
        hoverPoint.transform.position = transform.position + randomOffset;
    }

    private void BigHeadFollowsHoverPoint()
    {
        float followSpeed = 0.3f;
        bigHead.position = Vector3.Lerp(bigHead.position, hoverPoint.position, followSpeed * Time.deltaTime);
    }
}
