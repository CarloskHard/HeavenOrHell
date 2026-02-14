using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Header("Límites Verticales")]
    public float minY = 0f;      // La cámara no bajará de aquí
    public float maxY = 1000f;   // La cámara no subirá de aquí (pon un número muy alto si es infinito)

    void LateUpdate()
    {
        if (target != null)
        {
            // 1. Calculamos la posición deseada basada en el jugador
            float desiredY = target.position.y + offset.y;

            // 2. Aplicamos el CLAMP para no pasarnos de los límites
            float clampedY = Mathf.Clamp(desiredY, minY, maxY);

            // 3. Creamos el vector final (X fija, Y limitada, Z fija)
            Vector3 desiredPosition = new Vector3(transform.position.x, clampedY, transform.position.z);

            // 4. Suavizado
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        }
    }
}