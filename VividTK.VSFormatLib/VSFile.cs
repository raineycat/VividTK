using VividTK.VSFormatLib.Chart;
using VividTK.VSFormatLib.VSD;

namespace VividTK.VSFormatLib;

public static class VSFile
{
    public static VSDReader ReadVSD(string path)
    {
        using var stream = File.OpenRead(path);
        using var binaryReader = new BinaryReader(stream);
        var vsd = new VSDReader(binaryReader);
        return vsd;
    }

    public static IChartReader ReadSingleChart(string chartPath, bool text = false)
    {
        using var stream = File.OpenRead(chartPath);
        IChartReader chart;

        if(text)
        {
            using var reader = new StreamReader(stream);
            chart = new TextChartReader(reader);
        } else
        {
            using var reader = new BinaryReader(stream);
            chart = new BinaryChartReader(reader);
        }

        return chart;
    }
}
