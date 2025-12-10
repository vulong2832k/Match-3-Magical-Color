using UnityEngine;
using UnityEngine.UI;

public class StartButtonUI : MonoBehaviour
{
    [SerializeField] private Button _startButton;

    private void Start()
    {
        _startButton.onClick.AddListener(OnStartClicked);
    }

    private void OnStartClicked()
    {
        GameManager.Instance.StartGame();
        gameObject.SetActive(false);
    }
}
