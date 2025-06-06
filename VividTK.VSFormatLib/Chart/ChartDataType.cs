namespace VividTK.VSFormatLib.Chart;

public enum ChartDataType : byte
{
    NoteListStart = 0xC0,
    NoteListEnd = 0xC1,
    
    NoteEntryStart = 0xA0,
    NoteEntryEnd = 0xA1,
    NoteEntryType = 0xA2,
    NoteEntryLane = 0xA3,
    NoteEntryTime = 0xA4,
    NoteEntryExtra = 0xA6,
    NoteExtraEnd = 0xA7,
    
    GimmickStart = 0xE0,
    GimmickEnd = 0xE1,
    GimmickModData = 0xE2,
    GimmickModEnd = 0xE3,
    GimmickProxies = 0xE4,
    GimmickObjectName = 0xE5,
    GimmickModEntry = 0xE9,
    GimmickPerFrameEntry = 0xEC,
    
    ChartEnd = 0xFF,
}
