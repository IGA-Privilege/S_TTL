using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditorInternal;
using UnityEngine;

public class M_Weapon : Singleton<M_Weapon>
{

    public Transform weaponPivotsParent;
    public Data_Weapon originalSpearData;
    public Data_Weapon originalShieldData;
    public float weaponPivotRadius;
    [HideInInspector] public Camera playerCamera;
    public ParticleSystem shieldBatterEffect;

    public List<Vector3> oddsPivots;
    public List<Vector3> evenPivots;

    public Dictionary<RunePower, bool> runeActivationDic = new Dictionary<RunePower, bool>();
    private List<Transform> weaponPivots = new List<Transform>();

    private Vector2 aimDirection;
    [HideInInspector] public Data_Weapon _spearData;
    [HideInInspector] public Data_Weapon _shieldData;
    private O_Spear _spearOnHand;
    private int _spearFireCount = 1;
    private float _spearFireTimer = 0f;
    private float _avengerSpearFireTimer = 0f;
    private float _avengerSpearCD = 0.2f;
    private int _scatteredSpearCounter = 0;
    private O_Shield _shieldOnHand;
    private float _shieldBatterTimer = 0f;
    private float _avengerBatterTimer = 0f;
    private float _avengerBatterCD = 0.5f;
    private int _batterChargedLevel = 0;


    private void Awake()
    {
        shieldBatterEffect.Stop();
        playerCamera = Camera.main;
        CopyWeaponsData();
        InitializeRuneDictionary();
    }

    private void CopyWeaponsData()
    {
        _spearData = new Data_Weapon();
        _spearData.attackType = originalSpearData.attackType;
        _spearData.attackRange = originalSpearData.attackRange;
        _spearData.obj_Weapon = originalSpearData.obj_Weapon;
        _spearData.coolDown = originalSpearData.coolDown;
        _spearData.damageAmount = originalSpearData.damageAmount;
        _spearData.shootSpeed = originalSpearData.shootSpeed;

        _shieldData = new Data_Weapon();
        _shieldData.attackType = originalShieldData.attackType;
        _shieldData.attackRange = originalShieldData.attackRange;
        _shieldData.obj_Weapon = originalShieldData.obj_Weapon;
        _shieldData.coolDown = originalShieldData.coolDown;
        _shieldData.damageAmount = originalShieldData.damageAmount;
        _shieldData.shootSpeed = originalShieldData.shootSpeed;
    }

    private void InitializeRuneDictionary()
    {
        runeActivationDic.Clear();
        runeActivationDic.Add(RunePower.ScatteredSpear, false);
        runeActivationDic.Add(RunePower.ImpactSpear, false);
        runeActivationDic.Add(RunePower.HeavySpear, false);
        runeActivationDic.Add(RunePower.StrongSpear, false);
        runeActivationDic.Add(RunePower.ExtraSpear, false);
        runeActivationDic.Add(RunePower.SwiftSpear, false);
        runeActivationDic.Add(RunePower.AvengerSpear, false);
        runeActivationDic.Add(RunePower.RoundBatter, false);
        runeActivationDic.Add(RunePower.AvengerBatter, false);
        runeActivationDic.Add(RunePower.ChargedBatter, false);
        runeActivationDic.Add(RunePower.StrongBatter, false);
        runeActivationDic.Add(RunePower.SwiftBatter, false);
        runeActivationDic.Add(RunePower.OffensiveDash, false);
        runeActivationDic.Add(RunePower.MagneticDash, false);
        runeActivationDic.Add(RunePower.BlurDash, false);
        runeActivationDic.Add(RunePower.StrongDash, false);
        runeActivationDic.Add(RunePower.LethalDash, false);
        runeActivationDic.Add(RunePower.HealingDash, false);
        runeActivationDic.Add(RunePower.SwiftDash, false);
        runeActivationDic.Add(RunePower.ImpactDash, false);
    }

