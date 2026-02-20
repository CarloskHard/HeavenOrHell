using UnityEngine;

public class EndSceneManager : MonoBehaviour
{
    public void OnClickRestart()
    {
        // Comprobamos que el LevelLoader existe y le decimos que reinicie
        if (LevelLoader.Instance != null)
        {
            LevelLoader.Instance.RestartGame();
        }
        else
        {
            Debug.LogWarning("No se encontró el LevelLoader. Asegúrate de empezar el juego desde la primera escena.");
        }
    }
}
