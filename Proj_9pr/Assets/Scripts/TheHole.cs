using UnityEngine;
using System.Collections.Generic;

public class TheHole : MonoBehaviour
{
    [Header("Object Spawn Settings")]
    [SerializeField] private List<GameObject> prefabs;
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private float minSpawnInterval = 1f;
    [SerializeField] private float maxSpawnInterval = 5f;

    [Header("Object Lifetime Settings")]
    [SerializeField] private float objectLifetime = 10f;
    [SerializeField] private int maxObjects = 10;

    private Queue<GameObject> spawnedObjects = new Queue<GameObject>();
    private float nextSpawnTime;

    void Start()
    {
        ScheduleNextSpawn();
    }

    void Update()
    {
        HandleSpawning();
        HandleObjectLifetime();
    }

    private void HandleSpawning()
    {
        if (Time.time >= nextSpawnTime && prefabs.Count > 0)
        {
            SpawnObject();
            ScheduleNextSpawn();
        }
    }

    private void HandleObjectLifetime()
    {
        while (spawnedObjects.Count > 0)
        {
            GameObject oldestObject = spawnedObjects.Peek();
            if (oldestObject == null || Time.time - oldestObject.GetComponent<ObjectData>().SpawnTime >= objectLifetime || spawnedObjects.Count > maxObjects)
            {
                GameObject toDestroy = spawnedObjects.Dequeue();
                if (toDestroy != null)
                {
                    Destroy(toDestroy);
                }
            }
            else
            {
                break;
            }
        }
    }

    private void SpawnObject()
    {
        int randomIndex = Random.Range(0, prefabs.Count);
        GameObject prefab = prefabs[randomIndex];

        Vector3 randomPosition = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPosition.y = transform.position.y;

        GameObject newObject = Instantiate(prefab, randomPosition, Quaternion.identity);
        newObject.AddComponent<ObjectData>().SpawnTime = Time.time;

        spawnedObjects.Enqueue(newObject);
    }

    private void ScheduleNextSpawn()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
public class ObjectData : MonoBehaviour
{
    public float SpawnTime { get; set; }
}