using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    int levelScore = 5;

    // Esta función se ejecuta automáticamente cuando algo entra en el Trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger->"+other);
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡El jugador ha entrado en el área final!");

            if (LevelLoader.Instance != null)
            {
                LevelLoader.Instance.LoadNextLevelWithScore(levelScore);
            }
        }
    }
}