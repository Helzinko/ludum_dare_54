using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    public HUD hud;
    public GameOverScreen gameOverScreen;
    public SelectionScreen evilSelectionScreen;

    public void HudActive(bool active)
    {
        hud.GetComponent<CanvasGroup>().DOFade(active ? 1 : 0, 0.3f);
    }

    public void GameOverScreenActive(bool active)
    {
        gameOverScreen.GetComponent<CanvasGroup>().DOFade(active ? 1 : 0, 0.3f);
    }
}