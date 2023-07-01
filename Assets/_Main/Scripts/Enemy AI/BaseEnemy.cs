using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class BaseEnemy : MonoBehaviour
{
    public Rigidbody2D rb_Enemy;
    public float moveSpeed = 2;
    public int maxHealth = 3;
    protected float currentHealth;
    protected Action TakeDamage;
    protected bool isAlive = true;

    public void OnTakeDamage(int dmgAmount)
    {
        currentHealth -= dmgAmount;
        TakeDamage();
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
