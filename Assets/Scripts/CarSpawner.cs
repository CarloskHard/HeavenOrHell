using System.Collections.Generic;
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

    private bool isBlocked = false;
    private List<Car> activeCars = new List<Car>(); // Lista para rastrear los coches de esta línea

    void Start()
    {
        cam = Camera.main;
        moveRight = (Random.value > 0.5f);
        Invoke("Spawn", Random.Range(minDelay, maxDelay));
    }

    void Spawn()
    {
        if (isBlocked) return; // Si el carril está bloqueado, no nacen más coches

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

            // Hacer que el coche sea hijo del carril para organizarlos
            car.transform.SetParent(transform);
            activeCars.Add(carScript);
        }

        // Repetir el ciclo
        Invoke("Spawn", Random.Range(minDelay, maxDelay));
    }

    // Esta función la llamará el Player al chocar
    public void StopLane()
    {
        isBlocked = true;
        CancelInvoke("Spawn"); // Detenemos la creación de futuros coches

        // Detenemos todos los coches que ya están en este carril
        foreach (Car car in activeCars)
        {
            if (car != null)
            {
                car.speed = 0;
            }
        }
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

    void Update()
    {
        // Solo limpiamos si hay coches activos
        if (activeCars.Count > 0)
        {
            CleanUpOutOfBoundsCars();
        }
    }

    void CleanUpOutOfBoundsCars()
    {
        // Calculamos los bordes actuales de la pantalla
        // 0 es izquierda, 1 es derecha en Viewport
        float leftEdge = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        float rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;

        // Añadimos un margen extra para que no desaparezcan justo en el borde
        float margin = 2f;

        // Recorremos la lista al revés (importante al eliminar elementos de una lista)
        for (int i = activeCars.Count - 1; i >= 0; i--)
        {
            Car currentCar = activeCars[i];

            if (currentCar == null)
            {
                activeCars.RemoveAt(i);
                continue;
            }

            // Lógica de destrucción según dirección
            bool outOfBounds = false;

            if (moveRight && currentCar.transform.position.x > rightEdge + margin)
            {
                outOfBounds = true;
            }
            else if (!moveRight && currentCar.transform.position.x < leftEdge - margin)
            {
                outOfBounds = true;
            }

            if (outOfBounds)
            {
                GameObject carToDestroy = currentCar.gameObject;
                activeCars.RemoveAt(i);
                Destroy(carToDestroy);
            }
        }
    }

    // Función para que los coches se quiten de la lista al destruirse
    public void RemoveCarFromList(Car car)
    {
        activeCars.Remove(car);
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