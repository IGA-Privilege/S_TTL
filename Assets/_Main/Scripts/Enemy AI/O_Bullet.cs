using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Bullet : MonoBehaviour
{
    private int damage;
    private float moveSpeed;
    private Vector2 direction;
    private Rigidbody2D rigid;

    public void InitializeBullet(int _damage,float _moveSpeed,Vector2 _direction)
    {
        rigid = GetComponent<Rigidbody2D>();
        damage = _damage;
        moveSpeed = _moveSpeed;
        //direction = _direction.normalized;
        float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0,angle);
    }

    public void InitializeBullet(int _damage, float _moveSpeed, float _angle)
    {
        rigid = GetComponent<Rigidbody2D>();
        damage = _damage;
        moveSpeed = _moveSpeed;
        //direction = _direction.normalized;
        //float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, _angle);
    }

    public void FireBullet()
    {
        rigid.AddForce(moveSpeed * transform.right, ForceMode2D.Impulse);
    }
}
