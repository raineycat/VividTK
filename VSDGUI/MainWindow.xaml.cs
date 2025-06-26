using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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
    
    private GridViewColumnHeader? _listViewSortCol;
    private SortAdorner? _listViewSortAdorner;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        OpenFile = new VSDReader();
        SongList.ItemsSource = new List<SongInfo>();
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

    private void NewCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        if(OpenFile != null)
        {
            if(MessageBox.Show(
                this, 
                "Are you sure you want to discard any changes and start a new file?", 
                "VSDGUI",
                MessageBoxButton.YesNo,
                MessageBoxImage.Exclamation) 
            != MessageBoxResult.Yes)
            {
                return;
            }

            OpenFile = null;
            OpenFilePath = null;
        }

        OpenFile = new VSDReader();
        SongList.ItemsSource = new List<SongInfo>();
    }

    private void SaveCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        if (OpenFile == null) return;
        OpenFile.Songs = SongList.ItemsSource.Cast<SongInfo>().ToList();

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
        OpenFile.Songs = SongList.ItemsSource.Cast<SongInfo>().ToList();

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
        if (OpenFile == null) return;
        OpenFile.Songs = SongList.ItemsSource.Cast<SongInfo>().ToList();

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

    private void AddSongCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var infoList = SongList.ItemsSource.Cast<SongInfo>();
        var lastId = infoList.Any() ? infoList.Max(i => i.SongId) : 1;
        SongList.ItemsSource = infoList.Append(new SongInfo(lastId + 1));
    }

    private void RemoveSongsCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var toRemove = SongList.SelectedItems.Cast<SongInfo>().Select(i => i.SongId);
        var source = SongList.ItemsSource.Cast<SongInfo>();
        SongList.ItemsSource = source.Where(i => !toRemove.Contains(i.SongId)).ToList();
        SelectionList.ItemsSource = null;
    }

    private void AddChartCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var selectedSong = (SongInfo)SongList.SelectedItem;
        var lastIndex = selectedSong.Charts.Count;
        selectedSong.Charts.Add(new ChartInfo
        {
            Index = lastIndex + 1
        });
        SelectionList.ItemsSource = new List<SongInfo> { selectedSong };
    }

    private void RemoveChartCommandHandler(object sender, ExecutedRoutedEventArgs e)
    {
        var song = (SongInfo)SongList.SelectedItem;

        if (e.Parameter == null)
        {
            song.Charts.RemoveAt(song.Charts.Count - 1);
        } 
        else
        {
            var index = (int)e.Parameter;
            song.Charts.RemoveAt(index - 1); // the Index property starts at 1
        }

        // fix indices
        song.Charts = song.Charts.Select((c, i) =>
        {
            c.Index = i + 1;
            return c;
        }).ToList();

        SelectionList.ItemsSource = new List<SongInfo> { song };
    }

    private void CanExecuteIfSingleSongSelected(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = SongList.SelectedItems.Count == 1;
    }

    private void CanExecuteIfAnySongsSelected(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = SongList.SelectedItems.Count > 0;
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
    
    private void ColumnHeaderClickHandler(object sender, RoutedEventArgs e)
    {
        var column = (sender as GridViewColumnHeader);
        if(column == null) return;
        
        var sortBy = column.Tag.ToString();
        if(sortBy == null) return;
        
        if(_listViewSortCol != null)
        {
            if(_listViewSortAdorner != null)
                AdornerLayer.GetAdornerLayer(_listViewSortCol)?.Remove(_listViewSortAdorner);
            SongList.Items.SortDescriptions.Clear();
        }

        var newDir = ListSortDirection.Ascending;
        if(_listViewSortCol == column && _listViewSortAdorner != null && _listViewSortAdorner.Direction == newDir)
            newDir = ListSortDirection.Descending;

        _listViewSortCol = column;
        _listViewSortAdorner = new SortAdorner(_listViewSortCol, newDir);
        AdornerLayer.GetAdornerLayer(_listViewSortCol)?.Add(_listViewSortAdorner);
        SongList.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
    }
}