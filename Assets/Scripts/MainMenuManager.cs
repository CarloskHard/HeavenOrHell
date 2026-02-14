using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar escenas

public class MainMenuManager : MonoBehaviour
{

    public void JugarNivel()
    {
        // Carga la escena siguiente en la lista de Build Settings
        // O podemos poner el nombre exacto: SceneManager.LoadScene("NombreEscena");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}