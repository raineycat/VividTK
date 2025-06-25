namespace VividTK.VSFormatLib.Chart;

public enum NoteType : byte
{
    /// <summary>
    /// Standard (chip) note
    /// </summary>
    Chip = 0,
    
    /// <summary>
    /// Bumper note
    /// </summary>
    Bumper = 1,
    
    /// <summary>
    /// Hold note
    /// </summary>
    Hold = 2,
    
    /// <summary>
    /// Marker to change the BPM of a song
    /// </summary>
    TempoChange = 3,
    
    /// <summary>
    /// Mines
    /// </summary>
    Mine = 6,
    
    /// <summary>
    /// Bumper mine. A lane value of 0 puts it in the left bumper lane, anything else is the right bumper lane
    /// </summary>
    BumperMine = 7,
    
    /// <summary>
    /// Unknown bumper type #8
    /// </summary>
    BumperUnknown8 = 8,
}