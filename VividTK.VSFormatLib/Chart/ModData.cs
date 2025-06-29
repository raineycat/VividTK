namespace VividTK.VSFormatLib.Chart;

public struct ModData
{
    public float StartOffset; // originally 'b', offset in beats
    public float Duration; // originally 'd', length in beats
    public Easing Ease;
    public float From; // originally 'v1'
    public float To; // originally 'v2'
    public ModType Type;
    public sbyte ProxyIndex;
    public float Weight;
}