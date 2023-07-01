using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_Push : O_Weapon
{
    public override void DoAttack()
    {
        ChangeColliderStateTo(true);
        rb_Weapon.velocity = aimDirection * weaponData.shootSpeed;
        if (Vector2.Distance(transform.position, lauchPoint.position) >= weaponData.attackRange) ExitAttack();
    }
}
