[System.Serializable]
public enum ESFX
{
    CHOP = 0,
    BURNING,
    STOVE,
    METAL,
    WOOD,
    ICE,
    LEVER,
    POUR,
    COOK,
    GRAB,
    ORDER,
    CORRECT,
    RADIO_CLICK,
    STOVE_CLICK,
    UI_BUTTON,
    INCORRECT,
    BITE,
    DRINK
}

[System.Serializable]
public class ASFX : ASound
{
    public ESFX sfxType;
}
