using System.Collections;
using TMPro;
using UnityEngine;

public class KarmaBarAnimator : MonoBehaviour
{
    public RectTransform playerIcon;
    public ScoreData scoreData;
    public AudioEventChannel canalDeAudio;
    public float minKarmaScore = -100f;
    public float maxKarmaScore = 100f;
    public float minPositionY = -200f;
    public float maxPositionY = 200f;
    public float animationDuration = 4f; // Te recomiendo 3 o 4 segundos para crear tensión
    public float suspenseSpeed = 20f;     // Velocidad a la que sube y baja (nerviosismo)
    public float suspenseMagnitude = 250f; // Distancia máxima de los saltos falsos
    public AudioClip drumrollClip;
    public AudioClip finalScoreClip;
    public TextMeshProUGUI txtScroe;

    private void Start()
    {
        // El icono empieza en el centro
        playerIcon.anchoredPosition = new Vector2(playerIcon.anchoredPosition.x, 0f);
        txtScroe.text = "";
        StartCoroutine(AnimateKarmaSequence());
    }

    private IEnumerator AnimateKarmaSequence()
    {
        yield return new WaitForSeconds(0.5f);

        // --- EMPIEZA EL REDOBLE ---
        if (canalDeAudio != null && drumrollClip != null)
        {
            canalDeAudio.RaiseSfxEvent(drumrollClip);
        }

        // --- CALCULAR EL DESTINO REAL ---
        int currentScore = scoreData.totalScore;
        float clampedScore = Mathf.Clamp(currentScore, minKarmaScore, maxKarmaScore);
        float scorePercentage = Mathf.InverseLerp(minKarmaScore, maxKarmaScore, clampedScore);
        float targetY = Mathf.Lerp(minPositionY, maxPositionY, scorePercentage);

        // --- ANIMACIÓN CON SUSPENSE ---
        float timer = 0f;
        float startY = 0f;

        // Generamos una "semilla" aleatoria para que el patrón de saltos sea distinto en cada partida
        float randomSeed = Random.Range(0f, 100f);

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / animationDuration;

            // 1. Calculamos la trayectoria base (hacia donde debería ir suavemente)
            float smoothProgress = progress * progress * (3f - 2f * progress);
            float baseY = Mathf.Lerp(startY, targetY, smoothProgress);

            // 2. CREAMOS EL CAOS (Subidas y bajadas aleatorias)
            // Mathf.Sin hace que vaya de arriba a abajo continuamente.
            float swingDirection = Mathf.Sin(timer * suspenseSpeed);
            // PerlinNoise hace que la fuerza de esas subidas/bajadas sea impredecible.
            float randomIntensity = Mathf.PerlinNoise(timer * (suspenseSpeed * 0.2f), randomSeed);
            float noise = swingDirection * randomIntensity;

            // 3. AMORTIGUACIÓN (Dampening)
            // A medida que 'progress' se acerca a 1 (final), el caos se multiplica por 0 y desaparece.
            // Usamos Pow(..., 0.5f) para que el caos dure un poco más antes de frenar de golpe.
            float dampening = 1f - Mathf.Pow(progress, 0.5f);

            // 4. APLICAMOS EL MOVIMIENTO
            float currentY = baseY + (noise * suspenseMagnitude * dampening);

            // Limitamos la posición para que en un salto muy loco no se salga de la barra visualmente
            currentY = Mathf.Clamp(currentY, minPositionY, maxPositionY);

            playerIcon.anchoredPosition = new Vector2(playerIcon.anchoredPosition.x, currentY);

            yield return null;
        }

        // --- FINALIZAR ---
        // Nos aseguramos de que aterrice EXACTAMENTE en la puntuación real al terminar
        playerIcon.anchoredPosition = new Vector2(playerIcon.anchoredPosition.x, targetY);

        txtScroe.text = $"{currentScore}";

        // Suena el platillo / sonido de éxito final
        if (canalDeAudio != null && finalScoreClip != null)
        {
            canalDeAudio.RaiseSfxEvent(finalScoreClip);
        }
    }
}