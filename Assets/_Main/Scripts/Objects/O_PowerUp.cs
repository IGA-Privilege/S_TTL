using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_PowerUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<O_Character>(out O_Character character))
        {
            M_RunePower.Instance.ShowThreeRandomRuneUpgrades();
            Destroy(gameObject);
        }
    }
}
