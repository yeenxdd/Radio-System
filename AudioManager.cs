using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonobehaviourSingleton<AudioManager>
{
    [SerializeField] private Transform m_sfxParent = null;
    [SerializeField] private Transform m_bgmParent = null;
    [SerializeField] private Transform m_radioSongParent = null;

    private float m_bgmMasterVolume = 0.5f;
    private float m_sfxMasterVolume = 0.5f;

    private SO_sounds m_soundsData = null;

    //==================================================

    protected override void Awake()
    {
        base.Awake();

        this.AwakeToDo();
    }

    protected override void Start()
    {
        base.Start();
    }

    //==================================================

    //Public Function

    #region RADIO_SONG

    //The Source will be on Radio Object itself
    public RadioSong GetRadioSongData(ERadioSong radioSongType)
    {
        RadioSong radioSong;

        this.GetRadioSongInList(radioSongType, out radioSong);

        return radioSong;
    }

    #endregion RADIO_SONG

    #region BGM

    //BGM NO PLAY ONE SHOT BECAUSE DOESN'T NEED

    [ContextMenu("Debug BGM")]
    public void DebugPlayBGM()
    {
        this.PlayBGM(EBGM.MAINMENU);
    }

    public void PlayBGM(EBGM bgmType)
    {
        AudioSource source;
        if (!this.GetBGMInList(bgmType, out source))
        {
            Debug.LogWarning("Your Sound is not in the list! Sound Type:" + bgmType);
            return;
        }

        source.Play();
    }

    public void PauseBGM(EBGM bgmType)
    {
        AudioSource source;
        if (!this.GetBGMInList(bgmType, out source))
        {
            Debug.LogWarning("Your Sound is not in the list! Sound Type:" + bgmType);
            return;
        }

        source.Play();
    }

    public void StopBGM(EBGM bgmType)
    {
        AudioSource source;
        if (!this.GetBGMInList(bgmType, out source))
        {
            Debug.LogWarning("Your Sound is not in the list! Sound Type:" + bgmType);
            return;
        }

        if (source.isPlaying)
        {
            source.Stop();
            return;
        }

        Debug.LogWarning("Your Sound is not playing! Sound Type:" + bgmType);
    }

    public void AdjustBGMVolume(float volume)
    {
        this.m_bgmMasterVolume = Mathf.Clamp(volume, 0.0f, 1.0f);

        foreach (ABGM bgm in this.m_soundsData.GetBGMList())
        {
            bgm.source.volume = bgm.volume * this.m_bgmMasterVolume;
            //bgm.volume = volume;
        }
    }

    public float GetBGMMasterVolume()
    {
        return this.m_bgmMasterVolume;
    }

    #endregion BGM

    #region SFX

    [ContextMenu("Debug SFX")]
    public void DebugPlaySFX()
    {
        this.PlaySFXOneShot(ESFX.CHOP, Camera.main.transform.position);
    }

    /// <summary>
    /// This function will override the loop to make it only play one time
    /// </summary>
    /// <param name="sfxType">SFX to play</param>
    /// <param name="positionToFire">Position To Play</param>
    public void PlaySFXOneShot(ESFX sfxType, Vector3 positionToFire)
    {
        ASFX sfx;

        if (!this.GetSFXInList(sfxType, out sfx))
        {
            Debug.LogWarning("Your Sound is not in the list! Sound Type:" + sfxType);
            return;
        }

        //Create a empty game object
        GameObject oneShotObject = new GameObject();
        oneShotObject.transform.position = positionToFire;

        AudioSource source = oneShotObject.AddComponent<AudioSource>();
        source.clip = sfx.audioClip;
        oneShotObject.name = source.clip.name;

        source.volume = sfx.volume;
        source.pitch = sfx.pitch;
        source.spatialBlend = sfx.sptialBlend;

        source.dopplerLevel = sfx.dopplerLevel;
        source.spread = sfx.spread;
        source.rolloffMode = sfx.volumeRolloff;
        source.minDistance = sfx.minDistance;
        source.maxDistance = sfx.maximumDistance;

        source.Play();

        this.StartCoroutine(this.DestroyOneShotAfter(oneShotObject, source));
    }

    public void PlaySFX(ESFX sfxType)
    {
        AudioSource source;
        if (!this.GetSFXInList(sfxType, out source))
        {
            Debug.LogWarning("Your Sound is not in the list! Sound Type:" + sfxType);
            return;
        }

        source.Play();
    }

    public void PauseSFX(ESFX sfxType)
    {
        AudioSource source;
        if (!this.GetSFXInList(sfxType, out source))
        {
            Debug.LogWarning("Your Sound is not in the list! Sound Type:" + sfxType);
            return;
        }

        source.Play();
    }

    public void StopSFX(ESFX sfxType)
    {
        AudioSource source;
        if(!this.GetSFXInList(sfxType, out source))
        {
            Debug.LogWarning("Your Sound is not in the list! Sound Type:" + sfxType);
            return;
        }

        if(source.isPlaying)
        {
            source.Stop();
            return;
        }

        Debug.LogWarning("Your Sound is not playing! Sound Type:" + sfxType);
    }

    public void AdjustSFXVolume(float volume)
    {
        this.m_sfxMasterVolume = Mathf.Clamp(volume, 0.0f, 1.0f);

        foreach (ASFX sfx in this.m_soundsData.GetSFXList())
        {
            sfx.source.volume = sfx.volume * this.m_sfxMasterVolume;
            //sfx.volume = volume;
        }
    }

    public float GetSFXMasterVolume()
    {
        return this.m_sfxMasterVolume;
    }

    #endregion SFX

    //Private Functions

    private bool GetRadioSongInList(ERadioSong radioSongType, out RadioSong radioSong)
    {
        radioSong = this.m_soundsData.GetRadioSong(radioSongType);

        return radioSong != null;
    }

    private bool GetBGMInList(EBGM bgmType, out AudioSource audio)
    {
        audio = this.m_soundsData.GetBGM(bgmType).source;

        return audio != null;
    }

    private bool GetBGMInList(EBGM bgmType, out ABGM bgm)
    {
        bgm = this.m_soundsData.GetBGM(bgmType);

        return bgm != null;
    }

    private bool GetSFXInList(ESFX sfxType, out AudioSource audio)
    {
        audio = this.m_soundsData.GetSFX(sfxType).source;

        return audio != null;
    }

    private bool GetSFXInList(ESFX sfxType, out ASFX sfx)
    {
        sfx = this.m_soundsData.GetSFX(sfxType);

        return sfx != null;
    }

    private void AwakeToDo()
    {
        this.m_soundsData = SO_sounds.GetInstance();

        if(this.m_bgmParent == null)
        {
            GameObject bgmParent = new GameObject("BGM");
            bgmParent.transform.SetParent(this.transform);
            this.m_bgmParent = bgmParent.transform;
        }

        if(this.m_sfxParent == null)
        {
            GameObject sfxParent = new GameObject("SFX");
            sfxParent.transform.SetParent(this.transform);
            this.m_sfxParent = sfxParent.transform;
        }

        if(this.m_radioSongParent == null)
        {
            GameObject radioParent = new GameObject("Radio");
            radioParent.transform.SetParent(this.transform);
            this.m_radioSongParent = radioParent.transform;
        }

        if (this.m_soundsData == null) return;

        foreach (ASFX sfx in this.m_soundsData.GetSFXList())
        {
            sfx.source = this.m_sfxParent.gameObject.AddComponent<AudioSource>();
            sfx.source.clip = sfx.audioClip;

            sfx.source.volume = sfx.volume;
            sfx.source.pitch = sfx.pitch;
            sfx.source.spatialBlend = sfx.sptialBlend;
            sfx.source.loop = sfx.loop;

            sfx.source.dopplerLevel = sfx.dopplerLevel;
            sfx.source.spread = sfx.spread;
            sfx.source.rolloffMode = sfx.volumeRolloff;
            sfx.source.minDistance = sfx.minDistance;
            sfx.source.maxDistance = sfx.maximumDistance;
        }

        foreach (ABGM bgm in this.m_soundsData.GetBGMList())
        {
            bgm.source = this.m_bgmParent.gameObject.AddComponent<AudioSource>();
            bgm.source.clip = bgm.audioClip;

            bgm.source.volume = bgm.volume;
            bgm.source.pitch = bgm.pitch;
            bgm.source.spatialBlend = bgm.sptialBlend;
            bgm.source.loop = bgm.loop;

            bgm.source.dopplerLevel = bgm.dopplerLevel;
            bgm.source.spread = bgm.spread;
            bgm.source.rolloffMode = bgm.volumeRolloff;
            bgm.source.minDistance = bgm.minDistance;
            bgm.source.maxDistance = bgm.maximumDistance;
        }

        this.AdjustBGMVolume(this.m_bgmMasterVolume);
        this.AdjustSFXVolume(this.m_sfxMasterVolume);
    }

    private IEnumerator DestroyOneShotAfter(GameObject obj, AudioSource source)
    {

        while(source.isPlaying)
        {
            //if is playing do nothing
            yield return null;
        }

        Destroy(obj);
    }

    //==================================================
}
