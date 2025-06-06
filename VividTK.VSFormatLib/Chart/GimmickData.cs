namespace VividTK.VSFormatLib.Chart;

public struct GimmickData
{
    public byte Proxies { get; set; }
    public string ObjectName { get; set; }
    public List<ModData> Mods { get; set; }
    public List<PerFrameData> PerFrame { get; set; }
}