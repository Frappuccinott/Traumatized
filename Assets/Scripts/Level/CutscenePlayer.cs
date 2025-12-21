using UnityEngine;
using UnityEngine.Video;
using System;

/// <summary>
/// MP4 video cutscene oynatýcý
/// Video bitince callback çaðrýlýr
/// </summary>
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

    /// <summary>
    /// Video oynat ve bitince callback çaðýr
    /// </summary>
    public void PlayCutscene(Action onEnd)
    {
        onVideoEndCallback = onEnd;

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