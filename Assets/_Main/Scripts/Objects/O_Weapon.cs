using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum WeaponState { ReadyForAttack, Attacking, MovingBack, OnCoolDown }
public class O_Weapon : MonoBehaviour
{
    public WeaponState currentState = WeaponState.ReadyForAttack;
    protected Rigidbody2D rb_Weapon;
    protected Data_Weapon weaponData;
    protected float timer_CoolDown = 0;
    public Vector2 aimDirection;
    protected Vector2 backDirection;
    protected Vector3 lauchPoint;

    private void Start()
    {
        rb_Weapon = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case WeaponState.ReadyForAttack:
                break;
            case WeaponState.Attacking:
                DoAttack();
                break;
            case WeaponState.MovingBack:
                MovingBackToPlayer();
                break;
        }
    }

    public void InitializeWeapon(Vector3 launchPoint, Data_Weapon thisData, Vector2 flyDirection)
    {
        weaponData = thisData;
        lauchPoint = launchPoint;
        aimDirection = flyDirection;
        currentState = WeaponState.Attacking;
    }

    public virtual void MovingBackToPlayer()
    {
        backDirection = (O_Character.Instance.transform.position - transform.position).normalized;
        rb_Weapon.velocity = 2 * backDirection * weaponData.shootSpeed;
        if (Vector2.Distance(transform.position, O_Character.Instance.transform.position) <= 1f)
        {
            timer_CoolDown = weaponData.coolDown;
            currentState = WeaponState.ReadyForAttack; 
        }
    }


    public virtual void DoAttack() { }

    public void ExitAttack() 
    {
        GetComponent<BoxCollider2D>().enabled = false;
        currentState = WeaponState.MovingBack;
    }

    public void SetColliderEnabled(bool targetState)
    {
        GetComponent<Collider2D>().enabled = targetState;
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<BaseEnemy>().OnTakeDamage(weaponData.damageAmount);
        }
    }
}
