using TMPro;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Game/ScoreData")]
public class ScoreData : ScriptableObject
{
    // El valor actual (se mantiene entre escenas mientras el juego corre)
    public int currentScore;

    // Evento para avisar a la UI cuando cambie
    public UnityAction<int> OnScoreChanged;

    public void AddScore(int amount)
    {
        currentScore += amount;
        // Invocamos el evento si alguien está escuchando
        OnScoreChanged?.Invoke(currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    // Opcional: Resetear al iniciar el juego en el editor
    private void OnEnable()
    {
        currentScore = 0;
    }
}