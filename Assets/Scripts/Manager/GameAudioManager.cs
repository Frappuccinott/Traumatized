using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class GameAudioManager : MonoBehaviour
{
    public static GameAudioManager Instance;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Music")]
    [SerializeField] private AudioClip loopMusic;

    [Header("UI / SFX")]
    [SerializeField] private AudioClip uiClick;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private Coroutine miniGameFadeRoutine;

    private const string MUSIC_PARAM = "Music";
    private const string SFX_PARAM = "SFX";
    private const string MUSIC_PREF = "MusicVolume";
    private const string SFX_PREF = "SFXVolume";
    private const float DEFAULT_VOLUME = 0.75f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CreateSources();
        LoadVolumes();
        StartMusic();
    }

    private void CreateSources()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.ignoreListenerPause = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;
        sfxSource.playOnAwake = false;
        sfxSource.ignoreListenerPause = true;
    }

    private void StartMusic()
    {
        if (loopMusic == null) return;
        musicSource.clip = loopMusic;
        musicSource.Play();
    }

    public void PauseMusicForCutscene()
    {
        if (musicSource.isPlaying) musicSource.Pause();
    }

    public void ResumeMusicAfterCutscene()
    {
        if (!musicSource.isPlaying) musicSource.UnPause();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUIClick()
    {
        PlaySFX(uiClick);
    }

    public void PlayMiniGameSFX(AudioClip clip)
    {
        if (clip == null) return;

        StopMiniGameFade();
        sfxSource.clip = clip;
        sfxSource.volume = 1f;
        sfxSource.Play();
    }

    public void StopMiniGameSFX(float fadeDuration = 1f)
    {
        if (!sfxSource.isPlaying) return;

        StopMiniGameFade();
        miniGameFadeRoutine = StartCoroutine(FadeOutMiniGameSFX(fadeDuration));
    }

    private IEnumerator FadeOutMiniGameSFX(float duration)
    {
        float startVolume = sfxSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            sfxSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        sfxSource.Stop();
        sfxSource.volume = startVolume;
    }

    private void StopMiniGameFade()
    {
        if (miniGameFadeRoutine != null)
        {
            StopCoroutine(miniGameFadeRoutine);
            miniGameFadeRoutine = null;
        }
    }

    public void SetMusicVolume(float value) => SetMixer(MUSIC_PARAM, MUSIC_PREF, value);

    public void SetSFXVolume(float value) => SetMixer(SFX_PARAM, SFX_PREF, value);

    private void SetMixer(string param, string pref, float value)
    {
        float db = value <= 0f ? -80f : Mathf.Log10(value) * 20f;
        mixer.SetFloat(param, db);
        PlayerPrefs.SetFloat(pref, value);
    }

    private void LoadVolumes()
    {
        SetMusicVolume(PlayerPrefs.GetFloat(MUSIC_PREF, DEFAULT_VOLUME));
        SetSFXVolume(PlayerPrefs.GetFloat(SFX_PREF, DEFAULT_VOLUME));
    }
}