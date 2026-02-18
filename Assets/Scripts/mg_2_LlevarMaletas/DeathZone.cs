using UnityEngine;

public class DeathZone : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Maleta"))
        {
            // Destruir la maleta que entra en la zona
            Destroy(other.gameObject);

            Debug.Log("¡Se cayó una maleta! Fin del juego.");
            LevelLoader.Instance.LoadNextLevelWithScore(-5);
        }
    }
}