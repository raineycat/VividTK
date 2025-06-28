using System.ComponentModel;

namespace VividTK.VSFormatLib.Chart;

public enum Easing : byte
{
    Linear = 1,
    OutElastic = 2,
    InExpo = 3,
    OutExpo = 4,
    InOutExpo = 5,
    InQuad = 6,
    OutQuad = 7,
    InOutQuad = 8,
    InCubic = 9,
    OutCubic = 10,
    InOutCubic = 11,
    OutBack = 12,
    InSine = 13,
    OutSine = 14,
    InOutSine = 15,
    OutQuart = 16,
    InOutCirc = 17,
    InCirc = 18,
    OutCirc = 19
}

public static class EasingHelper
{
    public static Easing FromString(string s) => s switch
    {
        "linear" => Easing.Linear,
        "outElastic" => Easing.OutElastic,
        "inExpo" => Easing.InExpo,
        "outExpo" => Easing.OutExpo,
        "inOutExpo" => Easing.InOutExpo,
        "inQuad" => Easing.InQuad,
        "outQuad" => Easing.OutQuad,
        "inOutQuad" => Easing.InOutQuad,
        "inCubic" => Easing.InCubic,
        "outCubic" => Easing.OutCubic,
        "inOutCubic" => Easing.InOutCubic,
        "outBack" => Easing.OutBack,
        "inSine" => Easing.InSine,
        "outSine" => Easing.OutSine,
        "inOutSine" => Easing.InOutSine,
        "outQuart" => Easing.OutQuart,
        "inOutCirc" => Easing.InOutCirc,
        "inCirc" => Easing.InCirc,
        "outCirc" => Easing.OutCirc,
        _ => throw new ArgumentOutOfRangeException(nameof(s)),
    };
}