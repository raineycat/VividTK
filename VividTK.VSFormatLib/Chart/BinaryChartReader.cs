using VividTK.VSFormatLib.VSD;

namespace VividTK.VSFormatLib.Chart;

public class BinaryChartReader : IChartReader
{
    public List<NoteData> Notes { get; set; } = [];
    public GimmickData Gimmick { get; set; }
    
    private readonly BinaryReader _reader;

    public BinaryChartReader(BinaryReader br)
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

            if((flag & ChartDataType.NoteEntryStart) != ChartDataType.NoteEntryStart)
            {
                throw new InvalidOperationException("Invalid note entry flag!");
            }

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
                    note = ReadNoteExtra(note);
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

    private NoteData ReadNoteExtra(NoteData note)
    {
        var fields = new Dictionary<byte, object>();
        ChartDataType type;
        
        do
        {
            type = (ChartDataType)_reader.ReadByte();
            if (type != ChartDataType.NoteExtraEnd)
            {
                // 0xB0 is the mask for data types
                if (((byte)type & 0xB0) != 0xB0)
                {
                    throw new InvalidOperationException("Invalid extra entry flag!");
                }

                var fieldId = _reader.ReadByte();
                var dataType = SongFieldTypeHelper.ByteToFieldType((byte)type);
                var data = dataType.ReadFrom(_reader);
                fields.Add(fieldId, data);
            }
        } while (type != ChartDataType.NoteExtraEnd);

        switch(note.Type)
        {
            case NoteType.Hold:
                if (fields[1] is int endTime)
                {
                    note.EndTime = (float)endTime;
                    //Console.WriteLine("EndTime int");
                }
                else if (fields[1] is float endTimeFloat)
                {
                    note.EndTime = endTimeFloat;
                    //Console.WriteLine("EndTime float");
                }
                break;

            case NoteType.TempoChange:
                if (fields[1] is int bpm)
                {
                    note.BPM = (float)bpm;
                    //Console.WriteLine("BPM int");
                } 
                else if (fields[1] is float bpmFloat)
                {
                    note.BPM = bpmFloat;
                    //Console.WriteLine("BPM float");
                }
                break;
        }

        return note;
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
        
        Gimmick = data;
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
            From = _reader.ReadSingle(),
            To = _reader.ReadSingle(),
            Type = (ModType)_reader.ReadByte(),
            ProxyIndex = unchecked((sbyte)_reader.ReadByte())
        };

        // Console.WriteLine("R: ProxyIdx: " + (int)data.ProxyIndex);
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
