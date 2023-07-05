using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : BaseEnemy
{
    private bool isFollowing = true;

    void Start()
    {
        currentHealth = maxHealth;
        TakeDamage += NormalHitted;
        TakeDamage += DeathComfirm;
    }

    protected override void Update()
    {
        base.Update();
        if (isFollowing && isAlive) Walk();
    }


    private void Walk()
    {
        Vector2 direction = (O_Character.Instance.transform.position - transform.position).normalized;
        rb_Enemy.velocity = direction * moveSpeed;
    }
}
