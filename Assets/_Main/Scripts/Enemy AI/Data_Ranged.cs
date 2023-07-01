using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged Enemy Data", menuName = "Scriptable Object/Enemy/New Ranged Data")]
public class Data_Ranged : Data_Enemy
{
    [Header("Ranged Attributes")]
    public float shootRange;
    public int bulletCount;
    public float fireRate;
    public float bulletSpeed;
    public int bulletDamage;
    public float projectileRangeGap;
    public AudioClip bulletAudio;
    public GameObject bulletPrefab;
}
