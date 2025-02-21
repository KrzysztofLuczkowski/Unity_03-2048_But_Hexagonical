using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HighScoreManager : MonoBehaviour
{
    private const string HighScoreKey = "HighScore";

    public TMP_Text topScoreText;
    public GameUI gameUI;

    private int highScore;

    void Start()
    {
        LoadHighScore();
        UpdateTopScoreUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash)) // "\" na klawiaturze
        {
            ResetHighScore();
        }
    }

    public void CheckAndSaveHighScore()
    {
        int currentScore = gameUI.GetScore();
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
            UpdateTopScoreUI();
        }
    }

    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    private void UpdateTopScoreUI()
    {
        topScoreText.text = $"Top Score: {highScore}";
    }

    private void ResetHighScore()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
