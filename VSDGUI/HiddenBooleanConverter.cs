using System.Windows;
using System.Windows.Data;

namespace VSDGUI;

public class HiddenBooleanConverter : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var isVisible = (bool)value;
        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return (Visibility)value == Visibility.Visible;
    }
}