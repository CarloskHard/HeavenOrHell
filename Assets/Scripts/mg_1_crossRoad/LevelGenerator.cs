using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tilemapSuelo;

    [Header("Tipos de Carril")]
    public List<GameObject> lanePrefabs; // Lista de filas horizontales: carretera, césped, etc.

    [Header("Configuración")]
    public Transform player;
    public int initialLanes = 20;
    public int laneHeight = 1;

    private int spawnY = 0;
    private List<GameObject> activeLanes = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < initialLanes; i++)
        {
            // Los primeros carriles siempre son el primero de la lista, que debería ser acera (Espacio seguro en spawn)
            if (i < 3) SpawnLane(lanePrefabs[0]);
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
        int randomIndex = Random.Range(0, lanePrefabs.Count);
        SpawnLane(lanePrefabs[randomIndex]);
    }

    void SpawnLane(GameObject prefab)
    {
        GameObject newLance = Instantiate(prefab, new Vector3(0, spawnY, 0), Quaternion.identity);

        // Hacemos que sea hijo del generador solo por tener orden
        newLance.transform.SetParent(this.transform);

        // Pintar los tiles
        RowConfig config = newLance.GetComponent<RowConfig>();
        if (config != null)
        {
            config.PintarFila(tilemapSuelo, (int)spawnY);
        }

        activeLanes.Add(newLance);
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