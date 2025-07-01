using SkiaSharp;

namespace ChartEd;

public static class EditorPaints
{
    public static readonly SKPaint LeftChipPaint = new()
    {
        Color = SKColors.Aqua,
        Style = SKPaintStyle.StrokeAndFill,
        StrokeCap = SKStrokeCap.Round
    };

    public static readonly SKPaint RightChipPaint = new()
    {
        Color = SKColors.LightPink,
        Style = SKPaintStyle.StrokeAndFill,
        StrokeCap = SKStrokeCap.Round
    };

    public static readonly SKPaint ErrorPaint = new()
    {
        Color = SKColors.Green,
        Style = SKPaintStyle.StrokeAndFill,
        StrokeCap = SKStrokeCap.Round
    };

    public static readonly SKPaint LeftBumperPaint = new()
    {
        Color = SKColors.Blue,
        Style = SKPaintStyle.StrokeAndFill,
        StrokeCap = SKStrokeCap.Round
    };

    public static readonly SKPaint RightBumperPaint = new()
    {
        Color = SKColors.Red,
        Style = SKPaintStyle.StrokeAndFill,
        StrokeCap = SKStrokeCap.Round
    };

    public static readonly SKPaint MiddleBumperPaint = new()
    {
        Color = SKColors.MediumPurple,
        Style = SKPaintStyle.StrokeAndFill,
        StrokeCap = SKStrokeCap.Round
    };

    public static readonly SKPaint MinePaint = new()
    {
        Color = SKColors.DarkGray,
        Style = SKPaintStyle.StrokeAndFill,
        StrokeCap = SKStrokeCap.Round
    };

    public static readonly SKPaint MarkerTextPaint = new()
    {
        Color = SKColors.White,
        StrokeWidth = 10.0f,
    };

    public static readonly SKPaint MarkerLinePaint = new()
    {
        Color = SKColors.White,
        StrokeWidth = 2,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke
    };
}
