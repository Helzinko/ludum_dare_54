using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class HUD : MonoBehaviour
{
    [SerializeField] private Transform fillHolder;

    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private GameObject scoreIndicationText;
    [SerializeField] private GameObject highscoreText;

    [SerializeField] private float scorePunchSize = 1.5f;
    [SerializeField] private float scorePunchDuration = 1f;

    [SerializeField] private Image leftFill;
    [SerializeField] private Image rightFill;

    [SerializeField] private Color comboColor;

    private Tween scoreScaleTween;
    private Tween fillScaleTween;

    private void Start()
    {
        leftFill.fillAmount = 0.5f;
        rightFill.fillAmount = 0.5f;
    }

    public void UpdateBonusScoreFill(float value)
    {
        leftFill.fillAmount = value;
        rightFill.fillAmount = value;

        if (scoreText.color != Color.white && value <= 0)
            scoreText.color = Color.white;
    }

    public void UpdateScore(int score)
    {
        if (scoreScaleTween != null || fillScaleTween != null)
        {
            scoreScaleTween.Complete();
            fillScaleTween.Complete();
        }

        scoreScaleTween = scoreText.transform.DOPunchScale(new Vector3(scorePunchSize, scorePunchSize), scorePunchDuration, 0);
        fillScaleTween = fillHolder.DOPunchScale(new Vector3(scorePunchSize, 0), scorePunchDuration, 0);

        scoreText.text = score.ToString();

        leftFill.fillAmount = 1f;
        rightFill.fillAmount = 1f;
    }

    public void UpdateScoreIndicationPosition(Vector2 pos, int scoreToAdd, bool speedBonus, int comboBonus)
    {
        var viewportPos = Camera.main.WorldToViewportPoint(pos);

        var currentScoreText = Instantiate(scoreIndicationText, transform.position, default, transform);
        currentScoreText.GetComponent<TextMeshProUGUI>().text = "+" + scoreToAdd.ToString()
            + (speedBonus ? Environment.NewLine + "x2 Speed bonus" : "") + (comboBonus >= 5 ? Environment.NewLine + "x2 " + comboBonus + " Combo Bonus" : "");
        currentScoreText.GetComponent<RectTransform>().anchorMin = viewportPos;
        currentScoreText.GetComponent<RectTransform>().anchorMax = viewportPos;
        currentScoreText.transform.DOMoveY(currentScoreText.transform.position.y + 100f, 1.75f).SetEase(Ease.OutQuad);
        currentScoreText.GetComponent<TextMeshProUGUI>().DOFade(0, 2f).OnComplete(() => Destroy(currentScoreText));

        if (comboBonus >= 5)
            scoreText.color = comboColor;
    }

    public void UpdateHighscorePosition(Vector2 pos)
    {
        var viewportPos = Camera.main.WorldToViewportPoint(pos);

        var currentHighscore = Instantiate(highscoreText, transform.position, default, transform);
        currentHighscore.GetComponent<RectTransform>().anchorMin = viewportPos;
        currentHighscore.GetComponent<RectTransform>().anchorMax = viewportPos;

        currentHighscore.transform.DOMoveY(currentHighscore.transform.position.y + 150f, 4f).SetEase(Ease.OutQuad);

        currentHighscore.GetComponent<TextMeshProUGUI>().DOFade(0, 5f).OnComplete(() => Destroy(currentHighscore));
    }
}