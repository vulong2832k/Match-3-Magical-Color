using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public static event Action<int> OnScoreChanged;

    [SerializeField] private int _basePoint = 100;
    private int _score = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddScore(int matchCount)
    {
        int addScore = matchCount * _basePoint;
        _score += addScore;
        OnScoreChanged?.Invoke(_score);
        GameManager.Instance.CheckInstantLevelUp();
    }

    public int GetScore() => _score;
}
