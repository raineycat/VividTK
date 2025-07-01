using System.Windows.Input;

namespace ChartEd;

public static class ChartCommands
{
    public static RoutedUICommand ClearNotesEarlier { get; } = new(
        "Clear earlier notes",
        nameof(ClearNotesEarlier),
        typeof(ChartCommands),
        [new KeyGesture(Key.E, ModifierKeys.Alt | ModifierKeys.Shift)]
        );

    public static RoutedUICommand ClearNotesLater { get; } = new(
        "Clear later notes",
        nameof(ClearNotesLater),
        typeof(ChartCommands),
        [new KeyGesture(Key.D, ModifierKeys.Alt | ModifierKeys.Shift)]
        );

    public static RoutedUICommand ClearNotes { get; } = new(
        "Clear all notes",
        nameof(ClearNotes),
        typeof(ChartCommands),
        [new KeyGesture(Key.C, ModifierKeys.Alt | ModifierKeys.Shift)]
        );

    public static RoutedUICommand GimmickEditor { get; } = new(
        "Open Gimmick editor",
        nameof(GimmickEditor),
        typeof(ChartCommands),
        [new KeyGesture(Key.G, ModifierKeys.Alt)]
        );
}
