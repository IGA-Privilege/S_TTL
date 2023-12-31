using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Shield : O_Weapon
{
    private float _shieldFlyTimer = 0f;
    private float _maxFlyTime = 0.2f;
    private bool _isOneOff = false;
    private bool _hasHitEnemy = false;
    private Vector3 _normalScale;
    private Vector3 _extendedScale;

    private void Awake()
    {
        _normalScale = transform.localScale;
        _extendedScale = _normalScale * 2f;
    }

    public override void DoAttack()
    {
        float resizeSpeed = 15f;
        transform.localScale = Vector3.Lerp(transform.localScale, _extendedScale, resizeSpeed * Time.deltaTime);
        transform.up = aimDirection;

        _shieldFlyTimer += Time.deltaTime;
        SetColliderEnabled(true);
        rb_Weapon.velocity = aimDirection * weaponData.shootSpeed + O_Character.Instance.rb_Character.velocity;
        if (_shieldFlyTimer > _maxFlyTime) 
        {
            currentState = WeaponState.MovingBack;
            _shieldFlyTimer = 0f;
        }
    }

    public override void MovingBackToPlayer()
    {
        float resizeSpeed = 15f;
        transform.localScale = Vector3.Lerp(transform.localScale, _normalScale, resizeSpeed * Time.deltaTime);

        backDirection = (O_Character.Instance.transform.position - transform.position).normalized;
        rb_Weapon.velocity = 2 * backDirection * weaponData.shootSpeed;
        if (Vector2.Distance(transform.position, O_Character.Instance.transform.position) <= 1f)
        {
            if (_isOneOff)
            {
                Destroy(gameObject);
            }
            else
            {
                timer_CoolDown = weaponData.coolDown;
                currentState = WeaponState.ReadyForAttack;
                if (!_hasHitEnemy)
                {
                    M_Weapon.Instance.SetShieldBatterCharged(true);
                }
                else
                {
                    _hasHitEnemy = false;
                    M_Weapon.Instance.SetShieldBatterCharged(false);
                }
            }

        }
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            _hasHitEnemy = true;
        }
    }

    public void SetOneOff(bool isOneOff) { _isOneOff = isOneOff; }
}
