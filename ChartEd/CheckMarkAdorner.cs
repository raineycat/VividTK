using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ChartEd;

public class CheckMarkAdorner : Adorner
{
    private readonly VisualCollection _visuals;
    private readonly TextBlock _checkMark;

    public CheckMarkAdorner(UIElement adornedElement) : base(adornedElement)
    {
        _checkMark = new TextBlock
        {
            Text = "✔",
            FontSize = 12,
            Foreground = Brushes.Black,
            Background = Brushes.Transparent,
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Top
        };

        _visuals = new VisualCollection(this)
        {
            _checkMark
        };
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _checkMark.Arrange(new Rect(finalSize.Width - 30, 0, 30, 30));
        return finalSize;
    }

    protected override Visual GetVisualChild(int index) => _visuals[index];
    protected override int VisualChildrenCount => _visuals.Count;
}
