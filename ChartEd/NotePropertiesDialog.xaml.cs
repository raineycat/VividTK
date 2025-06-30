using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using VividTK.VSFormatLib.Chart;

namespace ChartEd;

public partial class NotePropertiesDialog : Window
{
    public NoteData Note { get; private set; }
    private NoteData _modifiedNote;

    private AdornerLayer? _adornerLayer;
    private CheckMarkAdorner? _currentAdorner;

    public NotePropertiesDialog(NoteData note)
    {
        Note = note;
        _modifiedNote = note;
        InitializeComponent();
    }

    private void HandleWindowLoaded(object sender, RoutedEventArgs e)
    {
        NoteTypeBox.ItemsSource = Enum.GetValues<NoteType>().Select(v => v.ToString());
        NoteTypeBox.SelectedValue = _modifiedNote.Type.ToString();

        TimeInput.Value = _modifiedNote.Time / 1000f;
        EndTimeInput.Value = _modifiedNote.EndTime / 1000f;
        TempoInput.Value = _modifiedNote.BPM;

        foreach (var obj in LaneSelector.Children)
        {
            if (obj is Rectangle rect && rect.Tag is string id)
            {
                if (id == _modifiedNote.Lane.ToString())
                {
                    ShowCheckMark(rect);
                    break;
                }
            }
        }
    }

    private void HandleCloseDialog(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Note = _modifiedNote;
        Close();
    }

    private void HandleNoteTypeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count < 1) return;
        _modifiedNote.Type = Enum.Parse<NoteType>((e.AddedItems[0] as string) ?? "Chip");
    }

    private void HandleLaneClicked(object sender, MouseButtonEventArgs e)
    {
        if (sender is not Rectangle rect) return;
        if (rect.Tag is not string id) return;
        if (!Enum.TryParse<LaneType>(id, out var lane)) return;

        Debug.WriteLine("Clicked on lane: " + lane);
        if(_modifiedNote.Lane != lane)
        {
            ShowCheckMark(rect);
            _modifiedNote.Lane = lane;
        }
    }

    private void ShowCheckMark(Rectangle target)
    {
        if (_adornerLayer == null)
            _adornerLayer = AdornerLayer.GetAdornerLayer(target);

        if (_currentAdorner != null)
            _adornerLayer.Remove(_currentAdorner);

        _currentAdorner = new CheckMarkAdorner(target);
        _adornerLayer.Add(_currentAdorner);
    }

    private void TimeInput_ValueChanged(object sender, FloatInput.ValueChangedEventArgs e)
    {
        _modifiedNote.Time = e.NewValue * 1000f;
    }

    private void EndTimeInput_ValueChanged(object sender, FloatInput.ValueChangedEventArgs e)
    {
        _modifiedNote.EndTime = e.NewValue * 1000f;
    }

    private void TempoInput_ValueChanged(object sender, FloatInput.ValueChangedEventArgs e)
    {
        _modifiedNote.BPM = e.NewValue;
    }
}