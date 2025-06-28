namespace VividTK.VSFormatLib.Chart;

public interface IChartReader
{
    public List<NoteData> Notes { get; }
    public GimmickData Gimmick { get; }
}
