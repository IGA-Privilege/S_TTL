using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Weapon : Singleton<M_Weapon>
{

    public Transform weaponPivotsParent;
    public Data_Weapon initialWeapon;
    public int initialWeaponCount;
    public float radius;

    public List<Vector3> oddsPivots;
    public List<Vector3> evenPivots;

    private int currentWeaponCount = 0;
    private List<Transform> weaponPivots = new List<Transform>();


    void Start()
    {
        UpdateWeaponPivotPosition();
        for (int i = 0; i < initialWeaponCount; i++)
        {
            CreateSpaceForWeapon();
            CreateNewWeapon();
        }

    }

    void UpdateWeaponPivotPosition()
    {
        int targetOddsNum = 8;
        List<Vector3> _oddsPivots = new List<Vector3>();
        for (int i = 0; i < targetOddsNum; i++)
        {
            float angle = 360 / targetOddsNum * i + 90;
            float xPos = radius * Mathf.Cos(angle * Mathf.PI / 180);
            float yPos = radius * Mathf.Sin(angle * Mathf.PI / 180);
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
            float xPos = radius * Mathf.Cos(angle * Mathf.PI / 180);
            float yPos = radius * Mathf.Sin(angle * Mathf.PI / 180);
            _evensPivots.Add(new Vector3(xPos, yPos, 0));
        }
        evenPivots.Add(_evensPivots[1]);
        evenPivots.Add(_evensPivots[2]);
        evenPivots.Add(_evensPivots[3]);
        evenPivots.Add(_evensPivots[4]);
    }

    public void CreateSpaceForWeapon()
    {
        currentWeaponCount++;
        weaponPivots.Add(new GameObject("Weapon " + (currentWeaponCount + 1)).transform);
        weaponPivots[currentWeaponCount - 1].SetParent(weaponPivotsParent);
        switch (currentWeaponCount)
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

    public void CreateNewWeapon()
    {
        GameObject go = Instantiate(initialWeapon.obj_Weapon, weaponPivots[currentWeaponCount - 1].position, Quaternion.identity);
        O_Weapon newWeapon = go.GetComponent<O_Weapon>();
        newWeapon.InitializeWeapon(currentWeaponCount - 1, initialWeapon);
        
    }

    public Transform GetWeaponPivotPos(int searchIndex)
    {
        return weaponPivots[searchIndex];
    }
}
