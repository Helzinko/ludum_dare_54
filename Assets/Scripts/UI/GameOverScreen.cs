using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highscoreTExt;
    public void UpdateScore(int score, int highscore)
    {
        scoreText.text = "Score: " + Environment.NewLine + score.ToString();
        highscoreTExt.text = "Highscore: " + Environment.NewLine + highscore.ToString();
    }
}