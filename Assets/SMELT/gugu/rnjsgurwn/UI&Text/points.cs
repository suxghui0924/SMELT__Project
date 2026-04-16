using JetBrains.Annotations;
using NUnit.Framework.Interfaces;
using TMPro;
using UnityEngine;

public class Scoremanager : MonoBehaviour
{
    [SerializeField] private int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int cost = 10; 

    private void Start()
    {
        UpdateScore();
    }

    public void AddScore(int value)
    {
        score += value;
        UpdateScore();
    }

    private void UpdateScore()
    {
        scoreText.text = $"Last Point: {score}";
    }
    public bool UseScore(int cost)
    {
        if (score >= cost)
        {
            score -= cost;
            scoreText.text = $"Last Point: {score}";
            return true; // 성공
        }
        else
        {
            Debug.Log("점수 부족!");
            return false; // 실패
        }
    }

}
