using UnityEngine;
using UnityEngine.UI;

public class VolumePanelUI : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private GameObject _volumepanel;
    [SerializeField] private Button _exitBtn;

    private void Start()
    {
        _bgmSlider.value = AudioManager.Instance.bgmVolume;
        _sfxSlider.value = AudioManager.Instance.sfxVolume;

        _bgmSlider.onValueChanged.AddListener((v) =>
        {
            AudioManager.Instance.SetBGMVolume(v);
        });

        _sfxSlider.onValueChanged.AddListener((v) =>
        {
            AudioManager.Instance.SetSFXVolume(v);
        });

        _volumepanel.SetActive(false);
    }

    public void ShowPanel()
    {
        _volumepanel.SetActive(true);
    }

    public void HidePanel()
    {
        _volumepanel.SetActive(false);
    }
    public void ExitPanel()
    {
        _volumepanel?.SetActive(false);
    }
}
