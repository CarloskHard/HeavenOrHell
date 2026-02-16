using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [Header("Animación")]
    public Animator transition;
    public float transitionTime = 1f;
    public string nombreSiguienteEscena;

    [Header("Sonidos")]
    public AudioEventChannel canalAudio;
    public AudioClip sonidoBoton;
    public AudioClip sonidoTransicion;

    public void LoadNextLevel()
    {
        if (sonidoTransicion) canalAudio.RaiseSfxEvent(sonidoTransicion);

        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        // Play animation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load Scene
        SceneManager.LoadScene(levelIndex);
    }
}