using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverChecker : MonoBehaviour
{
    public MoveValidator MoveValidator;
    public GameUI gameUI;
    public GameOverScreen gameOverScreen;
    
    public void CheckGameOver()
    {
        // Sprawdzamy, czy istnieje jakikolwiek mo¿liwy ruch
        if (!MoveValidator.IsAnyMovePossible())
        {
            gameUI.ShowGameOverScreen();
            gameOverScreen.PlayGameOverSound();
            gameOverScreen.ShowGameOverScreen();

        }
    }
}
