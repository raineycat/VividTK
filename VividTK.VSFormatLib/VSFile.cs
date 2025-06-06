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

    public static ChartReader ReadSingleChart(string path)
    {
        using var stream = File.OpenRead(path);
        using var binaryReader = new BinaryReader(stream);
        var chart = new ChartReader(binaryReader);
        return chart;
    }
}
