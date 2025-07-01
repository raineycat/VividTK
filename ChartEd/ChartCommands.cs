using System.Windows.Input;

namespace ChartEd;

public static class ChartCommands
{
    public static RoutedUICommand ClearNotes { get; } = new(
        "Clear all notes",
        nameof(ClearNotes),
        typeof(ChartCommands),
        [new KeyGesture(Key.C, ModifierKeys.Alt | ModifierKeys.Shift)]
        );

    public static RoutedUICommand GimmickEditor { get; } = new(
        "Clear all notes",
        nameof(GimmickEditor),
        typeof(ChartCommands),
        [new KeyGesture(Key.G, ModifierKeys.Alt)]
        );
}
