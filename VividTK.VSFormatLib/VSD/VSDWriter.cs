namespace VividTK.VSFormatLib.VSD;

public static class VSDWriter
{
    public static void WriteSongList(List<SongInfo> songs, BinaryWriter writer)
    {
        writer.Write('V');
        writer.Write('S');
        writer.Write('D');
        writer.Write((byte)1);
        writer.Write((byte)0);

        foreach (var song in songs)
        {
            WriteSong(song, writer);
        }
        
        writer.Write(ObjectType.DATA_END);
    }

    private static void WriteSong(SongInfo song, BinaryWriter writer)
    {
        writer.Write(ObjectType.SONG_INFO);
        
        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.U32);
        writer.Write(SongField.SongId);
        writer.Write(song.SongId);
        
        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.String);
        writer.Write(SongField.Name);
        writer.WriteTerminatedString(song.Name);
        
        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.String);
        writer.Write(SongField.FormattedName);
        writer.WriteTerminatedString(song.FormattedName);
        
        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.String);
        writer.Write(SongField.Artist);
        writer.WriteTerminatedString(song.Artist);
        
        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.String);
        writer.Write(SongField.ChartId);
        writer.WriteTerminatedString(song.ChartId);
        
        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.String);
        writer.Write(SongField.BPMDisplay);
        writer.WriteTerminatedString(song.BpmDisplay);
        
        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.String);
        writer.Write(SongField.Version);
        writer.WriteTerminatedString(song.Version);
        
        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.Bool);
        writer.Write(SongField.HasEncore);
        writer.Write(song.HasEncore);
        
        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.Bool);
        writer.Write(SongField.IsOriginal);
        writer.Write(song.IsOriginal);

        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.String);
        writer.Write(SongField.JacketArtist);
        writer.WriteTerminatedString(song.JacketArtist);
        
        writer.Write(ObjectType.SONG_FIELD);
        writer.Write(SongFieldType.Bool);
        writer.Write(SongField.IsPublished);
        writer.Write(song.IsPublished);

        foreach (var chart in song.Charts)
        {
            WriteChart(chart, writer);
        }
        
        writer.Write(ObjectType.DATA_END);
    }

    private static void WriteChart(ChartInfo chart, BinaryWriter writer)
    {
        writer.Write(ObjectType.CHART_INFO);
        writer.WriteTerminatedString(chart.DifficultyDisplay);
        writer.Write(chart.DifficultyConstant);
        writer.WriteTerminatedString(chart.NoteDesigner);
    }
}