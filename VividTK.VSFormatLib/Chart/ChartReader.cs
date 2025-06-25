using VividTK.VSFormatLib.VSD;

namespace VividTK.VSFormatLib.Chart;

public class ChartReader
{
    public List<NoteData> Notes = [];
    public List<GimmickData> Gimmicks = [];
    
    private readonly BinaryReader _reader;

    public ChartReader(BinaryReader br)
    {
        _reader = br;

        foreach (var c in "VSC")
        {
            if (_reader.ReadChar() != c)
            {
                throw new FileFormatException("Invalid VSC magic number");
            }
        }

        var formatVersion = _reader.ReadByte();
        if (formatVersion != 1)
        {
            throw new FileFormatException("Only ChartV1 support is implemented!");
        }

        if (_reader.ReadByte() != 0)
        {
            throw new FileFormatException("Missing magic null byte!");
        }

        ChartDataType rootType;
        do
        {
            rootType = (ChartDataType)_reader.ReadByte();

            switch (rootType)
            {
                case ChartDataType.NoteListStart:
                    ReadNoteList();
                    break;
                case ChartDataType.GimmickStart:
                    ReadGimmickData();
                    break;
            }
        }
        while (rootType != ChartDataType.ChartEnd);
    }

    private void ReadNoteList()
    {
        ChartDataType noteFlag;
        do
        {
            noteFlag = (ChartDataType)_reader.ReadByte();

            if (noteFlag == ChartDataType.NoteEntryStart)
            {
                ReadSingleNote();
            }
        }
        while (noteFlag != ChartDataType.NoteListEnd);
    }

    private void ReadSingleNote()
    {
        ChartDataType flag;
        var note = new NoteData();
        
        do
        {
            flag = (ChartDataType)_reader.ReadByte();
            switch (flag)
            {
                case ChartDataType.NoteEntryType:
                    note.Type = (NoteType)_reader.ReadByte();
                    break;
                case ChartDataType.NoteEntryLane:
                    note.Lane = (LaneType)_reader.ReadByte();
                    break;
                case ChartDataType.NoteEntryTime:
                    note.Time = _reader.ReadSingle();
                    break;
                case ChartDataType.NoteEntryExtra:
                    note.Extra = ReadNoteExtra();
                    break;
            }
        } while (flag != ChartDataType.NoteEntryEnd);


        switch (note.Type)
        {
            case NoteType.Bumper:
                note.Lane = note.Lane switch
                {
                    LaneType.Lane1 => LaneType.LeftBumper,
                    LaneType.Lane2 => LaneType.MiddleBumper,
                    _ => LaneType.RightBumper
                };
                break;
            
            case NoteType.BumperMine:
                note.Lane = note.Lane == LaneType.Lane1 ? LaneType.LeftBumper : LaneType.RightBumper;
                break;
        }
        
        Notes.Add(note);
    }

    private Dictionary<byte, object> ReadNoteExtra()
    {
        var fields = new Dictionary<byte, object>();
        ChartDataType type;
        
        do
        {
            type = (ChartDataType)_reader.ReadByte();
            if (type != ChartDataType.NoteExtraEnd)
            {
                var fieldId = _reader.ReadByte();
                var dataType = SongFieldTypeHelper.ByteToFieldType((byte)type);
                var data = dataType.ReadFrom(_reader);
                fields.Add(fieldId, data);
            }
        } while (type != ChartDataType.NoteExtraEnd);

        return fields;
    }

    private void ReadGimmickData()
    {
        var data = new GimmickData();
        ChartDataType flag;

        do
        {
            flag = (ChartDataType)_reader.ReadByte();
            switch (flag)
            {
                case ChartDataType.GimmickProxies:
                    data.Proxies = _reader.ReadByte();
                    break;
                case ChartDataType.GimmickObjectName:
                    data.ObjectName = _reader.ReadTerminatedString();
                    break;
                case ChartDataType.GimmickModData:
                    data = ReadGimmickModData(data);
                    break;
            }
        } while (flag != ChartDataType.GimmickEnd);
        
        Gimmicks.Add(data);
    }

    private GimmickData ReadGimmickModData(GimmickData data)
    {
        ChartDataType flag;
        data.Mods = new List<ModData>();
        data.PerFrame = new List<PerFrameData>();

        do
        {
            flag = (ChartDataType)_reader.ReadByte();
            switch (flag)
            {
                case ChartDataType.GimmickModEntry:
                    data.Mods.Add(ReadGimmickModEntry());
                    break;
                case ChartDataType.GimmickPerFrameEntry:
                    data.PerFrame.Add(ReadGimmickPerFrameEntry());
                    break;
            }
        } while (flag != ChartDataType.GimmickModEnd);
        
        return data;
    }

    private ModData ReadGimmickModEntry()
    {
        var data = new ModData
        {
            StartOffset = _reader.ReadSingle(),
            Duration = _reader.ReadSingle(),
            Ease = (Easing)_reader.ReadByte(),
            V1 = _reader.ReadSingle(),
            V2 = _reader.ReadSingle(),
            Type = (ModType)_reader.ReadByte(),
            ProxyIndex = _reader.ReadChar()
        };
        
        data.Weight = ModTypeHelper.GetModWeight(data.Type);
        return data;
    }

    private PerFrameData ReadGimmickPerFrameEntry()
    {
        var data = new PerFrameData
        {
            B = _reader.ReadSingle(),
            E = _reader.ReadSingle(),
            FunctionName = _reader.ReadTerminatedString()
        };
        
        return data;
    }
}
