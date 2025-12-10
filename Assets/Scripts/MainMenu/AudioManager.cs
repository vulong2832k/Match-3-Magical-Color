using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip swapTileClip;
    public AudioClip completeTileClip;
    public AudioClip levelUpClip;

    [Header("Volumes")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        LoadVolume();
        ApplyVolume();

        if (bgmSource && bgmClip)
        {
            bgmSource.clip = bgmClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    private void LoadVolume()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat("BGM_VOLUME", bgmVolume);
        PlayerPrefs.SetFloat("SFX_VOLUME", sfxVolume);
    }

    private void ApplyVolume()
    {
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        bgmSource.volume = bgmVolume;
        SaveVolume();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        sfxSource.volume = sfxVolume;
        SaveVolume();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip)
            sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlaySwapTile()
    {
        PlaySFX(swapTileClip);
    }

    public void PlayCompleteTile()
    {
        PlaySFX(completeTileClip);
    }

    public void PlayLevelUp()
    {
        PlaySFX(levelUpClip);
    }
}
