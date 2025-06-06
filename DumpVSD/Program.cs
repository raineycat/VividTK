using System.Text.Json;
using VividTK.VSFormatLib;

if (args.Length < 1)
{
    Console.WriteLine("Error: no file specified!");
    Console.WriteLine("Usage: ./DumpVSD <path/to/file.vsd>");
    Environment.Exit(1);
}

var path = args[0];
var vsd = VSFile.ReadVSD(path);

var serializeSettings = new JsonSerializerOptions
{
    WriteIndented = true,
    IncludeFields = true,
    IgnoreReadOnlyFields = true
};

var jsonString = JsonSerializer.Serialize(vsd, serializeSettings);
var outputPath = Path.ChangeExtension(path, "vsd.json");
File.WriteAllText(outputPath, jsonString);
Console.WriteLine("Saved data to: " + outputPath);