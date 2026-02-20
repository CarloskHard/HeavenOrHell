using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cambiar escenas

public class MainMenuManager : MonoBehaviour
{
    // Referencia al mismo canal
    public AudioEventChannel canalMusica;

    // La canción para ESTE nivel o zona
    public AudioClip musicaDeEsteNivel;
    public AudioClip sfx_btn;

    private void Start()
    {
        // Al iniciar el nivel, pedimos la música
        // No nos importa quién la reproduce, solo mandamos el mensaje al canal
        if (canalMusica != null && musicaDeEsteNivel != null)
        {
            canalMusica.RaiseMusicEvent(musicaDeEsteNivel);
        }
    }

    public void JugarNivel()
    {
        //Sonido botón
        if (canalMusica != null && musicaDeEsteNivel != null)
        {
            canalMusica.RaiseSfxEvent(sfx_btn);
        }

        LevelLoader.Instance.LoadNextLevel();
    }

    public void SalirJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}