using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MUSIC_PREF = "MusicVolume";
    private const string SFX_PREF = "SFXVolume";
    private const string MUSIC_PARAM = "Music";
    private const string SFX_PARAM = "SFX";
    private const float DEFAULT_VOLUME = 0.75f;

    private void Start()
    {
        if (musicSlider) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        LoadAudioSettings();
    }

    public void SetMusicVolume(float value)
    {
        float dbValue = VolumeToDecibel(value);
        audioMixer.SetFloat(MUSIC_PARAM, dbValue);
        PlayerPrefs.SetFloat(MUSIC_PREF, value);
    }

    public void SetSFXVolume(float value)
    {
        float dbValue = VolumeToDecibel(value);
        audioMixer.SetFloat(SFX_PARAM, dbValue);
        PlayerPrefs.SetFloat(SFX_PREF, value);
    }

    private void LoadAudioSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_PREF, DEFAULT_VOLUME);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_PREF, DEFAULT_VOLUME);

        if (musicSlider) musicSlider.value = musicVolume;
        if (sfxSlider) sfxSlider.value = sfxVolume;

        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    public void ResetToDefaults()
    {
        if (musicSlider)
        {
            musicSlider.value = DEFAULT_VOLUME;
            SetMusicVolume(DEFAULT_VOLUME);
        }

        if (sfxSlider)
        {
            sfxSlider.value = DEFAULT_VOLUME;
            SetSFXVolume(DEFAULT_VOLUME);
        }
    }

    private float VolumeToDecibel(float volume)
    {
        // 0-1 arasý deðeri -80 ile 0 dB arasýna çevirir
        return volume > 0 ? Mathf.Log10(volume) * 20f : -80f;
    }

    private void OnDestroy()
    {
        if (musicSlider) musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (sfxSlider) sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }
}