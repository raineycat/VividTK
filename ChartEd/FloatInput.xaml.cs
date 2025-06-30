using System.Windows;
using System.Windows.Controls;

namespace ChartEd;

/// <summary>
/// Interaction logic for FloatInput.xaml
/// </summary>
public partial class FloatInput : UserControl
{
    public float IncrementAmount { get; set; } = 0.5f;

    private string _formatCode = "F3";
    public string FormatCode
    {
        get => _formatCode;
        set
        {
            _formatCode = value;
            UpdateDisplayedValue();        
        }
    }

    private float _value = 0;
    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            UpdateDisplayedValue();
        }
    }

    public event EventHandler<ValueChangedEventArgs>? ValueChanged;

    public FloatInput()
    {
        InitializeComponent();
    }

    private void HandleDecrement(object sender, RoutedEventArgs e)
    {
        _value -= IncrementAmount;
        UpdateDisplayedValue();
        ValueChanged?.Invoke(this, new ValueChangedEventArgs(_value));
    }

    private void HandleIncrement(object sender, RoutedEventArgs e)
    {
        Value += IncrementAmount;
        UpdateDisplayedValue();
        ValueChanged?.Invoke(this, new ValueChangedEventArgs(_value));
    }

    private void HandleValueChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(ValueInput.Text, out var parsed))
        {
            _value = parsed;
            ValueChanged?.Invoke(this, new ValueChangedEventArgs(_value));
        }
    }

    private void UpdateDisplayedValue()
    {
        ValueInput.Text = _value.ToString(_formatCode);
    }

    public class ValueChangedEventArgs : EventArgs
    {
        public float NewValue { get; set; }

        public ValueChangedEventArgs(float val)
        {
            NewValue = val;
        }
    }
}
