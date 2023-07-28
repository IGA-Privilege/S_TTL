using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : BaseEnemy
{
    private Action AIAction;
    private readonly float _distanceStartBlock = 0.85f;
    private bool _shakeFlag = false;

    void Start()
    {
        currentHealth = maxHealth;
        TakeDamage += NormalHitted;
        TakeDamage += DeathComfirm;
    }

    protected override void Update()
    {
        base.Update();
        if (isAlive) StateControl();
        if (AIAction != null) AIAction();
    }

    private void StateControl()
    {
        float distance = Vector2.Distance(O_Character.Instance.transform.position, transform.position);
        if (distance > _distanceStartBlock)
        {
            AIAction = ApproachPlayer;
        }
        else
        {
            AIAction = PerformBlock;
        }
    }

    private void PerformBlock()
    {
        rb_Enemy.velocity = Vector3.zero;
        rb_Enemy.inertia = 0f;
        rb_Enemy.isKinematic = true;
        _shakeFlag = !_shakeFlag;
        if (_shakeFlag)
        {
            transform.position += new Vector3(-0.1f, 0f, 0f);
        }
        else
        {
            transform.position += new Vector3(0.1f, 0f, 0f);
        }
    }

    private void ApproachPlayer()
    {
        rb_Enemy.isKinematic = false;
        Vector2 direction = (O_Character.Instance.transform.position - transform.position).normalized;
        rb_Enemy.velocity = direction * moveSpeed;
    }
}
