using UnityEngine;

public class MusicStarter : MonoBehaviour
{
    // Referencia al mismo canal
    public AudioEventChannel canalMusica;

    // La canción que quieres para ESTE nivel o zona
    public AudioClip musicaDeEsteNivel;

    private void Start()
    {
        // Al iniciar el nivel, pedimos la música
        // No nos importa quién la reproduce, solo mandamos el mensaje al canal
        if (canalMusica != null && musicaDeEsteNivel != null)
        {
            canalMusica.RaiseMusicEvent(musicaDeEsteNivel);
        }
    }
}