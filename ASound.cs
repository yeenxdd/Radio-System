using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class ASound
{
    public AudioClip audioClip;

    [Range(0.0f, 1.0f)]
    public float volume;
    [Range(0.1f, 3.0f)]
    public float pitch;
    [Range(0.1f, 1.0f)]
    public float sptialBlend;

    public bool loop;

    [Header("3D Sound Setting")]
    [Range(0.1f, 5.0f)]
    public float dopplerLevel;
    [Range(0, 360)]
    public float spread;
    public AudioRolloffMode volumeRolloff;
    public float minDistance;
    public float maximumDistance;

    [HideInInspector]
    public AudioSource source;
}