    void Start()
    {
        GenerateAllPivotsPosition();
        GenerateSpearPivots();
        _spearOnHand = SpawnNewWeapon(_spearData, transform.position, Vector2.zero).GetComponent<O_Spear>();
        _shieldOnHand = SpawnNewWeapon(_shieldData, transform.position, Vector2.zero).GetComponent<O_Shield>();
    }

    private void GenerateSpearPivots()
    {
        float maxSpearCount = 4;
        for (int i = 0; i < maxSpearCount; i++)
        {
            weaponPivots.Add(new GameObject("Weapon " + (_spearFireCount + 1)).transform);
            weaponPivots[i].SetParent(weaponPivotsParent);
        }
    }

    private void Update()
    {
        if (runeActivationDic[RunePower.AvengerSpear])
        {
            _avengerSpearFireTimer += Time.deltaTime;
        }
        if (runeActivationDic[RunePower.AvengerBatter])
        {
            _avengerBatterTimer += Time.deltaTime;
        }
        UpdateAimDirection();
        UpdateSpearOnHand();
        TickSpearFire();
        UpdateShieldOnHand();
        TickShieldBatter();
        DebugInput();
    }

    private void UpdateAimDirection()
    {
        Vector2 mousePos = playerCamera.ScreenToWorldPoint(Input.mousePosition);
        aimDirection = (mousePos - (Vector2)transform.position).normalized;
        weaponPivotsParent.right = -aimDirection;
    }

    private void UpdateSpearOnHand()
    {
        _spearOnHand.currentState = WeaponState.ReadyForAttack;
        _spearOnHand.transform.position = transform.position + new Vector3(aimDirection.x, aimDirection.y, 0);
        _spearOnHand.transform.right = aimDirection;
    }

    private void TickShieldBatter()
    {
        _shieldBatterTimer += Time.deltaTime;
        if (_shieldBatterTimer > _shieldData.coolDown)
        {
            DoShieldBatter();
        }
    }

    public void DoShieldBatter()
    {
        StartCoroutine(PlayShieldBatterEffect());
        _shieldOnHand.currentState = WeaponState.Attacking;
        _shieldBatterTimer = 0f;
        if (runeActivationDic[RunePower.RoundBatter])
        {
            ShieldRoundBatter();
        }
    }

    private IEnumerator PlayShieldBatterEffect()
    {
        yield return new WaitForSeconds(0.1f);
        shieldBatterEffect.Play();
    }

    private void UpdateShieldOnHand()
    {
        if (_shieldOnHand.currentState == WeaponState.ReadyForAttack)
        {
            _shieldOnHand.transform.position = transform.position + new Vector3(aimDirection.x, aimDirection.y, 0);
            _shieldOnHand.transform.right = transform.right;
            shieldBatterEffect.transform.position = transform.position + new Vector3(aimDirection.x, aimDirection.y, 0);
            shieldBatterEffect.transform.up = aimDirection;
        }
        _shieldOnHand.aimDirection = aimDirection;
    }

    private void ShieldRoundBatter()
    {
        int fireAmount = 12;
        for (int i = 0; i < fireAmount; i++)
        {
            float angle = 360 / fireAmount * i + Vector2.Angle(transform.right, aimDirection);
            float xPos = weaponPivotRadius * Mathf.Cos(angle * Mathf.PI / 180);
            float yPos = weaponPivotRadius * Mathf.Sin(angle * Mathf.PI / 180);
            Vector3 firePoint = new Vector3(transform.position.x + xPos, transform.position.y + yPos, 0);
            O_Shield shield = SpawnNewWeapon(_shieldData, firePoint, (firePoint - transform.position).normalized).GetComponent<O_Shield>();
            shield.SetOneOff(true);
        }
    }

    private void DebugInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            runeActivationDic[RunePower.SwiftDash] = true;
            Debug.Log(RunePower.OffensiveDash.ToString() + "is On!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            runeActivationDic[RunePower.BlurDash] = true;
            Debug.Log(RunePower.AvengerBatter.ToString() + "is On!");
        }

