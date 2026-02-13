using UnityEngine;

public class Car : MonoBehaviour
{
    public float speed = 5f;
    private float screenLimit = 12f; // Distancia a la que el coche se destruye

    void Update()
    {
        // Se mueve siempre hacia adelante (derecha local)
        transform.Translate(Vector3.right * speed * Time.deltaTime);

        // Si se aleja mucho del centro, se destruye para limpiar memoria
        if (Mathf.Abs(transform.position.x) > screenLimit)
        {
            Destroy(gameObject);
        }
    }
}