using System.Text;
using VividTK.VSFormatLib;
using VividTK.VSFormatLib.Chart;

if (args.Length < 1)
{
    Console.WriteLine("Error: no chart provided!");
    Console.WriteLine("Usage: ./ChartBuilder <chart/notes.vsc> [chart/mods.vsm]");
    Environment.Exit(1);
}

var isTextChart = args[0].EndsWith(".vsc");
var chart = VSFile.ReadSingleChart(args[0], isTextChart);
Console.WriteLine("Loaded chart!");

if (args.Length > 1)
{
    using var reader = new StreamReader(args[1]);
    (chart as TextChartReader)?.LoadModFile(reader);
    Console.WriteLine("Loaded modifier data!");
}

Console.WriteLine($"->  {chart.Notes.Count} notes");
if(chart.Gimmick.Mods != null) Console.WriteLine($"->  {chart.Gimmick.Mods.Count} mods");
if(chart.Gimmick.PerFrame != null) Console.WriteLine($"->  {chart.Gimmick.PerFrame.Count} MPF");

var outputPath = Path.GetFileNameWithoutExtension(args[0]) + "_ChartBuilder.vsb";
Console.WriteLine($"Writing to: {outputPath}");

using var writer = new BinaryWriter(File.OpenWrite(outputPath), Encoding.ASCII);
BinaryChartWriter.WriteChart(chart, writer);
Console.WriteLine("Done!");