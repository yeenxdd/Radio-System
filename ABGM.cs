[System.Serializable]
public enum EBGM
{
    MAINMENU = 0
}

[System.Serializable]
public class ABGM : ASound
{
    public EBGM bgmType;
}
