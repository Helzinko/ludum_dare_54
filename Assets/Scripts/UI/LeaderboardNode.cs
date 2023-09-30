using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderboardNode : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI place;
    [SerializeField] private TextMeshProUGUI nickname;
    [SerializeField] private TextMeshProUGUI score;

    public void SetupNode(string place, string name, string score)
    {
        this.place.text = place;
        this.nickname.text = name;
        this.score.text = score;
    }

    public void ChangeTextColor(Color color)
    {
        place.color = color;
        nickname.color = color;
        score.color = color;
    }
}