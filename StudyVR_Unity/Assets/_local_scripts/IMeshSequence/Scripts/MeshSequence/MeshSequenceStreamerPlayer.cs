using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSequenceStreamerPlayer : MonoBehaviour
{
    public bool isPlaying = false;

    private int CurrentFrameIndex = 0;
    
    public float PlayerFramePerSecond = 30;
    private float FrameTimer = 0;

    private MeshRenderer meshRenderer;

    private MeshFilter meshFilter;

    private MeshSequenceContainer meshSequenceContainer;
    
    private bool isPlayingAudio = false;

    private AudioSource PlayerAudioSource;

    private int FrameCount = 0;

    private bool isLoaded = false;

    void Awake()
    {
        meshSequenceContainer = this.GetComponent<MeshSequenceContainer>();

        meshRenderer = this.GetComponent<MeshRenderer>();
        meshFilter = this.GetComponent<MeshFilter>();

        if(meshSequenceContainer != null && meshRenderer != null && meshFilter != null)
        {
            FrameCount = meshSequenceContainer.MeshSequence.Count;
            SwapFrame();
            isLoaded = true;
        }

        if(this.gameObject.TryGetComponent<AudioSource>(out PlayerAudioSource))
        {
            isPlayingAudio = true;
        }
    }

        
    private void Update()
    {
        if (isPlaying && isLoaded)
        {
            Debug.Log("Playing");
            if(isPlayingAudio)
            {
                if(Mathf.FloorToInt(PlayerAudioSource.time / PlayerAudioSource.clip.length * (FrameCount - 1)) != CurrentFrameIndex)
                {
                    SwapFrame();
                }
            }
            else
            {
                FrameTimer += Time.deltaTime;

                if (FrameTimer >= (1 / PlayerFramePerSecond))
                {

                    SwapFrame();

                    FrameTimer = 0;
                }
            }
        }
    }


    private void SwapFrame(bool isReversing = false)
    {
        meshRenderer.sharedMaterial = meshSequenceContainer.MaterialSequence[CurrentFrameIndex];
        meshFilter.mesh = meshSequenceContainer.MeshSequence[CurrentFrameIndex];

        if (isReversing)
        {
            CurrentFrameIndex = (CurrentFrameIndex - 1) < 0 ? FrameCount - 1 : CurrentFrameIndex - 1;
        }
        else
        {
            CurrentFrameIndex = (CurrentFrameIndex + 1) >= FrameCount ? 0 : CurrentFrameIndex + 1;
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        isPlaying = true;
        PlayerAudioSource.Play();
    }

    public void Stop()
    {
        isPlaying = false;
        PlayerAudioSource.Stop();
        CurrentFrameIndex = 0;
    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        isPlaying = false;
        PlayerAudioSource.Pause();
    }
}
