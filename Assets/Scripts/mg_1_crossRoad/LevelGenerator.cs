using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Tipos de Carril")]
    public List<GameObject> lanePrefabs; // Lista donde meterás carretera, césped, etc.

    [Header("Configuración")]
    public Transform player;
    public int initialLanes = 20;
    public float laneHeight = 1f;

    private float spawnY = 0f;
    private List<GameObject> activeLanes = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < initialLanes; i++)
        {
            // Los primeros carriles siempre son el primero de la lista (césped/seguro)
            if (i < 5) SpawnLane(lanePrefabs[0]);
            else RandomSpawn();
        }
    }

    void Update()
    {
        if (player.position.y + 15 > spawnY)
        {
            RandomSpawn();
            DeleteOldLane();
        }
    }

    void RandomSpawn()
    {
        // Elige cualquier prefab de la lista al azar
        int randomIndex = Random.Range(0, lanePrefabs.Count);
        SpawnLane(lanePrefabs[randomIndex]);
    }

    void SpawnLane(GameObject prefab)
    {
        GameObject lane = Instantiate(prefab, new Vector3(0, spawnY, 0), Quaternion.identity);
        activeLanes.Add(lane);
        spawnY += laneHeight;
    }

    void DeleteOldLane()
    {
        if (activeLanes.Count > 30)
        {
            Destroy(activeLanes[0]);
            activeLanes.RemoveAt(0);
        }
    }
}