using UnityEngine;

[System.Serializable]
public enum ERadioSong
{
    Acoustic1 = 0,
    Acoustic2,
    Chinese1,
    Chinese2,
    Chinese3,
    EDM1,
    EDM2,
    Funk1,
    Funk2,
    Funk3,
    HipHop,
    Malay1,
    Malay2,
    Malay3,
    OldSchool1,
    OldSchool2,
    Pop1,
    Pop2,
    Mmu1,
    Mmu2,
    Rock1,
    Rock2,
    Rock3,
    Rock4
}

[CreateAssetMenu(fileName ="New Radio Song", menuName ="Radio Song")]
public class RadioSong : ScriptableObject
{
    public ERadioSong radioSongType;

    public AudioClip song;

    [Header("Radio Effect")]
    public float mainAttenuation = 0.0f;
    public float distortionLevel = 0.5f;
    public int lowpassFreq = 3500;

    [Header("Noise Effect")]
    public float noiseAttenuation = 0.0f;
}
