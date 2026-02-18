using System.Collections; // Necesario para IEnumerator
using TMPro;
using UnityEngine;

public class Mg2Manager : MonoBehaviour
{
    [Header("Dependencias")]
    public LevelLoader levelLoader;
    [SerializeField] private ScoreData scoreData;
    public TextMeshProUGUI textoTiempo;

    [Header("Configuración de Nivel")]
    [Tooltip("Puntuación mínima para poder pasar al siguiente nivel")]
    public int minScore = -10;

    [Tooltip("Puntuación máxima permitida (si te pasas, pierdes)")]
    public int maxScore = 50;

    [Header("Temporizador")]
    public int tiempoInicialSegundos = 10;
    public float retrasoInicioSegundos = 3f;

    private int tiempoRestante;
    private Coroutine cuentaAtrasCoroutine;

    private void Start()
    {
        // Validación de seguridad
        if (scoreData == null)
        {
            Debug.LogError("¡Falta asignar el ScoreData en el inspector!");
            return;
        }

        tiempoRestante = Mathf.Max(0, tiempoInicialSegundos);
        ActualizarUI(tiempoRestante);

        // Iniciamos la cuenta atrás
        cuentaAtrasCoroutine = StartCoroutine(RutinaCuentaAtras());
    }

    // Nombramos los métodos en PascalCase por convención de C#
    private void FinishLevel()
    {
        // Obtenemos la puntuación actual directamente del ScriptableObject
        int currentScore = scoreData.currentScore;

        // Comprobamos condiciones de victoria/derrota
        // NOTA: Revisa si realmente quieres bloquear al jugador si supera el maxScore.
        if (currentScore < minScore || currentScore > maxScore)
        {
            Debug.Log($"Nivel fallido. Puntuación {currentScore} fuera del rango ({minScore}-{maxScore}).");
            // Aquí podrías llamar a un "GameOver" o "RetryLevel"
        }
        else
        {
            Debug.Log("Nivel superado. Cargando siguiente escena...");
            // NO sumamos la puntuación aquí, porque ya se sumó cuando ganaste los puntos.
            levelLoader.LoadNextLevel();
        }
    }

    private IEnumerator RutinaCuentaAtras()
    {
        // 1. Espera inicial (Retraso)
        if (retrasoInicioSegundos > 0f)
            yield return new WaitForSeconds(retrasoInicioSegundos);

        // 2. Bucle de cuenta atrás
        while (tiempoRestante > 0)
        {
            ActualizarUI(tiempoRestante);

            // Esperamos 1 segundo real
            yield return new WaitForSeconds(1f);

            tiempoRestante--;
        }

        // 3. Finalización
        ActualizarUI(0);
        FinishLevel();
    }

    private void ActualizarUI(int tiempo)
    {
        if (textoTiempo != null)
        {
            // SetText es más eficiente en memoria que .text = .ToString()
            // {0} es el marcador de posición para el número
            textoTiempo.SetText("{0}", tiempo);
        }
    }

    // Opcional: Si el jugador termina el nivel ANTES del tiempo (ej. llega a la meta)
    // puedes llamar a este método público desde otro script.
    public void ForceFinishLevel()
    {
        if (cuentaAtrasCoroutine != null) StopCoroutine(cuentaAtrasCoroutine);
        FinishLevel();
    }
}