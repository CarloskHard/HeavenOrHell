using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public float minDelay = 1.5f;
    public float maxDelay = 3.5f;
    public float carSpeed = 5f;
    public bool moveRight = true;

    void Start()
    {
        // Empezar el ciclo de generación
        Invoke("Spawn", Random.Range(minDelay, maxDelay));
    }

    void Spawn()
    {
        GameObject car = Instantiate(carPrefab, transform.position, Quaternion.identity);
        Car carScript = car.GetComponent<Car>();

        if (carScript != null)
        {
            // Elegir una skin aleatoria entre las disponibles en el script del coche
            int randomSkin = Random.Range(0, carScript.skins.Length);

            // Configurar dirección y apariencia
            carScript.speed = carSpeed;
            carScript.Initialize(moveRight, randomSkin);
        }

        Invoke("Spawn", Random.Range(minDelay, maxDelay));
    }


    // Esto SOLO se ejecuta dentro del Editor de Unity
    void OnDrawGizmos()
    {
        // 1. Configurar dirección y distancia
        // Usamos una distancia de 15 o la que tú quieras para que cubra la pantalla
        float distance = 15f;
        Vector3 direction = moveRight ? Vector3.right : Vector3.left;
        Vector3 endPoint = transform.position + direction * distance;

        // 2. Dibujar la línea de trayectoria
        // Verde si va a la derecha, Rojo si va a la izquierda
        Gizmos.color = moveRight ? Color.green : Color.red;
        Gizmos.DrawLine(transform.position, endPoint);

        // 3. Dibujar una flecha al final de la línea para saber hacia dónde va
        Gizmos.DrawSphere(endPoint, 0.2f);

        // 4. Dibujar un "Coche" esquemático (Cuerpo + Cabina)
        Gizmos.color = Color.yellow; // Color del Gizmo del coche

        // Cuerpo del coche
        Vector3 bodySize = new Vector3(1.2f, 0.6f, 0.1f);
        Gizmos.DrawWireCube(transform.position, bodySize);

        // Cabina (un cuadradito encima o delante para indicar el frente)
        Vector3 cabinOffset = direction * 0.3f;
        Vector3 cabinSize = new Vector3(0.4f, 0.4f, 0.1f);
        Gizmos.DrawIcon(transform.position, "car_icon.png", true);
    }
}