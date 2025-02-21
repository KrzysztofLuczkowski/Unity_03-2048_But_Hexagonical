using UnityEngine;
using TMPro;
using System.Collections;

public class GameUI : MonoBehaviour
{
    public TMP_Text scoreText;
    public TMP_Text movesText;
    public TMP_Text timerText;
    public GameObject gameOverScreen;
    public GameObject gameUI;
    public AudioSource moveSound; 

    private int score = 0;
    private int moves = 0;
    private float gameTime = 0f;
    private bool isRunning = true;

    void Start()
    {
        StartCoroutine(UpdateTimer());
        UpdateScoreText();
        UpdateMovesText();
        gameOverScreen.SetActive(false);
    }

    IEnumerator UpdateTimer()
    {
        while (isRunning)
        {
            gameTime += Time.deltaTime;
            UpdateTimerText();
            yield return null;
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    public void IncrementMoves()
    {
        moves++;
        UpdateMovesText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Points: {score}";
    }

    private void UpdateMovesText()
    {
        movesText.text = $"Moves: {moves}";
    }

    private void UpdateTimerText()
    {
        timerText.text = GetFormattedTime();
    }

    public string GetFormattedTime()
    {
        int totalSeconds = Mathf.FloorToInt(gameTime);
        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        if (hours > 0)
            return $"{hours}:{minutes:D2}:{seconds:D2}";
        else
            return $"{minutes}:{seconds:D2}";
    }

    public int GetScore()
    {
        return score;
    }

    public int GetMoveCount()
    {
        return moves;
    }

    public void ShowGameOverScreen()
    {
        isRunning = false; // Zatrzymuje timer
        gameUI.SetActive(false);
    }



    public void PlayMoveSound()
    {
        if (moveSound != null)
        {
            moveSound.Play();
        }
    }

}
