namespace VividTK.VSFormatLib.Chart;

public struct ModData
{
    public float StartOffset; // originally 'b', offset in beats
    public float Duration; // originally 'd', length in beats
    public Easing Ease;
    public float V1;
    public float V2;
    public ModType Type;
    public char ProxyIndex;
    public float Weight;
}