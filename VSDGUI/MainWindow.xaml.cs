using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using VividTK.VSFormatLib;
using VividTK.VSFormatLib.VSD;

namespace VSDGUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public VSDReader? OpenFile { get; private set; }
    public string? OpenFilePath { get; private set; }
    
    public MainWindow()
    {
        InitializeComponent();
    }

    public void OpenCommandHandler(object sender, ExecutedRoutedEventArgs ev)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Open a VSD file...",
            Filter = "VSD Files|*.bin;*.vsd|All files|*.*",
            CheckFileExists = true,
            FileName = "song_information.bin"
        };

        if (!(dialog.ShowDialog(this) ?? false))
        {
            return;
        }

        try
        {
            OpenFile = VSFile.ReadVSD(dialog.FileName);
            OpenFilePath = dialog.FileName;
            SongList.ItemsSource = OpenFile.Songs;
            // MessageBox.Show(this, "Loaded!");
        }
        catch (Exception e)
        {
            MessageBox.Show(this, $"Failed to read file!\n\n{e}");
        }
    }

    private void SongList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectionList.ItemsSource = SongList.SelectedItems;
    }

    private void SaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        if (OpenFile == null) return;

        if (OpenFilePath == null)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Save VSD...",
                Filter = "VSD files|*.bin;*.vsd|All files|*.*",
                FileName = "song_information.bin",
                AddExtension = true
            };
            
            if (dialog.ShowDialog(this) ?? false)
            {
                OpenFilePath = dialog.FileName;
            }
            else
            {
                return;
            }
        }

        using var s = File.OpenWrite(OpenFilePath);
        using var writer = new BinaryWriter(s);
        VSDWriter.WriteSongList(OpenFile.Songs, writer);
        
        MessageBox.Show(
            this, 
            "Saved!", 
            "VSD Editor", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    private void SaveAsCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        if (OpenFile == null) return;

        var dialog = new SaveFileDialog
        {
            Title = "Save VSD...",
            Filter = "VSD files|*.bin;*.vsd|All files|*.*",
            FileName = "song_information.bin",
            AddExtension = true
        };

        if (!(dialog.ShowDialog(this) ?? false)) return;
        
        using var s = File.OpenWrite(dialog.FileName);
        using var writer = new BinaryWriter(s);
        VSDWriter.WriteSongList(OpenFile.Songs, writer);

        MessageBox.Show(
            this, 
            "Saved!", 
            "VSD Editor", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    private void ExportCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var dialog = new SaveFileDialog
        {
            Title = "Select JSON export path...",
            Filter = "JSON Files|*.json|All files|*.*",
            AddExtension = true
        };
        
        if (!(dialog.ShowDialog(this) ?? false)) return;

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };
        
        options.Converters.Add(new JsonStringEnumConverter());
        var json = JsonSerializer.Serialize(OpenFile, options);
        File.WriteAllText(dialog.FileName, json);
        
        MessageBox.Show(
            this, 
            "Exported!", 
            "VSD Editor", 
            MessageBoxButton.OK, 
            MessageBoxImage.Information);
    }

    private void HelpCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        new AboutDialog
        {
            Owner = this
        }.ShowDialog();
    }

    private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        if (MessageBox.Show(
                this, 
                "Are you sure you want to exit?", 
                "VSD Editor",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) 
            == MessageBoxResult.Yes)
        {
            Environment.Exit(0);   
        }
    }

    private void DropFileHandler(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            return;
        }
        
        try
        {
            var filePaths = e.Data.GetData(DataFormats.FileDrop) as string[];
            if(filePaths == null || filePaths.Length < 1) return;
            
            OpenFile = VSFile.ReadVSD(filePaths[0]);
            OpenFilePath = filePaths[0];
            SongList.ItemsSource = OpenFile.Songs;
            // MessageBox.Show(this, "Loaded!");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to read file!\n\n{ex}");
        }
    }
}