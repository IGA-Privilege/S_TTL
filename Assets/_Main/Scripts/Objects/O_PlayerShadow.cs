using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_PlayerShadow : MonoBehaviour
{
    private float _lifeTimer = 0f;
    private Rigidbody2D _rb2D;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _rb2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        _lifeTimer += Time.deltaTime;
        if (_lifeTimer > O_Character.Instance.dashTime) 
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (O_Character.Instance.lastMoveDirection.x > 0)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }

        _rb2D.velocity = O_Character.Instance.lastMoveDirection * O_Character.Instance.dashSpeed;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, O_Character.Instance.dashImpactRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                enemy.OnTakeDamage(O_Character.Instance.dashDamage);

                if (M_Weapon.Instance.runeActivationDic[RunePower.MagneticDash])
                {
                    Vector2 pullDirection = O_Character.Instance.destination.position - enemy.transform.position;
                    enemy.rb_Enemy.velocity = pullDirection.normalized * O_Character.Instance.dashImpactStrength;
                }
                else
                {
                    enemy.rb_Enemy.velocity = O_Character.Instance.lastMoveDirection * O_Character.Instance.dashImpactStrength;
                }

                if (M_Weapon.Instance.runeActivationDic[RunePower.OffensiveDash])
                {
                    // 利用扣血的CD来防止撞同一个怪射多根长矛
                    if (enemy.canTakeDamage)
                    {
                        float randomX = UnityEngine.Random.Range(-1f, 1f);
                        float randomY = UnityEngine.Random.Range(-1f, 1f);
                        M_Weapon.Instance.SpawnNewWeapon(M_Weapon.Instance._spearData, transform.position, new Vector2(randomX, randomY));
                    }
                }
            }
        }
    }
}
