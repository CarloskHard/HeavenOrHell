using UnityEngine;
using UnityEngine.SceneManagement; // Para reiniciar

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Maleta")) // Asegúrate de crear el Tag "Maleta"
        {
            Debug.Log("¡Se cayó una maleta! Fin del juego.");
            // Aquí recargas la escena o muestras menú
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}