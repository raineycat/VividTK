namespace VividTK.VSFormatLib.Chart;

public struct NoteData
{
    public float Time { get; set; }
    public LaneType Lane { get; set; }
    public NoteType Type { get; set; }

    // only used for hold notes
    public float EndTime { get; set; }

    // only valid for temp change notes
    public float BPM { get; set; }
}