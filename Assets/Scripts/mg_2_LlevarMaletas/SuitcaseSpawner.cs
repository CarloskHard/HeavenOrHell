using UnityEngine;

public class SuitcaseSpawner : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject maletaPrefab;
    public Transform handPlatform; // Referencia para saber la altura base si no hay maletas
    public float distanciaSobreLaMasAlta = 4f; // "X distancia" sobre la torre
    public float rangoX = 2f; // Variación horizontal
    public float tiempoEntreSpawns = 3f;

    private float temporizador;

    void Update()
    {
        temporizador -= Time.deltaTime;

        if (temporizador <= 0)
        {
            SpawnMaleta();
            temporizador = tiempoEntreSpawns;
        }
    }

    void SpawnMaleta()
    {
        // 1. Calcular la altura de spawn
        float alturaSpawn = ObtenerAlturaObjetivo();

        // 2. Calcular posición horizontal aleatoria
        float randomX = Random.Range(-rangoX, rangoX);

        // 3. Crear el vector de posición
        // Usamos la X aleatoria y la Y calculada
        Vector3 spawnPos = new Vector3(randomX, alturaSpawn, 0);

        Instantiate(maletaPrefab, spawnPos, Quaternion.identity);
    }

    float ObtenerAlturaObjetivo()
    {
        // Buscamos todas las maletas
        GameObject[] maletas = GameObject.FindGameObjectsWithTag("Maleta");

        // Si no hay maletas, usamos la altura de las manos como base
        if (maletas.Length == 0)
        {
            // Si handPlatform no está asignada, usa 0 por defecto
            return (handPlatform != null ? handPlatform.position.y : 0) + distanciaSobreLaMasAlta;
        }

        float maxY = -Mathf.Infinity;

        // Buscamos la más alta
        foreach (GameObject maleta in maletas)
        {
            // Verificamos que no sea null (por si se destruyó justo ahora)
            if (maleta != null)
            {
                if (maleta.transform.position.y > maxY)
                {
                    maxY = maleta.transform.position.y;
                }
            }
        }

        // Devolvemos la altura de la maleta más alta + la distancia deseada
        return maxY + distanciaSobreLaMasAlta;
    }

    // Dibujo visual en el editor para ver dónde aparecerá la próxima (aprox)
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return; // Solo dibujar cuando jugamos para no confundir

        Gizmos.color = Color.cyan;
        float y = ObtenerAlturaObjetivo();
        Gizmos.DrawLine(new Vector3(-5, y, 0), new Vector3(5, y, 0));
        Gizmos.DrawWireSphere(new Vector3(0, y, 0), 0.5f);
    }
}