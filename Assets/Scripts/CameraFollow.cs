using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // Arrastrar aquí al Player
    public float smoothSpeed = 0.125f; // Suavidad del movimiento
    public Vector3 offset;          // Distancia de separación

    void LateUpdate() // LateUpdate se usa para cámaras para evitar tirones
    {
        if (target != null)
        {
            // Calculamos la posición deseada
            // Mantenemos la X de la cámara (fija)
            // Usamos la Y del jugador + el offset
            // Mantenemos la Z de la cámara (-10)
            Vector3 desiredPosition = new Vector3(transform.position.x, target.position.y + offset.y, transform.position.z);

            // Interpolación (movimiento suave) entre posición actual y deseada
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Aplicamos la posición
            transform.position = smoothedPosition;
        }
    }
}