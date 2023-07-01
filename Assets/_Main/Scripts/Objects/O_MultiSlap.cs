using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class O_MultiSlap : O_Weapon
{
    private bool isEntered = false;
    private bool isMultiAttacking = false;
    private bool isMultiEnterd = false;
    private Vector2 startPos;
    private Vector2 endPos;

    public override void DoAttack()
    {
        if (!isEntered)
        {
            startPos = transform.position;
            endPos = startPos + aimDirection * weaponData.attackRange;
            isEntered = true;
        }

        if (!isMultiAttacking)
        {
            if (Vector2.Distance(transform.position, endPos) > 0.3f)
            {
                rb_Weapon.velocity = aimDirection * weaponData.shootSpeed;
                Debug.Log("moving");
            }
            else isMultiAttacking = true;
        }
        else
        {
            rb_Weapon.velocity = Vector2.zero;
            if(!isMultiEnterd)  MultiSlapAction();
        }



    }

    public void MultiSlapAction()
    {
        isMultiEnterd = true;
        Sequence s = DOTween.Sequence();
        s.Append(transform.DOMoveY(transform.position.y + 1, 0.3f));
        s.Append(transform.DOMoveY(transform.position.y - 1, 0.1f));
        s.AppendCallback(() => ChangeColliderStateTo(true));
        s.AppendInterval(0.2f);
        s.AppendCallback(() => ChangeColliderStateTo(false));
        s.Append(transform.DOMoveY(transform.position.y + 1, 0.3f));
        s.Append(transform.DOMoveY(transform.position.y - 1, 0.1f));
        s.AppendCallback(() => ChangeColliderStateTo(true));
        s.AppendInterval(0.2f);
        s.AppendCallback(() => ChangeColliderStateTo(false));
        s.Append(transform.DOMoveY(transform.position.y + 1, 0.3f));
        s.Append(transform.DOMoveY(transform.position.y - 1, 0.1f));
        s.AppendCallback(() => ChangeColliderStateTo(true));
        s.AppendInterval(0.2f);
        s.AppendCallback(() => ChangeColliderStateTo(false));
        s.AppendCallback(() => ExitAttack());
        s.AppendCallback(() => isMultiAttacking = false);
        s.AppendCallback(() => isEntered = false);
        s.AppendCallback(() => isMultiEnterd = false);
    }
}
