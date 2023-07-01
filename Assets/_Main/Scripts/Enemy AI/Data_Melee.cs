using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Enemy Data", menuName = "Scriptable Object/Enemy/New Melee Data")]
public class Data_Melee : Data_Enemy
{
    [Header("Melee Attributes")]
    public float damageAmount;
}
