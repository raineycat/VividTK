namespace VividTK.VSFormatLib.VSD;

public class VSDReader
{
    private readonly BinaryReader _reader;

    public List<SongInfo> Songs { get; } = new();

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
        var song = new SongInfo(0);
        var charts = new List<object>();

        ObjectType ot;
        do
        {
            ot = (ObjectType)_reader.ReadByte();
            switch (ot)
            {
                case ObjectType.CHART_INFO:
                    var chart = new ChartInfo();
                    chart.DifficultyDisplay = _reader.ReadTerminatedString();
                    chart.DifficultyConstant = _reader.ReadSingle();
                    chart.NoteDesigner = _reader.ReadTerminatedString();
                    charts.Add(chart);
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
                            song.BPMDisplay = _reader.ReadTerminatedString();
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

                default:
                    break;
            }
        } while (ot != ObjectType.DATA_END);

        foreach (var c in charts)
        {

        }

        return song;
    }
}
