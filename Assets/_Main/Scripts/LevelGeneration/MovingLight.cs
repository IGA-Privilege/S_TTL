using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingLight : MonoBehaviour
{
    [HideInInspector] public LightSpawner spawner;
    [HideInInspector] public Transform centerPoint;
    [HideInInspector] public float speed;

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position += transform.up * (Time.deltaTime * speed);

        float distance = Vector3.Distance(transform.position, centerPoint.transform.position);
        if (distance > spawner.death_circle_radius)
        {
            RemoveShip();
        }
    }

    private void RemoveShip()
    {
        Destroy(gameObject);
        spawner.light_count -= 1;
    }
}
