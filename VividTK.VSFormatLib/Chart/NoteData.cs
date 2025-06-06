namespace VividTK.VSFormatLib.Chart;

public struct NoteData
{
    public float Time { get; set; }
    public byte Lane { get; set; }
    public byte Type { get; set; }
    public Dictionary<byte, object> Extra { get; set; }
}