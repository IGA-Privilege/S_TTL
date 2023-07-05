using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class BaseEnemy : MonoBehaviour
{
    public O_PowerUp deathLootPref;
    public Rigidbody2D rb_Enemy;
    public float moveSpeed = 2;
    public float maxHealth = 3;
    protected float currentHealth;
    protected Action TakeDamage;
    protected bool isAlive = true;
    protected float takeDamageTimer = 0f;
    protected float takeDamageInterval = 0.2f;
    public bool canTakeDamage { get { return takeDamageTimer > takeDamageInterval; } }

    protected virtual void Update()
    {
        takeDamageTimer += Time.deltaTime;
    }

    public void OnTakeDamage(float dmgAmount)
    {
        if (canTakeDamage)
        {
            currentHealth -= dmgAmount;
            TakeDamage();
            takeDamageTimer = 0f;
        }
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

    public void DeathComfirm()
    {
        if (currentHealth <= 0)
        {
            Instantiate(deathLootPref, transform.position, Quaternion.identity);
            GetComponent<BoxCollider2D>().enabled = false;
            float colorTime = 0.2f;
            isAlive = false;
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => DOTween.To(() => sprite.color, x => sprite.color = x, Color.red, colorTime));
            s.AppendInterval(colorTime);
            s.AppendCallback(() => DOTween.To(() => sprite.color, x => sprite.color = x, Color.white, colorTime));
            s.AppendInterval(colorTime);
            s.AppendCallback(() => DOTween.To(() => sprite.color, x => sprite.color = x, Color.red, colorTime));
            s.AppendInterval(colorTime);
            s.AppendCallback(() => DOTween.To(() => sprite.color, x => sprite.color = x, Color.white, colorTime));
            s.AppendInterval(colorTime);
            s.Append(transform.DOScale(0, 0.2f));
            s.AppendCallback(() => Destroy(gameObject, 0.1f));
        }
    }
}
