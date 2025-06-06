namespace VividTK.VSFormatLib.VSD
{
    // Constructor values taken from getGenericSong
    // (in load_song_information global script)
    public class SongInfo(uint id)
    {
        public uint SongId = id;
        public string Name = "N/A";
        public string FormattedName = "N/A";
        public string Artist = "N/A";
        public string ChartId = "generic";
        public object AudioId = null; //TODO
        public object Jacket = null; //TODO
        public object PreviewId = null; //TODO
        public string BPMDisplay = "120";
        public string Version = "9.9.9";
        public bool HasEncore = false;
        public bool IsOriginal = false;
        public bool IsPublished = false;
        public string JacketArtist = "N/A";

        public int DifficultyConstant1 = 0;
        public string DIfficultyDisplay1 = "0";
        public string NoteDesigner1 = "N/A";

        public int DifficultyConstant2 = 0;
        public string DIfficultyDisplay2 = "0";
        public string NoteDesigner2 = "N/A";

        public int DifficultyConstant3 = 0;
        public string DIfficultyDisplay3 = "0";
        public string NoteDesigner3 = "N/A";

        public int DifficultyConstant4 = 0;
        public string DIfficultyDisplay4 = "0";
        public string NoteDesigner4 = "N/A";
    }
}
