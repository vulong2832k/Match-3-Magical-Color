using UnityEngine;

public enum GameState
{
    Idle,
    Playing,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Level Settings")]
    [SerializeField] private int _currentLevel = 1;
    [SerializeField] private float _multiplier = 1f;
    [SerializeField] private float _levelDuration = 180f;

    private float _timer = 0;
    private int _requiredScore = 0;

    public int CurrentLevel => _currentLevel;
    public int RequiredScore => _requiredScore;
    public float TimeLeft => _timer;

    [Header("State: ")]
    public GameState currentState = GameState.Idle;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartLevel();

        currentState = GameState.Idle;
        Time.timeScale = 0;
    }

    private void Update()
    {
        if (currentState != GameState.Playing)
            return;

        if (_timer > 0)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0)
            {
                CheckLevelSuccess();
            }
        }
    }
    public void StartGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1;

        _timer = _levelDuration;
        FindFirstObjectByType<BoardManager>().StartGame();
    }
    private void StartLevel()
    {
        _requiredScore = Mathf.RoundToInt(_currentLevel * _multiplier * 2000f);
        _timer = _levelDuration;
    }

    private void CheckLevelSuccess()
    {
        int currentScore = ScoreManager.Instance.GetScore();

        if (currentScore < _requiredScore)
        {
            LoseGame();
        }
        else
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        _currentLevel++;

        if (_currentLevel % 5 == 0)
        {
            _multiplier += 0.2f;
        }
        AudioManager.Instance.PlaySFX(AudioManager.Instance.levelUpClip);
        StartLevel();
        FindFirstObjectByType<ProgressBarUI>().ResetProgress();
    }
    public void CheckInstantLevelUp()
    {
        if (currentState != GameState.Playing) return;

        int currentScore = ScoreManager.Instance.GetScore();
        if (currentScore >= _requiredScore)
        {
            LevelUp();
        }
    }
    private void LoseGame()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0;
    }
}
