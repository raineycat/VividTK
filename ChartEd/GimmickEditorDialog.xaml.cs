using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using VividTK.VSFormatLib.Chart;

namespace ChartEd;

public partial class GimmickEditorDialog : Window
{
    public GimmickData Data { get; private set; }
    private GimmickData _modifiedData;

    public GimmickEditorDialog(GimmickData data)
    {
        Data = data;
        _modifiedData = data;
        InitializeComponent();
    }

    private void HandleWindowLoaded(object sender, RoutedEventArgs e)
    {
        
    }

    private void HandleCloseDialog(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Data = _modifiedData;
        Close();
    }
}