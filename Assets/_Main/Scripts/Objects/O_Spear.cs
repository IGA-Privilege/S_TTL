using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Spear : O_Weapon
{
    public override void DoAttack()
    {
        transform.right = aimDirection;
        SetColliderEnabled(true);
        rb_Weapon.velocity = aimDirection * weaponData.shootSpeed;
        if (Vector2.Distance(transform.position, lauchPoint) >= weaponData.attackRange) Destroy(this.gameObject);
    }
}
