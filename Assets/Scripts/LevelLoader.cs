using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader Instance;

    [Header("Datos Globales")]
    public ScoreData scoreData;

    [Header("Transición Estándar (Círculo)")]
    public GameObject canvasTransition;
    public Animator transitionAnimator;
    public float transitionTime = 1f;

    [Header("Transición Puntuación")]
    public GameObject canvasScore;
    public TextMeshProUGUI scoreText;
    public Animator scoreAnimator;
    public float scoreCountDuration = 2f;
    public float scoreExitDuration = 1f;

    [Header("Audio Puntuación")]
    public AudioEventChannel canalMusica;
    public AudioClip scoreUpClip;
    public AudioClip scoreDownClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ... (El resto de métodos LoadNextLevel, ScoreSequence, etc. siguen igual) ...
    // Solo copio los métodos que no cambian para dar contexto, pero el importante es el de abajo.

    private IEnumerator LoadLevelRoutine()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
            scoreData.ResetScore();
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(nextSceneIndex);
        while (!operation.isDone) yield return null;
    }

    public void LoadNextLevelWithScore(int scoreEarnedInLevel)
    {
        StartCoroutine(ScoreSequence(scoreEarnedInLevel));
    }

    private IEnumerator ScoreSequence(int earnedScore)
    {
        canvasScore.SetActive(true);
        if (scoreAnimator) scoreAnimator.SetTrigger("Enter");

        int oldTotal = scoreData.totalScore;
        int newTotal = oldTotal + earnedScore;

        // Aquí llamamos a la animación de números modificada
        yield return StartCoroutine(AnimateNumbers(oldTotal, newTotal));
        yield return new WaitForSeconds(1.5f);

        scoreData.CommitScore(earnedScore);

        yield return StartCoroutine(LoadLevelRoutine());

        if (scoreAnimator) scoreAnimator.SetTrigger("Exit");

        yield return new WaitForSeconds(scoreExitDuration);
        canvasScore.SetActive(false);
    }

    public void LoadNextLevel()
    {
        StartCoroutine(StandardSequence());
    }

    private IEnumerator StandardSequence()
    {
        canvasTransition.SetActive(true);
        transitionAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        yield return StartCoroutine(LoadLevelRoutine());
        transitionAnimator.SetTrigger("End");
    }

    // ========================================================================
    // UTILIDADES UI
    // ========================================================================
    private IEnumerator AnimateNumbers(int start, int end)
    {
        float timer = 0f;
        scoreText.text = $"Total: {start}";

        // Guardamos el último valor mostrado para comparar
        int previousDisplay = start;

        while (timer < scoreCountDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / scoreCountDuration;

            // Calculamos el valor actual
            int currentDisplay = (int)Mathf.Lerp(start, end, progress);

            // Actualizamos el texto
            scoreText.text = $"Total: {currentDisplay}";

            // LÓGICA DE SONIDO
            // Si el número que mostramos es diferente al del frame anterior...
            if (currentDisplay != previousDisplay)
            {
                // Verificamos si sube o baja para elegir el clip
                if (currentDisplay > previousDisplay && scoreUpClip != null)
                {
                    // Usamos PlayOneShot para permitir superposición rápida si cuenta muy rápido
                    canalMusica.RaiseSfxEvent(scoreUpClip);
                }
                else if (currentDisplay < previousDisplay && scoreDownClip != null)
                {
                    canalMusica.RaiseSfxEvent(scoreDownClip);
                }

                // Actualizamos el "anterior" para la siguiente vuelta
                previousDisplay = currentDisplay;
            }

            yield return null;
        }

        // Aseguramos que el valor final es exacto y suena una última vez si faltó
        scoreText.text = $"Total: {end}";
        if (previousDisplay != end)
        {
            if (end > previousDisplay && scoreUpClip != null) canalMusica.RaiseSfxEvent(scoreUpClip);
            else if (end < previousDisplay && scoreDownClip != null) canalMusica.RaiseSfxEvent(scoreDownClip);
        }
    }
}