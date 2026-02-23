using UnityEngine;

// Asegura que este script esté en el mismo objeto que la cámara
public class CameraFollow : MonoBehaviour
{
    [Header("Objetivo a seguir")]
    public Transform target;

    [Header("Configuración de Cámara")]
    public float smoothSpeed = 0.125f; // ¡Ahora actuará como el tiempo de suavizado del SmoothDamp!
    public Vector3 offset;
    public float minY = 0f;      // El borde inferior de la pantalla no pasará de aquí
    public float maxY = 1000f;   // El borde superior de la pantalla no pasará de aquí

    private float camHalfHeight; // Guardará la mitad de la altura de la cámara

    // NUEVO: Variable necesaria para que el SmoothDamp funcione (almacena la inercia internamente)
    private Vector3 currentVelocity = Vector3.zero;

    void Start()
    {
        // Calculamos la mitad del alto de la cámara (su tamaño ortográfico)
        camHalfHeight = GetComponent<Camera>().orthographicSize;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // 1. Calculamos la posición deseada basada en el jugador
            float desiredY = target.position.y + offset.y;

            // 2. Calculamos los límites reales de la posición (Límite del mundo +/- tamaño de cámara)
            float realMinY = minY + camHalfHeight;
            float realMaxY = maxY - camHalfHeight;

            // (Opcional) Seguridad: Si tus límites son más pequeños que la propia pantalla, lo centramos
            if (realMinY > realMaxY)
            {
                realMinY = (minY + maxY) / 2f;
                realMaxY = realMinY;
            }

            // 3. Aplicamos el CLAMP con los límites corregidos
            float clampedY = Mathf.Clamp(desiredY, realMinY, realMaxY);

            // 4. Creamos el vector final (X fija de la cámara, Y limitada, Z fija de la cámara)
            // Nota: Mantenemos tu lógica de X fija por si no quieres que la cámara se mueva hacia los lados.
            Vector3 desiredPosition = new Vector3(transform.position.x, clampedY, transform.position.z);

            // 5. Suavizado MEJORADO: Cambiamos Lerp por SmoothDamp
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
        }
    }
}