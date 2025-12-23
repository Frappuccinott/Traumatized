using UnityEngine;
using UnityEngine.Audio;

public class AnaMenuAudioManager : MonoBehaviour
{
    public static AnaMenuAudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;

    [Header("UI SFX")]
    [SerializeField] private AudioClip uiClickSound;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private const string MUSIC_PARAM = "Music";
    private const string SFX_PARAM = "SFX";
    private const string MUSIC_PREF = "MusicVolume";
    private const string SFX_PREF = "SFXVolume";
    private const float DEFAULT_VOLUME = 0.75f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CreateSources();
    }

    private void Start()
    {
        LoadAudioSettings();
        PlayMainMenuMusic();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void CreateSources()
    {
        musicSource = CreateAudioSource(musicGroup, true);
        sfxSource = CreateAudioSource(sfxGroup, false);
    }

    private AudioSource CreateAudioSource(AudioMixerGroup group, bool loop)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = group;
        source.playOnAwake = false;
        source.loop = loop;
        return source;
    }

    private void PlayMainMenuMusic()
    {
        if (!mainMenuMusic) return;
        musicSource.clip = mainMenuMusic;
        musicSource.Play();
    }

    public void PlayUIClick() => PlaySFX(uiClickSound);

    private void PlaySFX(AudioClip clip)
    {
        if (clip) sfxSource.PlayOneShot(clip);
    }

    public void SetMusicVolume(float volume) => SetVolume(MUSIC_PARAM, MUSIC_PREF, volume);

    public void SetSFXVolume(float volume) => SetVolume(SFX_PARAM, SFX_PREF, volume);

    private void SetVolume(string param, string pref, float volume)
    {
        float db = volume > 0 ? Mathf.Log10(volume) * 20f : -80f;
        mixer.SetFloat(param, db);
        PlayerPrefs.SetFloat(pref, volume);
    }

    private void LoadAudioSettings()
    {
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_PREF, DEFAULT_VOLUME));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_PREF, DEFAULT_VOLUME));
    }

    public void CleanupBeforeExit()
    {
        if (musicSource != null) musicSource.Stop();
        if (Instance == this) Instance = null;
        Destroy(gameObject);
    }
}