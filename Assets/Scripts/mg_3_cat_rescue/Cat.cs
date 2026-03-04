using UnityEngine;
using UnityEngine.SceneManagement;

public class Cat : MonoBehaviour
{
    public AudioEventChannel canalAudio;
    public AudioClip pickUpCatSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("onTrigger ->" + collision);
        // 1. Detectar si lo que tocó al gato es el jugador
        if (collision.CompareTag("Player"))
        {
            // 2. Unir el gato al transform del jugador (hacerlo su "hijo")
            transform.SetParent(collision.transform);

            // Opcional: Centrar el gato en la posición del jugador
            transform.localPosition = Vector3.zero;

            canalAudio.RaiseSfxEvent(pickUpCatSound);

            // 3. Pasar de nivel
            LevelLoader.Instance.LoadNextLevelWithScore(10);
        }
    }
}
