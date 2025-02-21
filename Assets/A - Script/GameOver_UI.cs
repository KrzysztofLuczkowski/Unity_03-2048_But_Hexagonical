using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public Button restartButton;
    public Button exitButton;
    public HighScoreManager highScoreManager;
    public AudioSource endSound;

    public GameUI gameUI; // Referencja do GameUI
    

    private void Start()
    {
        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        exitButton.onClick.AddListener(ExitGame);
    }

    public void ShowGameOverScreen()
    {
        
        
        // Pobranie danych po ich zapisaniu w GameUI
        int score = gameUI.GetScore();
        int moves = gameUI.GetMoveCount();
        string formattedTime = gameUI.GetFormattedTime();

        scoreText.text = $"Score {score} in {moves} Moves";
        timeText.text = $"Time: {formattedTime}";

        highScoreManager.CheckAndSaveHighScore();
        gameOverPanel.SetActive(true);
        StartCoroutine(StopTimeAfterFrame()); // Op�niamy zatrzymanie czasu
    }

    private IEnumerator StopTimeAfterFrame()
    {
        yield return null; // Czekamy jedn� klatk�, �eby UI zd��y�o si� od�wie�y�
        Time.timeScale = 0f;
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayGameOverSound()
    {
        if (endSound != null)
        {
            endSound.Play();
        }
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Zatrzymuje gr� w edytorze
#else
        Application.Quit(); // Dzia�a w buildzie
#endif
    }

}
