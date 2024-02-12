using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Sounds", menuName = "SO/SO_Sounds")]
public class SO_sounds : SingletonScriptableobject<SO_sounds>
{
    [SerializeField] private List<RadioSong> m_radioSongList = new List<RadioSong>();
    [SerializeField] private List<ASFX> m_sfxList = new List<ASFX>();
    [SerializeField] private List<ABGM> m_bgmList = new List<ABGM>();

    //==================================================

    public List<RadioSong> GetRadioSongList()
    {
        return this.m_radioSongList;
    }

    public RadioSong GetRadioSong(ERadioSong radioSongType)
    {
        for(int i = 0; i < this.m_radioSongList.Count; ++i)
        {
            if(this.m_radioSongList[i].radioSongType == radioSongType)
            {
                return this.m_radioSongList[i];
            }
        }

        return null;
    }

    public List<ASFX> GetSFXList()
    {
        return this.m_sfxList;
    }

    public ASFX GetSFX(ESFX sfxType)
    {
        for(int i = 0; i < this.m_sfxList.Count; ++i)
        {
            if(this.m_sfxList[i].sfxType == sfxType)
            {
                return this.m_sfxList[i];
            }
        }

        return null;
    }

    public List<ABGM> GetBGMList()
    {
        return this.m_bgmList;
    }

    public ABGM GetBGM(EBGM bgmType)
    {
        for(int i= 0; i < this.m_bgmList.Count; ++i)
        {
            if(this.m_bgmList[i].bgmType == bgmType)
            {
                return this.m_bgmList[i];
            }
        }

        return null;
    }

    //==================================================

}
