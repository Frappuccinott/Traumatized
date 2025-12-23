using UnityEngine;
using UnityEngine.Video;
using System;

public class CutscenePlayer : MonoBehaviour
{
    [Header("Video Settings")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject videoPanel;

    private Action onVideoEndCallback;

    private void Awake()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }

        if (videoPanel != null)
        {
            videoPanel.SetActive(false);
        }
    }

    public void PlayCutscene(Action onEnd)
    {
        onVideoEndCallback = onEnd;

        // 🔇 Müziği duraklat
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.PauseMusicForCutscene();
        }

        if (videoPanel != null)
        {
            videoPanel.SetActive(true);
        }

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.Play();
        }
        else
        {
            OnVideoEnd(null);
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (videoPanel != null)
        {
            videoPanel.SetActive(false);
        }

        // 🔊 Müziği devam ettir
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.ResumeMusicAfterCutscene();
        }

        onVideoEndCallback?.Invoke();
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}
