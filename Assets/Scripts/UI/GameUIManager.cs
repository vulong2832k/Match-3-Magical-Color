using UnityEngine;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance;

    [Header("UI Text")]
    [SerializeField] private TMP_Text _levelText;
    [SerializeField] private TMP_Text _requiredScoreText;
    [SerializeField] private TMP_Text _timerText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        _levelText.text = "" + GameManager.Instance.CurrentLevel;

        _requiredScoreText.text = "" + GameManager.Instance.RequiredScore;

        float time = GameManager.Instance.TimeLeft;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        _timerText.text = $"{minutes:00} : {seconds:00}";
    }
}
