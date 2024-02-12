using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum ESnapShots
{
    SwitchOff,
    SwitchOn
}

public class ListForSong
{
    public List<RadioSong> list = new List<RadioSong>();

    public void AddSong(RadioSong radioSong)
    {
        list.Add(radioSong);
    }
}

[RequireComponent(typeof(AudioSource))]
public class RadioSystem : MonobehaviourSingleton<RadioSystem>
{
    [Header("Radio Song Data")]
    private AudioManager audioManager;

    [Header("Radio")]
    [SerializeField] private AudioMixer mixer;
    private AudioSource channelSource;
    private bool switchStatus;
    private bool powerSwitch = true;

    [Header("FM Slider")]
    [SerializeField] private Transform fmSlider;
    private Coroutine m_lerpCO = null;
    private Vector3 initFMPosition = new Vector3();

    [Header("Switch Channel")]
    [SerializeField] private AudioClip switchChannelSound;
    [SerializeField] private List<Transform> channelTransform;
    private Coroutine switchChannelCO = null;
    private int songInPlaylist = 0;
    private int channel = 0;
    private int currentChannel = -1;
    private float channelRange;

    // snapshots
    private AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[2];
    private const float transitionTime = 0.8f;

    [Header("Particle")]
    [SerializeField] private Transform particleSpawnPoint;
    private AParticle particle = null;

    protected override void Start()
    {
        base.Start();

        // SOURCES
        channelSource = this.GetComponent<AudioSource>();
        //switchStatus = false;

        audioManager = AudioManager.GetInstance();

        // Initialize channel and playlist
        AddSongIntoList();
        //CurrentChannel();

        // FM Slider
        initFMPosition = channelTransform[0].position;
        SetFMBar();

        // find and store the snapshots
        string[] snapshotsNames = Enum.GetNames(typeof(ESnapShots));
        for (int i = 0; i < snapshotsNames.Length; i++)
        {
            snapshots[i] = mixer.FindSnapshot(snapshotsNames[i]);
        }
        // -----------------------------

        //slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        // radio is turned off at the start of scene
        PowerSwitch();
    }

    // Channel Category
    // 0 - Acoustic
    // 1 - Chinese
    // 2 - EDM
    // 3 - Funk
    // 4 - Malay
    // 5 - Old School
    // 6 - Pop
    // 7 - Rock

    #region Radio Song Lists & Playlist Dictionary
    private ListForSong AcousticList = new ListForSong();
    private ListForSong ChineseList = new ListForSong();
    private ListForSong EDMList = new ListForSong();
    private ListForSong FunkList = new ListForSong();
    private ListForSong MalayList = new ListForSong();
    private ListForSong OldSchoolList = new ListForSong();
    private ListForSong PopList = new ListForSong();
    private ListForSong RockList = new ListForSong();
    private Dictionary<int, ListForSong> playlist = new Dictionary<int, ListForSong>();
    private void AddSongIntoList()
    {
        AcousticList.AddSong(audioManager.GetRadioSongData(ERadioSong.Acoustic1));
        AcousticList.AddSong(audioManager.GetRadioSongData(ERadioSong.Acoustic2));

        ChineseList.AddSong(audioManager.GetRadioSongData(ERadioSong.Chinese1));
        ChineseList.AddSong(audioManager.GetRadioSongData(ERadioSong.Chinese2));
        ChineseList.AddSong(audioManager.GetRadioSongData(ERadioSong.Chinese3));

        EDMList.AddSong(audioManager.GetRadioSongData(ERadioSong.EDM1));
        EDMList.AddSong(audioManager.GetRadioSongData(ERadioSong.EDM2));

        FunkList.AddSong(audioManager.GetRadioSongData(ERadioSong.Funk1));
        FunkList.AddSong(audioManager.GetRadioSongData(ERadioSong.Funk2));
        FunkList.AddSong(audioManager.GetRadioSongData(ERadioSong.Funk3));

        MalayList.AddSong(audioManager.GetRadioSongData(ERadioSong.Malay1));
        MalayList.AddSong(audioManager.GetRadioSongData(ERadioSong.Malay2));
        MalayList.AddSong(audioManager.GetRadioSongData(ERadioSong.Malay3));

        OldSchoolList.AddSong(audioManager.GetRadioSongData(ERadioSong.OldSchool1));
        OldSchoolList.AddSong(audioManager.GetRadioSongData(ERadioSong.OldSchool2));

        PopList.AddSong(audioManager.GetRadioSongData(ERadioSong.Pop1));
        PopList.AddSong(audioManager.GetRadioSongData(ERadioSong.Pop2));

        RockList.AddSong(audioManager.GetRadioSongData(ERadioSong.Rock1));
        RockList.AddSong(audioManager.GetRadioSongData(ERadioSong.Rock2));
        RockList.AddSong(audioManager.GetRadioSongData(ERadioSong.Rock3));
        RockList.AddSong(audioManager.GetRadioSongData(ERadioSong.Rock4));

        playlist.Add(0, AcousticList);
        playlist.Add(1, ChineseList);
        playlist.Add(2, EDMList);
        playlist.Add(3, FunkList);
        playlist.Add(4, MalayList);
        playlist.Add(5, OldSchoolList);
        playlist.Add(6, PopList);
        playlist.Add(7, RockList);

        channelRange = 360f / (float)playlist.Count;
    }

