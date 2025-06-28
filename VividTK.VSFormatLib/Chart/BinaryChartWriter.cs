using VividTK.VSFormatLib.VSD;

namespace VividTK.VSFormatLib.Chart;

public static class BinaryChartWriter
{
    public static void WriteChart(IChartReader chart, BinaryWriter writer)
    {
        writer.Write('V');
        writer.Write('S');
        writer.Write('C');
        writer.Write((byte)1);
        writer.Write((byte)0);

        writer.Write(ChartDataType.NoteListStart);
        foreach(var note in chart.Notes)
        {
            WriteNote(note, writer);
        }
        writer.Write(ChartDataType.NoteListEnd);

        WriteGimmickData(chart.Gimmick, writer);

        writer.Write(ChartDataType.ChartEnd);
    }

    private static void WriteNote(NoteData note, BinaryWriter writer)
    {
        writer.Write(ChartDataType.NoteEntryStart);

        writer.Write(ChartDataType.NoteEntryType);
        writer.Write((byte)note.Type);

        writer.Write(ChartDataType.NoteEntryLane);
        writer.Write(FixNoteLane(note.Lane));

        writer.Write(ChartDataType.NoteEntryTime);
        writer.Write(note.Time);

        switch(note.Type)
        {
            case NoteType.Hold: // 2
                writer.Write(ChartDataType.NoteEntryExtra);
                writer.WriteExtraMarkup(1, SongFieldType.S32, (int)note.EndTime);
                writer.Write(ChartDataType.NoteExtraEnd);
                break;

            case NoteType.TempoChange: // 3
                writer.Write(ChartDataType.NoteEntryExtra);
                writer.WriteExtraMarkup(1, SongFieldType.F32, note.BPM);
                writer.Write(ChartDataType.NoteExtraEnd);
                break;
        }

        writer.Write(ChartDataType.NoteEntryEnd);
    }

    private static void WriteGimmickData(GimmickData data, BinaryWriter writer)
    {
        if (data.ObjectName == null) return;

        writer.Write(ChartDataType.GimmickStart);

        writer.Write(ChartDataType.GimmickProxies);
        writer.Write(data.Proxies);

        writer.Write(ChartDataType.GimmickObjectName);
        writer.WriteTerminatedString(data.ObjectName);

        writer.Write(ChartDataType.GimmickModData);

        foreach(var mod in data.Mods)
        {
            writer.Write(ChartDataType.GimmickModEntry);
            writer.Write(mod.StartOffset);
            writer.Write(mod.Duration);
            writer.Write((byte)mod.Ease);
            writer.Write(mod.From);
            writer.Write(mod.To);
            writer.Write((byte)mod.Type);
            writer.Write(mod.ProxyIndex);
        }

        foreach(var perFrame in data.PerFrame)
        {
            writer.Write(ChartDataType.GimmickPerFrameEntry);
            writer.Write(perFrame.B);
            writer.Write(perFrame.E);
            writer.WriteTerminatedString(perFrame.FunctionName);
        }

        writer.Write(ChartDataType.GimmickModEnd);
        writer.Write(ChartDataType.GimmickEnd);
    }

    private static void WriteExtraMarkup(this BinaryWriter writer, byte fieldId, SongFieldType type, object value)
    {
        writer.Write(SongFieldTypeHelper.FieldTypeToByte(type));
        writer.Write(fieldId);
        writer.WriteSongField(value, type);
    }

    private static byte FixNoteLane(LaneType lane) => lane switch
    {
        LaneType.Lane1 or LaneType.Lane2 or LaneType.Lane3 or LaneType.Lane4 => (byte)lane,
        LaneType.LeftBumper => (byte)LaneType.Lane1,
        LaneType.MiddleBumper => (byte)LaneType.Lane2,
        LaneType.RightBumper => (byte)LaneType.Lane3,
        _ => 0,
    };
}
