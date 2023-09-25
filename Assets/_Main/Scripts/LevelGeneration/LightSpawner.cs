using UnityEngine;

public class LightSpawner : MonoBehaviour
{
    [HideInInspector] public Transform centerPoint;
    public Transform lightsParent;
    public MovingLight lightPrefab;

    public int light_count = 0;
    public int light_limit = 30;
    public int lights_per_frame = 1;

    public float spawn_circle_radius = 80.0f;
    public float death_circle_radius = 90.0f;

    public float fastest_speed = 12.0f;
    public float slowest_speed = 0.75f;

    void Start()
    {
        centerPoint = FindObjectOfType<O_Character>().transform;
        InitialPopulation();
        transform.position = new Vector3(centerPoint.position.x, centerPoint.position.y, transform.position.z);
    }

    private void Update()
    {
        MaintainPopulation();
    }

    private void InitialPopulation()
    {
        for (int i = 0; i < light_limit; i++)
        {
            Vector3 position = GetRandomPosition(false);
            SpawnNewLight(position);
        }
    }

    private void MaintainPopulation()
    {
        if (light_count < light_limit)
        {
            for (int i = 0; i < lights_per_frame; i++)
            {
                Vector3 position = GetRandomPosition(false);
                SpawnNewLight(position);
            }
        }
    }

    private Vector3 GetRandomPosition(bool within_camera)
    {
        Vector3 position = Random.insideUnitCircle;

        if (within_camera == false)
        {
            position = position.normalized;
        }

        position *= spawn_circle_radius;
        position += centerPoint.transform.position;

        position.z = 0.5f;

        return position;
    }

    private MovingLight SpawnNewLight(Vector3 position)
    {
        light_count += 1;
        Vector3 flyDirection = new Vector3((centerPoint.transform.position - position).x, (centerPoint.transform.position - position).y, 0f);
        MovingLight newLight = Instantiate(lightPrefab, position, Quaternion.FromToRotation(Vector3.up, flyDirection), centerPoint);

        MovingLight light = newLight.GetComponent<MovingLight>();
        light.spawner = this;
        light.centerPoint = centerPoint;
        light.speed = Random.Range(slowest_speed, fastest_speed);

        light.transform.SetParent(lightsParent, true);

        return light;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, spawn_circle_radius);
    }
}
