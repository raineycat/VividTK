using System.Windows.Input;

namespace VSDGUI;

public static class EditCommands
{
    public static RoutedUICommand AddSong = new(
        "Add song",
        "AddSong",
        typeof(EditCommands),
        [new KeyGesture(Key.S, ModifierKeys.Alt)]
        );

    public static RoutedUICommand RemoveSongs = new(
        "Remove songs",
        "RemoveSongs",
        typeof(EditCommands),
        [new KeyGesture(Key.S, ModifierKeys.Alt | ModifierKeys.Shift)]
        );

    public static RoutedUICommand AddChart = new(
        "Add chart",
        "AddChart",
        typeof(EditCommands),
        [new KeyGesture(Key.C, ModifierKeys.Alt)]
        );

    public static RoutedUICommand RemoveChart = new(
        "Remove chart",
        "RemoveChart",
        typeof(EditCommands),
        [new KeyGesture(Key.C, ModifierKeys.Alt | ModifierKeys.Shift)]
        );
}
