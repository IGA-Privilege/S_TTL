using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyRanged : BaseEnemy
{
    private enum RangedEnemyState { Approach, Shoot, Escape }
    private Action AIAction;

    private Data_Ranged enemyInfo;
    private float shootTimer;
    private float behaviorChangeTimer;
    private bool _behaviourFlag = false;

    void Start()
    {
        currentHealth = enemyInfo.maxHp;
        TakeDamage += NormalHitted;
        TakeDamage += DeathComfirm;
    }

    public void SetEnemyInfo(Data_Ranged info)
    {
        enemyInfo = info;
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
        shootTimer -= Time.deltaTime;
        behaviorChangeTimer -= Time.deltaTime;
        if (distance < enemyInfo.shootRange && shootTimer <= 0) ShootPlayer();

        if(behaviorChangeTimer <= 0)
        {
            if (distance < enemyInfo.shootRange) AIAction = EscapeFromPlayer;
            else AIAction = PerformManeuver;
            behaviorChangeTimer = UnityEngine.Random.Range(1, 2);
            _behaviourFlag = !_behaviourFlag;
        }
     
    }

    private void PerformManeuver()
    {
        if (!O_Character.Instance.isDashing)
        {
            Vector2 playerDirection = (O_Character.Instance.transform.position - transform.position).normalized;
            playerDirection = Vector2.Perpendicular(playerDirection) * (_behaviourFlag ? 1 : -1);
            rb_Enemy.velocity = playerDirection * moveSpeed;
        }
    }

    private void EscapeFromPlayer()
    {
        if (!O_Character.Instance.isDashing)
        {
            Vector2 direction = (transform.position - O_Character.Instance.transform.position).normalized;
            rb_Enemy.velocity = direction * moveSpeed;
        }
    }

    private void ShootPlayer()
    {
        shootTimer = enemyInfo.fireRate;
        Vector2 shootDirection = O_Character.Instance.transform.position - transform.position;

        if (enemyInfo.bulletCount > 1)
        {
            List<float> directions = VectorListCalculator(enemyInfo.bulletCount, enemyInfo.projectileRangeGap);
            foreach (var direction in directions)
            {
                GameObject newBullet = Instantiate(enemyInfo.bulletPrefab, transform.position, Quaternion.identity);
                newBullet.GetComponent<O_Bullet>().InitializeBullet(enemyInfo.bulletDamage, enemyInfo.bulletSpeed, direction);
                newBullet.GetComponent<O_Bullet>().FireBullet(this);
            }
        }
        else
        {
            GameObject newBullet = Instantiate(enemyInfo.bulletPrefab, transform.position, Quaternion.identity);
            newBullet.GetComponent<O_Bullet>().InitializeBullet(enemyInfo.bulletDamage, enemyInfo.bulletSpeed, shootDirection);
            newBullet.GetComponent<O_Bullet>().FireBullet(this);
        }


        List<float> VectorListCalculator(int number, float degreeGap)
        {
            int remainder = 0;
            int half = 0;
            List<float> degrees = new List<float>();
            if (number > 1)
            {
                remainder = number % 2;
                if (remainder == 1) half = (number - 1) / 2;
                else half = number / 2;
                for (int i = 0; i < number; i++)
                {
                    if (remainder == 1)
                    {
                        if (i < half) degrees.Add(-(half - i) * degreeGap);
                        else if (i == half) degrees.Add(0);
                        else if (i > half) degrees.Add((i - half) * degreeGap);
                    }
                    else
                    {
                        if (i < half) degrees.Add(-((half - i - 1) * degreeGap + degreeGap / 2));
                        else degrees.Add((i - half) * degreeGap + degreeGap / 2);
                    }
                }
            }
            for (int i = 0; i < degrees.Count; i++)
                degrees[i] += Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
            return degrees;
        }
    }
}
