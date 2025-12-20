using UnityEngine;
using UnityEngine.Video;
using System;

public class CutscenePlayer : MonoBehaviour
{
    [Header("Video Settings")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject cutsceneCanvas;

    [Header("Skip Settings")]
    [SerializeField] private bool canSkip = false;
    [SerializeField] private float skipDelay = 1f;

    private Action onCutsceneEndCallback;
    private bool isPlaying = false;
    private float skipTimer = 0f;

    private void Awake()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.SetActive(false);
        }

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    private void Update()
    {
        if (!isPlaying) return;

        if (canSkip)
        {
            skipTimer += Time.deltaTime;

            // Skip için input kontrolü
            if (skipTimer >= skipDelay)
            {
                if (Input.anyKeyDown || (UnityEngine.InputSystem.Gamepad.current != null &&
                    UnityEngine.InputSystem.Gamepad.current.buttonSouth.wasPressedThisFrame))
                {
                    SkipCutscene();
                }
            }
        }
    }

    public void PlayCutscene(Action onEnd)
    {
        if (videoPlayer == null)
        {
            Debug.LogWarning("VideoPlayer is not assigned!");
            onEnd?.Invoke();
            return;
        }

        onCutsceneEndCallback = onEnd;
        isPlaying = true;
        skipTimer = 0f;

        // Canvas'ý aç
        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.SetActive(true);
        }

        // Oyuncu hareketini durdur
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            player.DisableMovement();
        }

        // Video oynat
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        EndCutscene();
    }

    private void SkipCutscene()
    {
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
        EndCutscene();
    }

    private void EndCutscene()
    {
        isPlaying = false;

        // Canvas'ý kapat
        if (cutsceneCanvas != null)
        {
            cutsceneCanvas.SetActive(false);
        }

        // Callback çaðýr
        onCutsceneEndCallback?.Invoke();
        onCutsceneEndCallback = null;
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }
}