namespace VividTK.VSFormatLib.Chart;

public struct NoteData
{
    public float Time { get; set; }
    public LaneType Lane { get; set; }
    public NoteType Type { get; set; }
    public Dictionary<byte, object> Extra { get; set; }

    public float HoldEndMillis
    {
        get
        {
            if (Type != NoteType.Hold) throw new InvalidOperationException("Not valid for this note type!");
            if (Extra.TryGetValue(1, out var ms))
            {
                return (float)(int)ms;
            }

            throw new InvalidOperationException("The note is missing this data!");
        }
    }
    
    public float? NewTempo
    {
        get
        {
            if (Type != NoteType.TempoChange) throw new InvalidOperationException("Not valid for this note type!");
            if (Extra.TryGetValue(1, out var newTempo))
            {
                return (float)newTempo;
            }

            return null;
        }
    }
}