namespace VividTK.VSFormatLib.VSD;

public class VSDReader
{
    private readonly BinaryReader? _reader = null;

    public List<SongInfo> Songs { get; set; } = new();

    public VSDReader() { }

    public VSDReader(BinaryReader br)
    {
        _reader = br;

        foreach (var c in "VSD")
        {
            if (_reader.ReadChar() != c)
            {
                throw new FileFormatException("Invalid VSD magic number");
            }
        }

        var formatVersion = _reader.ReadByte();
        if (formatVersion != 1)
        {
            throw new FileFormatException("Only VSD1 support is implemented!");
        }

        if (_reader.ReadByte() != 0)
        {
            throw new FileFormatException("Missing magic null byte!");
        }

        //var pos = _reader.BaseStream.Position;
        //_reader.BaseStream.Seek(-384, SeekOrigin.End);
        //var signature = SignatureUtil.DecodeChartSignature(_reader.ReadBytes(384));
        //_reader.BaseStream.Seek(pos, SeekOrigin.Begin);
        //Console.WriteLine("Sig: " + signature);

        while (_reader.ReadByte() == (byte)ObjectType.SONG_INFO)
        {
            Songs.Add(ReadSong());
        }
    }

    private SongInfo ReadSong()
    {
        if (_reader == null) throw new InvalidOperationException("No data to read!");

        var song = new SongInfo(0);
        var charts = new List<ChartInfo>();
        //var chartType = ChartType.OPENING;
        var chartIndex = 1;

        ObjectType ot;
        do
        {
            ot = (ObjectType)_reader.ReadByte();
            switch (ot)
            {
                case ObjectType.CHART_INFO:
                    charts.Add(new ChartInfo
                    {
                        //Type = chartType,
                        Index = chartIndex,
                        DifficultyDisplay = _reader.ReadTerminatedString(),
                        DifficultyConstant = _reader.ReadSingle(),
                        NoteDesigner = _reader.ReadTerminatedString()
                    });
                    //chartType = (ChartType)((int)chartType + 1);
                    chartIndex++;
                    break;

                case ObjectType.SONG_FIELD:
                    var fieldType = SongFieldTypeHelper.ByteToFieldType(_reader.ReadByte());
                    var fieldId = (SongField)_reader.ReadByte();

                    switch (fieldId)
                    {
                        case SongField.SongId:
                            song.SongId = _reader.ReadUInt32();
                            break;

                        case SongField.Name:
                            song.Name = _reader.ReadTerminatedString();
                            break;

                        case SongField.FormattedName:
                            song.FormattedName = _reader.ReadTerminatedString();
                            break;

                        case SongField.Artist:
                            song.Artist = _reader.ReadTerminatedString();
                            break;

                        case SongField.ChartId:
                            song.ChartId = _reader.ReadTerminatedString();
                            break;

                        case SongField.BPMDisplay:
                            song.BpmDisplay = _reader.ReadTerminatedString();
                            break;

                        case SongField.Version:
                            song.Version = _reader.ReadTerminatedString();
                            break;

                        case SongField.HasEncore:
                            song.HasEncore = _reader.ReadBoolean();
                            break;

                        case SongField.IsOriginal:
                            song.IsOriginal = _reader.ReadBoolean();
                            break;

                        case SongField.JacketArtist:
                            song.JacketArtist = _reader.ReadTerminatedString();
                            break;

                        case SongField.IsPublished:
                            song.IsPublished = _reader.ReadBoolean();
                            break;
                    }
                    break;
            }
        } while (ot != ObjectType.DATA_END);

        song.Charts = charts.Where(c => c.DifficultyConstant > 0).ToList();
        return song;
    }
}
