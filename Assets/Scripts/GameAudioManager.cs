using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance { get; private set; }

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Music")]
    [SerializeField] private AudioClip[] musicClips;

    [Header("UI SFX")]
    [SerializeField] private AudioClip uiClickSound;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private Coroutine musicRoutine;
    private int currentMusicIndex = 0;

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
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void Start() => LoadAudioSettings();

    private void OnDestroy()
    {
        if (musicRoutine != null) StopCoroutine(musicRoutine);
        if (Instance == this) Instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "AnaMenu") StartMusic();
    }

    private void CreateSources()
    {
        musicSource = CreateAudioSource(musicGroup, false);
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

    private void StartMusic()
    {
        if (musicRoutine != null) StopCoroutine(musicRoutine);

        if (musicClips == null || musicClips.Length == 0) return;

        currentMusicIndex = 0;
        musicRoutine = StartCoroutine(MusicRoutine());
    }

    private IEnumerator MusicRoutine()
    {
        while (true)
        {
            AudioClip clip = musicClips[currentMusicIndex];

            musicSource.clip = clip;
            musicSource.Play();

            yield return new WaitForSeconds(clip.length);

            currentMusicIndex = (currentMusicIndex + 1) % musicClips.Length;
        }
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
}