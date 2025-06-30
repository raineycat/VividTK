
namespace VividTK.VSFormatLib.Chart;

public class InMemoryChartReader : IChartReader
{
    public List<NoteData> Notes { get; set; } = [];

    public GimmickData Gimmick { get; set; }
}
