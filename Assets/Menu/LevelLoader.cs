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

    // VARIABLE DE CONTROL AÑADIDA
    private bool isTransitioning = false;

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

    // ---------------------------------------------------------
    // ---------------------------------------------------------
    public void LoadNextLevelWithScore(int scoreEarnedInLevel)
    {
        // Si ya estamos transicionando, no hacemos nada
        if (isTransitioning) return;

        // Bloqueamos
        isTransitioning = true;
        StartCoroutine(ScoreSequence(scoreEarnedInLevel));
    }

    private IEnumerator ScoreSequence(int earnedScore)
    {
        canvasScore.SetActive(true);
        if (scoreAnimator) scoreAnimator.SetTrigger("Enter");

        int oldTotal = scoreData.totalScore;
        int newTotal = oldTotal + earnedScore;

        yield return StartCoroutine(AnimateNumbers(oldTotal, newTotal));
        yield return new WaitForSeconds(1.5f);

        scoreData.CommitScore(earnedScore);

        yield return StartCoroutine(LoadLevelRoutine());

        if (scoreAnimator) scoreAnimator.SetTrigger("Exit");

        yield return new WaitForSeconds(scoreExitDuration);
        canvasScore.SetActive(false);

        // DESBLOQUEAMOS al terminar toda la secuencia
        isTransitioning = false;
    }

    // ---------------------------------------------------------
    // ---------------------------------------------------------
    public void LoadNextLevel()
    {
        // Si ya estamos transicionando, no hacemos nada
        if (isTransitioning) return;

        // Bloqueamos
        isTransitioning = true;
        StartCoroutine(StandardSequence());
    }

    private IEnumerator StandardSequence()
    {
        canvasTransition.SetActive(true);
        transitionAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        yield return StartCoroutine(LoadLevelRoutine());
        transitionAnimator.SetTrigger("End");

        // Opcional: Esperar a que termine la animación de entrada (End) antes de desbloquear
        // yield return new WaitForSeconds(transitionTime); 

        // DESBLOQUEAMOS para permitir futuras cargas en el siguiente nivel
        isTransitioning = false;
    }

    // ========================================================================
    // Secuencia de Reinicio (Vuelta a la escena 0)
    // ========================================================================
    public void RestartGame()
    {
        if (isTransitioning) return;
        isTransitioning = true;
        StartCoroutine(RestartSequence());
    }

    private IEnumerator RestartSequence()
    {
        // 1. Empezamos la transición del círculo
        canvasTransition.SetActive(true);
        transitionAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);

        // 2. Reseteamos los puntos a 0
        if (scoreData != null)
        {
            scoreData.ResetScore();
        }

        // 3. Cargamos la primera escena (Índice 0 en Build Settings)
        AsyncOperation operation = SceneManager.LoadSceneAsync(0);
        while (!operation.isDone) yield return null;

        // 4. Terminamos la transición
        transitionAnimator.SetTrigger("End");
        isTransitioning = false;
    }

    // ========================================================================
    // UTILIDADES UI
    // ========================================================================
    private IEnumerator AnimateNumbers(int start, int end)
    {
        // Debug.Log($"Animando números de {start} a {end}");
        float timer = 0f;
        scoreText.text = $"Total: {start}";

        int previousDisplay = start;

        while (timer < scoreCountDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / scoreCountDuration;

            int currentDisplay = (int)Mathf.Lerp(start, end, progress);

            scoreText.text = $"Total: {currentDisplay}";

            if (currentDisplay != previousDisplay)
            {
                if (currentDisplay > previousDisplay && scoreUpClip != null)
                {
                    canalMusica.RaiseSfxEvent(scoreUpClip);
                }
                else if (currentDisplay < previousDisplay && scoreDownClip != null)
                {
                    canalMusica.RaiseSfxEvent(scoreDownClip);
                }
                previousDisplay = currentDisplay;
            }

            yield return null;
        }

        scoreText.text = $"Total: {end}";
        if (previousDisplay != end)
        {
            if (end > previousDisplay && scoreUpClip != null) canalMusica.RaiseSfxEvent(scoreUpClip);
            else if (end < previousDisplay && scoreDownClip != null) canalMusica.RaiseSfxEvent(scoreDownClip);
        }
    }
}