using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeshSequencePlayer : MonoBehaviour
{
    public Vector3 PositionOffset = Vector3.zero;
    public Vector3 RotationOffset = Vector3.zero;
    public Vector3 ScaleOffset = Vector3.one;

    public float PlayerFramePerSecond = 30;

    public bool isPlaying = false;

    private int CurrentFrame = 0;

    private float FrameTimer = 0;

    private MeshSequenceContainer meshSequenceContainer;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public bool isPlayingAudio = false;
    [SerializeField][HideInInspector] public AudioClip PlayerAudio;
    private AudioSource PlayerAudioSource;

    private int FrameCount = 0;


    private void Awake()
    {
        foreach (Transform child in this.transform)
        {
            child.gameObject.SetActive(false);
        }

        meshSequenceContainer = this.GetComponent<MeshSequenceContainer>();

        if (meshSequenceContainer == null)
        {
            Debug.LogError("Please load mesh sequence first!");
            isPlaying = false;
            return;
        }

        meshRenderer = this.GetComponent<MeshRenderer>();
        meshFilter = this.GetComponent<MeshFilter>();
        FrameCount = meshSequenceContainer.MeshSequence.Count;

        if (isPlayingAudio)
        {
            if (PlayerAudio != null)
            {
                PlayerAudioSource = this.gameObject.AddComponent<AudioSource>();
                PlayerAudioSource.clip = PlayerAudio;
                PlayerAudioSource.loop = true;
                PlayerFramePerSecond = meshSequenceContainer.MeshSequence.Count / PlayerAudio.length;
            }
        }
    }

    private void Start()
    {
        if (meshSequenceContainer != null)
        {
            this.transform.position += PositionOffset;
            this.transform.eulerAngles += RotationOffset;
            this.transform.localScale = ScaleOffset;
        }

        if (isPlayingAudio)
        {
            PlayerAudioSource.Play();
        }
    }

    private void Update()
    {
        if (isPlaying)
        {
            if (isPlayingAudio)
            {
                if (Mathf.FloorToInt(PlayerAudioSource.time / PlayerAudio.length * (FrameCount - 1)) != CurrentFrame)
                {
                    SwapFrame();
                }
            }
            else
            {
                FrameTimer += Time.deltaTime;

                if (FrameTimer >= 1f / PlayerFramePerSecond)
                {
                    SwapFrame();
                    FrameTimer = 0;
                }
            }

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SwapFrame();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SwapFrame(true);
            }
        }
    }

    private void SwapFrame(bool isReversing = false)
    {
        int NextFrame = 0;
        if (isReversing)
        {
            NextFrame = (CurrentFrame - 1) < 0 ? FrameCount - 1 : CurrentFrame - 1;
        }
        else
        {
            NextFrame = (CurrentFrame + 1) >= FrameCount ? 0 : CurrentFrame + 1;
        }

        meshFilter.mesh = meshSequenceContainer.MeshSequence[NextFrame];

        //meshRenderer.sharedMaterial = meshSequenceContainer.MaterialSequence[NextFrame];
        
        CurrentFrame = NextFrame;
    }

    [ContextMenu("Apply Player Transform Offset")]
    public void ApplyOffset()
    {
        PositionOffset = this.transform.position;
        RotationOffset = this.transform.eulerAngles;
        ScaleOffset = this.transform.localScale;

        Debug.Log("Offset Applied");
    }
}
