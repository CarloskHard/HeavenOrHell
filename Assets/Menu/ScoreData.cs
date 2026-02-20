using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Game/ScoreData")]
public class ScoreData : ScriptableObject
{
    public int totalScore;

    void Start()
    {
        ResetScore();
    }

    // Reseteamos solo al empezar una partida nueva completa
    public void ResetScore()
    {
        totalScore = 0;
    }

    // Método simple para confirmar la suma
    public void CommitScore(int amountToAdd)
    {
        totalScore += amountToAdd;
    }
}