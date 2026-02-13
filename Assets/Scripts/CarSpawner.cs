using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public float minDelay = 1.5f;
    public float maxDelay = 3.5f;
    public float carSpeed = 5f;
    public bool moveRight = true;

    [Tooltip("Margen extra fuera de la pantalla para que el coche no aparezca de golpe")]
    public float screenMargin = 1.5f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        moveRight = (Random.value > 0.5f);
        Invoke("Spawn", Random.Range(minDelay, maxDelay));
    }

    void Spawn()
    {
        // 1. Calculamos la posición X dinámica
        float spawnX = CalculateSpawnX();

        // 2. La posición Y es la del carril actual
        Vector3 spawnPos = new Vector3(spawnX, transform.position.y, 0);

        // 3. Crear el coche
        GameObject car = Instantiate(carPrefab, spawnPos, Quaternion.identity);

        Car carScript = car.GetComponent<Car>();
        if (carScript != null)
        {
            int randomSkin = Random.Range(0, carScript.skins.Length);
            carScript.speed = carSpeed;
            carScript.Initialize(moveRight, randomSkin);
        }

        // Repetir el ciclo
        Invoke("Spawn", Random.Range(minDelay, maxDelay));
    }

    float CalculateSpawnX()
    {
        // Viewport: 0 es izquierda, 1 es derecha.
        // Si va a la derecha (moveRight), nace a la izquierda (Viewport 0).
        // Si va a la izquierda (!moveRight), nace a la derecha (Viewport 1).
        float viewportX = moveRight ? 0 : 1;

        // Convertimos ese punto 0 o 1 a coordenadas del mundo (X de Unity)
        Vector3 edgePoint = cam.ViewportToWorldPoint(new Vector3(viewportX, 0, cam.nearClipPlane));

        // Le aplicamos el margen para que aparezca un poco más afuera
        if (moveRight)
            return edgePoint.x - screenMargin; // Un poco a la izquierda del borde izq
        else
            return edgePoint.x + screenMargin; // Un poco a la derecha del borde der
    }

    // Actualizamos el Gizmo para que también sea dinámico en el Editor
    void OnDrawGizmos()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        float spawnX = CalculateSpawnX();
        Vector3 spawnPos = new Vector3(spawnX, transform.position.y, 0);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(spawnPos, new Vector3(0, transform.position.y, 0));
        Gizmos.DrawWireCube(spawnPos, new Vector3(1.2f, 0.6f, 0.1f));
    }
}