    //private void CurrentChannel()
    //{
    //    channel = Mathf.FloorToInt((angle + 180f) / channelRange);
    //}
    #endregion

    #region FM Bar
    private void SetFMBar()
    {
        if (this.m_lerpCO != null)
        {
            this.StopCoroutine(this.m_lerpCO);
            this.m_lerpCO = null;
        }

        this.m_lerpCO = StartCoroutine(LerpFMBar());
    }

    private float totalDuration = 10.0f;
    private IEnumerator LerpFMBar()
    {
        float elapsedTime = 0.0f;
        Vector3 destination = channelTransform[channel].position;
        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;

            fmSlider.position = Vector3.Lerp(fmSlider.position, destination, elapsedTime);
            yield return null;
        }
    }
    #endregion

    // call this method when changing channels
    public void ChangeChannel(int channelNum)
    {
        channel = channelNum;
        SetChannel();
    }

    private void SetChannel()
    {
        if (currentChannel == channel)
        {
            SetFMBar();
        }
        if (switchStatus)
            PlaySong();
    }

    // call this for radio power switch
    [ContextMenu("Power Switch")]
    public void PowerSwitch()
    {
        powerSwitch = !powerSwitch;

        switchStatus = powerSwitch;
        Debug.Log("powerSwitch = " + switchStatus);

        // if switch is turned off, stop the song & the radio
        if (!powerSwitch)
        {
            SnapShotTransition("SwitchOff");
            StartCoroutine(StopMusic());
            return;
        }
        else
        {
            // switch is turned on
            // play the current channel
            SnapShotTransition("SwitchOn");
            SetChannel();
        }
    }

    private void SnapShotTransition(string snapshotName)
    {
        for (int i = 0; i < snapshots.Length; i++)
        {
            if (snapshots[i].name == snapshotName)
            {
                snapshots[i].TransitionTo(transitionTime);
                return;
            }
        }
    }

    private IEnumerator StopMusic()
    {
        yield return new WaitForSeconds(1.0f);
        channelSource.Stop();

        if(switchChannelCO != null)
        {
            StopCoroutine(switchChannelCO);
        }

        // stop particle
        if (particle != null)
        {
            particle.ForceReturnToPool();
            particle = null;
        }

    }

    private RadioSong GetSong()
    {
        // if index is the last number in the list, start the list from 0
        if (playlist[channel].list.Count > songInPlaylist + 1)
        {
            // if channel is the same, proceed to the next song
            // if channel has changed, play the first song on the list
            songInPlaylist = currentChannel == channel ? songInPlaylist + 1 : 0;
        }
        else
        {
            songInPlaylist = 0;
        }
        currentChannel = channel;
        return playlist[channel].list[songInPlaylist];
    }

    #region Play Song & Audio Tuning
    private void PlaySong()
    {
        // if there is other song playing, stop the radio
        if (channelSource.isPlaying)
        {
            channelSource.Stop();
        }

        if (switchChannelCO != null)
        {
            channelSource.Stop();
            StopCoroutine(switchChannelCO);
        }
        switchChannelCO = StartCoroutine(SwitchChannel());
    }

    private IEnumerator SwitchChannel()
    {
        channelSource.clip = switchChannelSound;
        channelSource.Play();
        yield return new WaitUntil(() => !channelSource.isPlaying);

        // particle
        if (particle == null)
        {
            particle = ObjectPooling.GetInstance().GetFromPool(SO_particleBlueprint.GetInstance().GetParticle(EPARTICLE.MUSIC).gameObject).GetComponent<AParticle>();
            particle.transform.position = particleSpawnPoint.position;
        }

        while (switchChannelCO != null)
        {
            // get the channel's song
            // Retrieving data from AudioManager.cs
            RadioSong currentSong = GetSong();

            // audio tuning
            AudioTuning(currentSong);

            // play the song
            channelSource.clip = currentSong.song;
            channelSource.Play();

            // waiting to play the next song
            yield return new WaitUntil(() => !channelSource.isPlaying);
        }
    }

    private void AudioTuning(RadioSong currentSong)
    {
        mixer.SetFloat("mainAttenuation", currentSong.mainAttenuation);
        mixer.SetFloat("distortionLevel", currentSong.distortionLevel);
        mixer.SetFloat("lowpassFreq", currentSong.lowpassFreq);
    }
    #endregion

    // call this when setting volume, range: -80 ~ 0
    public void SetVolume(int volumeNum)
    {
        if (switchStatus)
            mixer.SetFloat("masterVolume", volumeNum);
    }
}


