namespace VividTK.VSFormatLib.VSD
{
    // Constructor values taken from getGenericSong
    // (in load_song_information global script)
    public class SongInfo(uint id)
    {
        public uint SongId { get; set; } = id;
        public string Name { get; set; } = "N/A";
        public string FormattedName { get; set; } = "N/A";
        public string Artist { get; set; } = "N/A";
        public string ChartId { get; set; } = "generic";
        public string BpmDisplay { get; set; } = "120";
        public string Version { get; set; } = "9.9.9";
        public bool HasEncore { get; set; } = false;
        public bool IsOriginal { get; set; } = false;
        public bool IsPublished { get; set; } = false;
        public string JacketArtist { get; set; } = "N/A";

        public List<ChartInfo> Charts { get; set; } = [];
    }
}