        UpdateRunePowerInfluences();
    }

    private void UpdateRunePowerInfluences()
    {
        float fixedSpearDamage = originalSpearData.damageAmount;
        float fixedSpearRange = originalSpearData.attackRange;
        float fixedBatterDamage = originalShieldData.damageAmount;
        float fixedDashDamage = O_Character.Instance.originalDashDamage;

        if (runeActivationDic[RunePower.HeavySpear])
        {
            fixedSpearRange *= 0.6f;
            fixedSpearDamage *= 2;
        }
        if (runeActivationDic[RunePower.StrongSpear])
        {
            fixedSpearDamage *= 1.2f;
        }
        if (runeActivationDic[RunePower.SwiftSpear])
        {
            _spearData.coolDown = originalSpearData.coolDown * 0.9f;
        }
        if (runeActivationDic[RunePower.StrongBatter])
        {
            fixedBatterDamage *= 1.3f;
        }
        if (runeActivationDic[RunePower.SwiftBatter])
        {
            _shieldData.coolDown = originalShieldData.coolDown - 1;
        }
        if (runeActivationDic[RunePower.StrongDash])
        {
            fixedDashDamage *= 1.1f;
        }
        if (runeActivationDic[RunePower.LethalDash])
        {
            fixedDashDamage *= 1.3f;
        }
        if (runeActivationDic[RunePower.SwiftDash])
        {
            O_Character.Instance.dashCoolDown = O_Character.Instance.originalDashCoolDown - 2;
        }

        _spearData.damageAmount = fixedSpearDamage;
        _spearData.attackRange = fixedSpearRange;
        _shieldData.damageAmount = fixedBatterDamage;
        O_Character.Instance.dashDamage = fixedDashDamage;
    }

    public void SetShieldBatterCharged(bool isCharged)
    {
        if (runeActivationDic[RunePower.ChargedBatter])
        {
            if (isCharged)
            {
                if (_batterChargedLevel < 2)
                {
                    _batterChargedLevel++;
                    _shieldData.damageAmount *= 2;
                }
            }
            else
            {
                _batterChargedLevel = 0;
                _shieldData.damageAmount = originalShieldData.damageAmount;
            }
        }
    }

    private void TickSpearFire()
    {
        _spearFireTimer += Time.deltaTime;
        // 冷却结束时，进行一轮射击
        if (_spearFireTimer > _spearData.coolDown)
        {
            // 发射前修正
            if (runeActivationDic[RunePower.ScatteredSpear])
            {
                if (_scatteredSpearCounter > 2)
                {
                    _spearFireCount += 2;
                }
            }
            if (runeActivationDic[RunePower.ExtraSpear])
            {
                _spearFireCount++;
            }
            // 根据发射数量安排投射物的初始位置
            SortWeaponFireSpaces();
            // 发射长矛
            for (int i = 0;i < _spearFireCount;i++)
            {
                SpawnNewWeapon(_spearData, i);
            }
            _spearFireTimer = 0;
            // 发射后修正
            if (runeActivationDic[RunePower.ScatteredSpear])
            {
                if (_scatteredSpearCounter > 2)
                {
                    _spearFireCount -= 2;
                    _scatteredSpearCounter = 0;
                }
                else
                {
                    _scatteredSpearCounter++;
                }
            }
            if (runeActivationDic[RunePower.ExtraSpear])
            {
                _spearFireCount--;
            }
        }

    }

    public void DoAvengeActions(BaseEnemy damageSource)
    {
        if (runeActivationDic[RunePower.AvengerSpear])
        {
            if (damageSource != null)
            {
                if (_avengerSpearFireTimer > _avengerSpearCD)
                {
                    Vector2 direction = damageSource.transform.position - transform.position;
                    SpawnNewWeapon(_spearData, transform.position, direction.normalized);
                }
                _avengerSpearFireTimer = 0;
            }
        }

        if (runeActivationDic[RunePower.AvengerBatter])
        {
            if (_avengerBatterTimer > _avengerBatterCD)
            {
                _shieldData.damageAmount *= 1.6f;
                DoShieldBatter();
                _shieldData.damageAmount = _shieldData.damageAmount / 1.6f;
                _avengerBatterTimer = 0;
            }
        }
    }

    private void GenerateAllPivotsPosition()
    {
        int targetOddsNum = 8;
        List<Vector3> _oddsPivots = new List<Vector3>();
        for (int i = 0; i < targetOddsNum; i++)
        {
            float angle = 360 / targetOddsNum * i + 90;
            float xPos = weaponPivotRadius * Mathf.Cos(angle * Mathf.PI / 180);
            float yPos = weaponPivotRadius * Mathf.Sin(angle * Mathf.PI / 180);
            _oddsPivots.Add(new Vector3(xPos, yPos, 0));
        }
        oddsPivots.Add(_oddsPivots[1]);
        oddsPivots.Add(_oddsPivots[2]);
        oddsPivots.Add(_oddsPivots[3]);

        int targetEvensNum = 10;
        List<Vector3> _evensPivots = new List<Vector3>();
        for (int i = 0; i < targetEvensNum; i++)
        {
            float angle = 360 / targetEvensNum * i + 90;
            float xPos = weaponPivotRadius * Mathf.Cos(angle * Mathf.PI / 180);
            float yPos = weaponPivotRadius * Mathf.Sin(angle * Mathf.PI / 180);
            _evensPivots.Add(new Vector3(xPos, yPos, 0));
        }
        evenPivots.Add(_evensPivots[1]);
        evenPivots.Add(_evensPivots[2]);
        evenPivots.Add(_evensPivots[3]);
        evenPivots.Add(_evensPivots[4]);
    }

    private void SortWeaponFireSpaces()
    {
        switch (_spearFireCount)
        {
            case 1:
                weaponPivots[0].localPosition = oddsPivots[1];
                break;
            case 2:
                weaponPivots[0].localPosition = evenPivots[1];
                weaponPivots[1].localPosition = evenPivots[2];
                break;
            case 3:
                weaponPivots[0].localPosition = oddsPivots[0];
                weaponPivots[1].localPosition = oddsPivots[1];
                weaponPivots[2].localPosition = oddsPivots[2];
                break;
            case 4:
                weaponPivots[0].localPosition = evenPivots[0];
                weaponPivots[1].localPosition = evenPivots[1];
                weaponPivots[2].localPosition = evenPivots[2];
                weaponPivots[3].localPosition = evenPivots[3];
                break;
        }
    }


    public void SpawnNewWeapon(Data_Weapon weaponData, int pivotIndex)
    {
        GameObject weaponObject = Instantiate(weaponData.obj_Weapon, weaponPivots[pivotIndex].position, Quaternion.identity);
        O_Weapon newWeapon = weaponObject.GetComponent<O_Weapon>();
        Vector2 direction = weaponPivots[pivotIndex].position - transform.position;
        newWeapon.InitializeWeapon(GetWeaponPivotPos(pivotIndex).position, weaponData, direction.normalized);
    }

    public GameObject SpawnNewWeapon(Data_Weapon weaponData, Vector3 launchPoint, Vector2 flyDirection)
    {
        GameObject weaponObject = Instantiate(weaponData.obj_Weapon, launchPoint, Quaternion.identity);
        O_Weapon newWeapon = weaponObject.GetComponent<O_Weapon>();
        newWeapon.InitializeWeapon(launchPoint, weaponData, flyDirection.normalized);
        return weaponObject;
    }


    public Transform GetWeaponPivotPos(int searchIndex)
    {
        return weaponPivots[searchIndex];
    }
}

public enum RunePower
{
    ScatteredSpear, 
    ImpactSpear, 
    HeavySpear, 
    StrongSpear, 
    ExtraSpear, 
    SwiftSpear, 
    AvengerSpear, 
    RoundBatter, 
    AvengerBatter,
    ChargedBatter,
    StrongBatter,
    SwiftBatter,
    OffensiveDash,
    MagneticDash,
    BlurDash,
    StrongDash,
    LethalDash,
    HealingDash,
    SwiftDash,
    ImpactDash,
    None
}
