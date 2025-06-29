namespace VividTK.VSFormatLib.Chart;

public class TextChartReader : IChartReader
{
    public List<NoteData> Notes { get; set; } = [];
    public GimmickData Gimmick { get; set; }

    public TextChartReader(TextReader reader)
    {
        string? line;
        while ((line = reader.ReadLine()) != null) {
            var parts = line.Split(',');

            var time = float.Parse(parts[0]);
            var type = (NoteType)byte.Parse(parts[1]);
            var lane = (LaneType)byte.Parse(parts[2]);

            var push = true;
            var extraData = new Dictionary<string, string>();
            var endTime = time;

            if (parts.Length > 3)
            {
                var extra = parts[3];

                if(type == NoteType.TempoChange)
                {
                    var e = ParseExtraValue(extra);
                    if(e != null)
                    {
                        extraData = e;
                    } 
                    else
                    {
                        push = false;
                    }
                } 
                else if (type == NoteType.Hold)
                {
                    endTime = float.Parse(extra);
                }
            }

            var note = new NoteData
            {
                Time = time,
                Lane = lane,
                Type = type
            };

            if(extraData.TryGetValue("bpm", out var bpm))
            {
                note.BPM = float.Parse(bpm);
            }

            if(push)
            {
                Notes.Add(note);
            }
        }
    }

    private Dictionary<string, string>? ParseExtraValue(string extra)
    {
        if(string.IsNullOrWhiteSpace(extra))
        {
            return null;
        }

        var map = new Dictionary<string, string>();
        var isReadingValue = false;
        var readKey = "";
        var readValue = "";
        string? fullKey;

        foreach (char c in extra)
        {
            if(c == ':')
            {
                isReadingValue = true;
            }
            else if(c == '|')
            {
                isReadingValue = false;
                fullKey = readKey switch
                {
                    "t" => "time",
                    "b" => "bpm",
                    "v" => "velocity",
                    "s" => "signature",
                    _ => null
                };
                if(fullKey != null)
                {
                    map[fullKey] = readValue;
                }
            }
            else if(isReadingValue)
            {
                readValue += c;
            } else
            {
                readKey += c;
            }
        }

        fullKey = readKey switch
        {
            "t" => "time",
            "b" => "bpm",
            "v" => "velocity",
            "s" => "signature",
            _ => null
        };
        if (fullKey != null)
        {
            map[fullKey] = readValue;
        }

        return map;
    }

    public void LoadModFile(TextReader reader)
    {
        var mode = ModReadingMode.Mods;
        var data = new GimmickData();

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            line = line.Trim();

            if(string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if(line == "mpf")
            {
                mode = ModReadingMode.PerFrame;
                continue;
            }

            if(line.StartsWith("!"))
            {
                var parts = line.Substring(1).Split(':');
                switch(parts[0].Trim())
                {
                    case "proxies":
                        data.Proxies = byte.Parse(parts[1].Trim());
                        break;
                    case "obj":
                        data.ObjectName = parts[1].Trim();
                        break;
                    default:
                        throw new InvalidOperationException("Unknown property!");
                }
                continue;
            }

            if(mode == ModReadingMode.Mods)
            {
                var parts = line.Split(',');
                var mod = new ModData();

                mod.Duration = float.Parse(parts[1]);
                mod.Ease = EasingHelper.FromString(parts[2]);
                mod.Type = ModTypeHelper.FromString(parts[5]);
                mod.ProxyIndex = (sbyte)int.Parse(parts[6]);

                var beatParts = parts[0].Split(':');
                var beatStart = float.Parse(beatParts[0]);

                var beatEnd = beatStart;
                if (beatParts.Length > 1)
                    beatEnd = float.Parse(beatParts[1]);

                var beatIncrement = 1f;
                if (beatParts.Length > 2)
                    beatIncrement = float.Parse(beatParts[2]);

                var from = parts[3];
                var to = parts[4];

                mod.From = from == "_" ? 573613 : float.Parse(from);
                mod.To = to == "_" ? 573613 : float.Parse(to);

                mod.Weight = ModTypeHelper.GetModWeight(mod.Type);

                for(var beat = beatStart; beat <= beatEnd; beat += beatIncrement)
                {
                    // this copies `mod` for each beat
                    mod.StartOffset = beat;
                    data.Mods.Add(mod);
                }

                continue;
            }
            
            if(mode == ModReadingMode.PerFrame)
            {
                var parts = line.Split(',');
                data.PerFrame.Add(new PerFrameData
                {
                    B = float.Parse(parts[0]),
                    E = float.Parse(parts[1]),
                    FunctionName = parts[2]
                });
            }
        }

        Gimmick = data;
    }

    private enum ModReadingMode
    {
        Mods,
        PerFrame
    }
}
