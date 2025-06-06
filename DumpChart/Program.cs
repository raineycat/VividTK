using System.Text.Json;
using System.Text.Json.Serialization;

if (args.Length < 1)
{
    Console.WriteLine("Error: no file specified!");
    Console.WriteLine("Usage: ./DumpChart <path/to/chart/VERSION.vsb>");
    Environment.Exit(1);
}

var path = args[0];
var chart = VividTK.VSFormatLib.VSFile.ReadSingleChart(path);

var serializeSettings = new JsonSerializerOptions
{
    WriteIndented = true,
    IncludeFields = true,
    IgnoreReadOnlyFields = true,
    Converters = { new JsonStringEnumConverter() }
};

var jsonString = JsonSerializer.Serialize(chart, serializeSettings);
var outputPath = Path.ChangeExtension(path, "chart.json");
File.WriteAllText(outputPath, jsonString);
Console.WriteLine("Saved data to: " + outputPath);