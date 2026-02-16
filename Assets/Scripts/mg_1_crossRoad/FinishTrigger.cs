using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public LevelLoader levelLoader;

    // Esta función se ejecuta automáticamente cuando algo entra en el Trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger->"+other);
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡El jugador ha entrado en el área final!");

            if (levelLoader != null)
            {
                levelLoader.LoadNextLevel();
            }
        }
    }
}