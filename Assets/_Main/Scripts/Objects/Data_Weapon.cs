using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Weapon Data",menuName = "Scriptable Object/Weapon Data")]
public class Data_Weapon : ScriptableObject
{
    public WeaponType attackType;
    public float coolDown;
    public float attackRange;
    public int damageAmount;
    public float shootSpeed;
    public GameObject obj_Weapon;
}

public enum WeaponType { Push, Puush, Push180, Puuush, Slap, MultiSlap, BigSlap }