using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class O_Character : Singleton<O_Character>
{
    public float moveSpeed;
    public float dashDistance;
    [HideInInspector] public Rigidbody2D rb_Character;
    public Transform destination;
    public O_PlayerShadow dashBlurShadow;
    private SpriteRenderer characterSprite;
    private Animator _animator;

    public float maxHP = 999;
    private float currentHP;

    public float originalDashDamage = 1f;
    [HideInInspector] public float dashDamage = 1f;
    public float originalDashCoolDown = 3f;
    [HideInInspector] public float dashCoolDown = 3f;
    [HideInInspector] public float dashTime = 0.2f;
    private float _dashTimer = 0.2f;
    public float dashSpeed = 20f;
    public float dashImpactRadius = 1f;
    public float dashImpactStrength = 25f;
    public bool isDashing { get { return _dashTimer < dashTime; } }
    public bool canDash { get { return _dashTimer > dashCoolDown; } }
    [HideInInspector] public Vector2 lastMoveDirection;
    private bool _canControl = true;
    public SpriteRenderer stunEffect;


    void Start()
    {
        _animator = GetComponent<Animator>();
        rb_Character = GetComponent<Rigidbody2D>();
        characterSprite = GetComponent<SpriteRenderer>();
        currentHP = maxHP;
    }

    private void AddHP(float amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    private void Update()
    {
        _dashTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (canDash && _canControl)
            {
                _dashTimer = 0f;
                if (M_Weapon.Instance.runeActivationDic[RunePower.BlurDash])
                {
                    O_PlayerShadow shadow1 = Instantiate(dashBlurShadow, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                    O_PlayerShadow shadow2 = Instantiate(dashBlurShadow, transform.position + new Vector3(0, -1, 0), Quaternion.identity);
                }
            }
        }
    }

    private void TickDashThings(Vector2 dashDirection)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, dashImpactRadius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                if (enemy.canTakeDamage)
                {
                    // 利用扣血的CD来防止同一个怪多次触发被动
                    if (M_Weapon.Instance.runeActivationDic[RunePower.OffensiveDash])
                    {
                        float randomX = UnityEngine.Random.Range(-1f, 1f);
                        float randomY = UnityEngine.Random.Range(-1f, 1f);
                        M_Weapon.Instance.SpawnNewWeapon(M_Weapon.Instance._spearData, transform.position, new Vector2(randomX, randomY));
                    }

                    if (M_Weapon.Instance.runeActivationDic[RunePower.HealingDash])
                    {
                        AddHP((float)0.02f * maxHP);
                    }

                    if (M_Weapon.Instance.runeActivationDic[RunePower.ImpactDash])
                    {
                        M_Weapon.Instance.DoShieldBatter();
                    }
                }
                // 这行代码要靠后放
                enemy.OnTakeDamage(dashDamage);

                if (M_Weapon.Instance.runeActivationDic[RunePower.MagneticDash])
                {
                    Vector2 pullDirection = destination.position - enemy.transform.position;
                    enemy.rb_Enemy.velocity = pullDirection.normalized * dashImpactStrength;
                }
                else
                {
                    enemy.rb_Enemy.velocity = dashDirection * dashImpactStrength;
                }
            }
        }
    }

    void FixedUpdate()
    {
        float horiAxis = Input.GetAxis("Horizontal");
        float verAxis = Input.GetAxis("Vertical");

        Vector2 direction = Vector2.zero;
        if (_canControl)
        {
            direction = new Vector2(horiAxis, verAxis).normalized;
        }
        lastMoveDirection = direction;

        if (isDashing)
        {
            rb_Character.velocity = direction * dashSpeed;
            TickDashThings(direction);
        }
        else
        {
            if (direction != Vector2.zero)
            {
                rb_Character.velocity = direction * moveSpeed;
                _animator.SetBool("isMoving", true);
            }
            else
            {
                rb_Character.velocity = Vector2.zero;
                _animator.SetBool("isMoving", false);
            }

            if (horiAxis > 0)
            {
                characterSprite.flipX = false;
            }
            else if (horiAxis < 0)
            {
                characterSprite.flipX = true;
            }
        }
    }

    public void TakeDamage(BaseEnemy damageSource)
    {
        currentHP--;
        NormalHitted();
        M_Weapon.Instance.DoAvengeActions(damageSource);
    }

    public void NormalHitted()
    {
        float colorTime = 0.2f;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Sequence s = DOTween.Sequence();
        s.AppendCallback(() => DOTween.To(() => sprite.color, x => sprite.color = x, Color.red, colorTime));
        s.AppendInterval(colorTime);
        s.AppendCallback(() => DOTween.To(() => sprite.color, x => sprite.color = x, Color.white, colorTime));
    }

    public void SetStunned()
    {
        StartCoroutine(RecoverFromStunned());
    }

    private IEnumerator RecoverFromStunned()
    {
        _canControl = false;
        yield return new WaitForSeconds(0.4f);
        stunEffect.gameObject.SetActive(true);
        yield return new WaitForSeconds(2.6f);
        _canControl = true;
        stunEffect.gameObject.SetActive(false);
    }
}
