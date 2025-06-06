namespace VividTK.VSFormatLib.VSD
{
    public enum ObjectType : byte
    {
        SONG_INFO = 0xA0,
        SONG_FIELD = 0xA2,
        CHART_INFO = 0xC0,
        DATA_END = 0xA1,
    }
}
