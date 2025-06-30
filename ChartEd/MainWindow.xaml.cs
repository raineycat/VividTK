using Microsoft.Win32;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using VividTK.VSFormatLib;
using VividTK.VSFormatLib.Chart;

namespace ChartEd;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private IChartReader? _chart;
    private float _timelineScroll;
    private float _timelineScale = 0.5f;
        
    public MainWindow()
    {
        InitializeComponent();
    }

    private void HandleWindowLoaded(object sender, RoutedEventArgs e)
    {
        PlacingNoteInput.ItemsSource = Enum.GetValues<NoteType>().Select(t => t.ToString());
        PlacingNoteInput.SelectedIndex = 0;
    }

    public void NewCommandHandler(object sender, ExecutedRoutedEventArgs ev)
    {
        _chart = new InMemoryChartReader();
        Timeline.InvalidateVisual(); // no idea if this is needed or not but eh
    }
    
    public void OpenCommandHandler(object sender, ExecutedRoutedEventArgs ev)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Open a chart file...",
            Filter = "VSB Files|*.vsb|All files|*.*",
            CheckFileExists = true,
            FileName = "OPENING.vsb"
        };

        if (!(dialog.ShowDialog(this) ?? false))
        {
            return;
        }

        try
        {
            _chart = VSFile.ReadSingleChart(dialog.FileName);
            // Timeline.Height = GetMaxChartHeight(_chart, 0.05f, 10);
            Timeline.InvalidateVisual();
            // MessageBox.Show(this, "Loaded!");
        }
        catch (Exception e)
        {
            MessageBox.Show(this, $"Failed to read chart!\n\n{e}");
        }
    }

    public void SaveAsCommandHandler(object sender, ExecutedRoutedEventArgs ev)
    {
        if (_chart == null) return;

        var dialog = new SaveFileDialog
        {
            Title = "Save chart...",
            Filter = "VSB Files|*.vsb|All files|*.*",
            AddExtension = true,
            FileName = "DIFFICULTY.vsb"
        };

        if (!(dialog.ShowDialog(this) ?? false))
        {
            return;
        }

        try
        {
            using var writer = new BinaryWriter(File.OpenWrite(dialog.FileName), Encoding.ASCII);
            BinaryChartWriter.WriteChart(_chart, writer);
            MessageBox.Show(this, "Saved!", "ChartEd", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
        catch (Exception e)
        {
            MessageBox.Show(this, $"Failed to save chart!\n\n{e}");
        }
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
                "Chart Editor",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning) 
            == MessageBoxResult.Yes)
        {
            Environment.Exit(0);   
        }
    }

    private void HandleTimelinePaint(object? sender, SKPaintSurfaceEventArgs args)
    {
        SKImageInfo info = args.Info;
        SKSurface surface = args.Surface;
        SKCanvas canvas = surface.Canvas;
        
        canvas.Clear(SKColors.Black);

        if (_chart == null)
        {
            var font = new SKFont(SKTypeface.FromFamilyName("Arial"));
            var paint = new SKPaint
            {
                Color = SKColors.White
            };

            const string str = "No chart loaded!";
            var textWidth = font.MeasureText(str, paint);
            font.Size = 0.9f * info.Width * font.Size / textWidth;
            canvas.DrawText(str, 0, info.Height / 2.0f, font, paint);
            return;
        }

        NoteCount.Text = _chart.Notes.Count.ToString();
            
        const int laneCount = 4;
        var laneWidth = (info.Width - 10) / laneCount;
        
        float GetLanePos(LaneType t)
        {
            return t switch
            {
                LaneType.Lane1 or LaneType.Lane2 or LaneType.Lane3 or LaneType.Lane4 => ((byte)t * laneWidth) + 5,
                LaneType.LeftBumper or LaneType.MiddleBumper or LaneType.RightBumper => (((byte)t - 4) * laneWidth) + 5,
                _ => -100
            };
        }

        const float noteHeight = 20.0f;

        var markerPaint = new SKPaint
        {
            Color = ShowMarkersBox.IsChecked.GetValueOrDefault() ? SKColors.White : SKColor.Empty,
            StrokeWidth = 10.0f,
        };
        
        // start marker
        var startY = 50 - _timelineScroll;
        canvas.DrawLine(0, startY, (float)Timeline.Width, startY, markerPaint);

        var leftChipPaint = new SKPaint
        {
            Color = SKColors.Aqua,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeCap = SKStrokeCap.Round
        };

        var rightChipPaint = new SKPaint
        {
            Color = SKColors.LightPink,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeCap = SKStrokeCap.Round
        };

        var errorPaint = new SKPaint
        {
            Color = SKColors.Green,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeCap = SKStrokeCap.Round
        };

        var leftBumperPaint = new SKPaint
        {
            Color = SKColors.Blue,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeCap = SKStrokeCap.Round
        };
        
        var rightBumperPaint = new SKPaint
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeCap = SKStrokeCap.Round
        };

        var middleBumperPaint = new SKPaint
        {
            Color = SKColors.MediumPurple,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeCap = SKStrokeCap.Round
        };

        var minePaint = new SKPaint
        {
            Color = SKColors.DarkGray,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeCap = SKStrokeCap.Round
        };
        
        var markerFont = new SKFont(SKTypeface.FromFamilyName("Arial"));
        markerFont.Size = (float)MarkerSizeSlider.Value;

        SKPaint GetPaintForLane(LaneType lane) => lane switch
        {
            LaneType.Lane1 or LaneType.Lane2 => leftChipPaint,
            LaneType.Lane3 or LaneType.Lane4 => rightChipPaint,
            LaneType.LeftBumper => leftBumperPaint,
            LaneType.MiddleBumper => middleBumperPaint,
            LaneType.RightBumper => rightBumperPaint,
            _ => errorPaint
        };
        
        foreach (var note in _chart.Notes)
        {
            var xPos = GetLanePos(note.Lane);
            var yPos = 50 + note.Time * _timelineScale;

            if ((yPos < _timelineScroll || yPos > _timelineScroll + Timeline.Height) && note.Type != NoteType.Hold)
            {
                continue;
            }

            switch (note.Type)
            {
                case NoteType.Chip:
                    canvas.DrawRect(xPos + 2.5f, yPos - _timelineScroll, laneWidth - 5, noteHeight, GetPaintForLane(note.Lane));
                    break;
                
                case NoteType.Mine:
                    canvas.DrawRect(xPos + 2.5f, yPos - _timelineScroll, laneWidth - 5, noteHeight, minePaint);
                    break;
                
                case NoteType.Bumper:
                    canvas.DrawRect(xPos + 2.5f, yPos - _timelineScroll, laneWidth * 2 - 5, noteHeight, GetPaintForLane(note.Lane));
                    break;
                
                case NoteType.Hold:
                    var holdTime = note.EndTime - note.Time;
                    canvas.DrawRect(xPos + 2.5f, yPos - _timelineScroll, laneWidth - 5, noteHeight + holdTime * _timelineScale, GetPaintForLane(note.Lane));
                    break;
                    
                case NoteType.TempoChange:
                    var newTempo = note.BPM;
                    var text = $"TEMPO TO: {newTempo}bpm";
                    
                    canvas.DrawLine(0, yPos - _timelineScroll - 20, 
                        (float)Timeline.Width, yPos - _timelineScroll - 20, markerPaint);
                    canvas.DrawText(text, 10, yPos - _timelineScroll, markerFont, markerPaint);
                    break;
                
                default:
                    Debug.WriteLine($"Error note: {note.Type} @ {note.Time}/{note.Lane}");
                    canvas.DrawRect(xPos + 2.5f, yPos - _timelineScroll, laneWidth - 5, noteHeight, errorPaint);
                    break;
            }
        }
    }

    private void HandleTimelineMouseWheel(object sender, MouseWheelEventArgs e)
    {
        ScrollBy(-e.Delta);
    }

    private void HandleScrollUpButtonClick(object sender, RoutedEventArgs e)
    {
        ScrollBy(-50.0f);
    }
    
    private void HandleScrollResetButtonClick(object sender, RoutedEventArgs e)
    {
        ScrollBy(-_timelineScroll);
    }
    
    private void HandleScrollDownButtonClick(object sender, RoutedEventArgs e)
    {
        ScrollBy(50.0f);
    }

    private void HandleScaleSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        _timelineScale = (float)e.NewValue;
        Timeline?.InvalidateVisual();
    }

    private void HandleMarkerSettingsChanged(object sender, RoutedEventArgs e)
    {
        Timeline?.InvalidateVisual();
    }

    private void ScrollBy(float value)
    {
        _timelineScroll += value;
        if (_timelineScroll < -20) _timelineScroll = -20;
        Timeline?.InvalidateVisual();
        if (PositionDisplay != null) PositionDisplay.Text = ((_timelineScroll * _timelineScale - 50) / 1000).ToString("F3");
    }

    private void HandleTimelineMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (_chart == null) return;

        var actualMouseX = (float)(e.GetPosition(Timeline).X);
        var actualMouseY = (float)(e.GetPosition(Timeline).Y + _timelineScroll);

        if (e.RightButton == MouseButtonState.Pressed)
        {
            _chart.Notes.RemoveAll(n => IsHoveredNote(n, actualMouseX, actualMouseY));
            Timeline?.InvalidateVisual();
            return;
        }
        else if (e.LeftButton == MouseButtonState.Pressed)
        {
            //todo: show note properties dialog
            // (change type, change extra fields)
            // if no note is hovered, add one and then show 

            NoteData actualNote;
            NoteData? existingNote = null;
            try
            {
                existingNote = _chart.Notes.First(n => IsHoveredNote(n, actualMouseX, actualMouseY));
                actualNote = existingNote.Value;
            }
            catch (InvalidOperationException)
            {
                var currentTime = (actualMouseY - 50) / _timelineScale;
                var laneWidth = (Timeline.CanvasSize.Width - 10) / 4;
                var currentLane = (byte)Math.Floor(actualMouseX / laneWidth);

                if (float.TryParse(SnappingInput.Text, out var snappingAmount) && snappingAmount > 0)
                {
                    snappingAmount *= 1000; // seconds to millis
                    currentTime = MathF.Round(currentTime / snappingAmount, MidpointRounding.AwayFromZero) * snappingAmount;

                    // prevent multiple notes snapping into the same spot
                    if (_chart.Notes.Any(n => n.Lane == (LaneType)currentLane && n.Time == currentTime))
                    {
                        return;
                    }
                }

                actualNote = new NoteData
                {
                    Time = currentTime,
                    Lane = (LaneType)currentLane,
                    Type = Enum.Parse<NoteType>(PlacingNoteInput.SelectedValue as string ?? "Chip")
                };

                if(actualNote.Type is NoteType.Bumper or NoteType.BumperMine or NoteType.BumperUnknown8)
                {
                    actualNote.Lane = (LaneType)((byte)actualNote.Lane + 4);
                    if((byte)actualNote.Lane > (byte)LaneType.RightBumper)
                    {
                        actualNote.Lane = LaneType.RightBumper;
                    }
                }

                if(actualNote.Type is NoteType.Hold)
                {
                    actualNote.EndTime = actualNote.Time + 250f;
                }
            }

            if(Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl) || existingNote.HasValue)
            {
                var dlg = new NotePropertiesDialog(actualNote)
                {
                    Owner = this
                };

                if (dlg.ShowDialog().GetValueOrDefault())
                {
                    if (existingNote.HasValue) _chart.Notes.Remove(existingNote.Value);
                    _chart.Notes.Add(dlg.Note);
                    Timeline.InvalidateVisual();
                }
            } 
            else
            {
                _chart.Notes.Add(actualNote);
                Timeline.InvalidateVisual();
            }
        }
    }

    private bool IsHoveredNote(NoteData note, float mouseX, float mouseY)
    {
        const float noteHeight = 20.0f;
        const int laneCount = 4;
        var laneWidth = (Timeline.CanvasSize.Width - 10) / laneCount;
        var noteY = 50 + note.Time * _timelineScale;

        switch (note.Type)
        {
            case NoteType.Chip:
            case NoteType.Mine:
                return mouseY >= noteY &&
                        mouseY < noteY + noteHeight &&
                        mouseX >= laneWidth * (byte)note.Lane &&
                        mouseX < laneWidth * ((byte)note.Lane + 1);

            case NoteType.Hold:
                var holdTime = note.EndTime - note.Time;
                return mouseY >= noteY &&
                        mouseY < noteY + holdTime &&
                        mouseX >= laneWidth * (byte)note.Lane &&
                        mouseX < laneWidth * ((byte)note.Lane + 1);

            case NoteType.Bumper:
            case NoteType.BumperMine:
            case NoteType.BumperUnknown8:
                return mouseY >= noteY &&
                        mouseY < noteY + noteHeight &&
                        mouseX >= laneWidth * ((byte)note.Lane - 4) &&
                        mouseX < laneWidth * ((byte)note.Lane - 2);

            default:
                return false;
        }
        
    }
}