using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum WeaponState { ReadyForAttack, Attacking, MovingBack, OnCoolDown }
public class O_Weapon : MonoBehaviour
{
    protected int pivotIndex;
    protected WeaponState currentState = WeaponState.ReadyForAttack;
    protected Rigidbody2D rb_Weapon;
    protected Data_Weapon weaponData;
    protected float timer_CoolDown = 0;
    protected Vector2 aimDirection;
    protected Vector2 backDirection;
    protected Transform lauchPoint;

    private void Start()
    {
        rb_Weapon = GetComponent<Rigidbody2D>();
        lauchPoint = M_Weapon.Instance.GetWeaponPivotPos(pivotIndex);
    }

    private void Update()
    {
        switch (currentState)
        {
            case WeaponState.ReadyForAttack:
                FollowPlayer();
                break;
            case WeaponState.Attacking:
                DoAttack();
                break;
            case WeaponState.MovingBack:
                MovingBackToPlayer();
                break;
        }
    }

    public void InitializeWeapon(int targetIndex,Data_Weapon thisData)
    {
        pivotIndex = targetIndex;
        weaponData = thisData;
    }

    public void MovingBackToPlayer()
    {
        backDirection = (lauchPoint.position - transform.position).normalized;
        rb_Weapon.velocity = backDirection * weaponData.shootSpeed;
        if (Vector2.Distance(transform.position, lauchPoint.position) <= 0.3f)
        {
            timer_CoolDown = weaponData.coolDown;
            currentState = WeaponState.ReadyForAttack; 
        }
    }

    public void FollowPlayer()
    {
        transform.position = lauchPoint.position;
        timer_CoolDown -= Time.deltaTime;
        if (timer_CoolDown <= 0)
        {
            EnterAttack();
            currentState = WeaponState.Attacking;
        }
    }

    public void EnterAttack()
    {
        aimDirection = (transform.position - O_Character.Instance.transform.position).normalized;
    }

    public virtual void DoAttack() { }

    public void ExitAttack() 
    {
        GetComponent<BoxCollider2D>().enabled = false;
        currentState = WeaponState.MovingBack;
    }

    public void ChangeColliderStateTo(bool targetState)
    {
        GetComponent<BoxCollider2D>().enabled = targetState;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<BaseEnemy>().OnTakeDamage(weaponData.damageAmount);
        }
    }
}
