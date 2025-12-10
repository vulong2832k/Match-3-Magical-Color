using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private Image progressFill;

    private void Update()
    {
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        int currentScore = ScoreManager.Instance.GetScore();
        int required = GameManager.Instance.RequiredScore;

        float percent = Mathf.Clamp01((float)currentScore / required);

        progressFill.fillAmount = percent;
    }
    public void ResetProgress()
    {
        progressFill.fillAmount = 0f;
    }
}
