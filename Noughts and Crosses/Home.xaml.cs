using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Noughts_and_Crosses;

/// <summary>
///     Interaction logic for Page1.xaml
/// </summary>
public partial class Page1 : Window
{
    public Page1()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Set();
        Width = 600;
        Height = 450;
    }
    /// <summary>
    /// Reset and update all values
    /// </summary>
    private void Set()
    {
        txtP1Score.Foreground = clrP1.Colour;
        txtP2Score.Foreground = clrP2.Colour;
        symbP1.Foreground = clrP1.Colour;
        symbP2.Foreground = clrP2.Colour;
        symbP1.SelectedIndex = 0;
        symbP2.SelectedIndex = 0;
        player.Open(musics[Music]);
        player.Play();
        width = 7;
        height = 6;
        win = 4;
        time = 5;
        p1Score = 0;
        txtP1Score.Text = txtP2Score.Text = "Score: 0";
        p2Score = 0;
        txtP1Name.Text = "Player 1";
        txtP2Name.Text = "Player 2";
        clrP1.sldB.Value = clrP2.sldG.Value = clrP2.sldB.Value = 0;
        clrP1.sldR.Value = clrP2.sldR.Value = clrP1.sldG.Value = 255;
        clrP1_MouseMove(null!, null!);
        clrP2_MouseMove(null!, null!);
        tckComp.IsChecked = false;
    }
    /// <summary>
    /// If sliders changed, change all P1 colours
    /// </summary>
    private void clrP1_MouseMove(object sender, MouseEventArgs e)
    {
        txtP1Score.Foreground  = txtP1Name.Foreground
            = symbP1.Foreground =
                clrP1.Colour;

        if (p1token == 0)
        {
            P1Display.Fill = clrP1.Colour;
        }
    }
    // <summary>
    /// If sliders changed, change all P2 colours
    /// </summary>
    private void clrP2_MouseMove(object sender, MouseEventArgs e)
    {
        txtP2Score.Foreground  = txtP2Name.Foreground
            = symbP2.Foreground = tckComp.Foreground =
                clrP2.Colour;
        if (p2token == 0)
        {
            P2Display.Fill = clrP2.Colour;
        }
    }

    private void btnStart_Click(object sender, RoutedEventArgs e)
    {
        Ellipse p1 = new(), p2 = new();
        p1.Fill = p1token == 0 ? clrP1.Colour : tokens[p1token];
        p2.Fill = p2token == 0 ? clrP2.Colour : tokens[p2token];


        if (p1token == p2token && p1token!=0) //Ensure symbols aren't same, throw error
        {
            MessageBox.Show("Player symbols cannot be the same", "Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Visibility = Visibility.Collapsed;
        var isComp = tckComp.IsChecked != null && (bool)tckComp.IsChecked; //Bypass nullable warning
        if (isComp && (height > 3 || width > 3)) //Warn for slow AI
            MessageBox.Show("Warning: computer player may be slow on larger grids", "Warning",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        var frm1 = new MainWindow(height, width, win, //Open new window
            p1 , p2, clrP1.Colour, clrP2.Colour,
            txtP1Name.Text, txtP2Name.Text, isComp, time,
            modes[mode], Background, musics[music]);
        frm1.Home = this;
        frm1.Show();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        try //scale button text
        {
            btnStart.FontSize = Window.ActualWidth * 0.05;
            symbP1.FontSize = symbP2.FontSize = Window.ActualWidth * 0.02;
        }
        catch //Suppress errors if unitialised
        {
        }
    }
    /// <summary>
    /// Open config form
    /// </summary>
    private void btnSettings_Click(object sender, RoutedEventArgs e)
    {
        Hide();
        var config = new Config();
        config.Home = this;
        config.Show();
    }

    private void btnReset_Click(object sender, RoutedEventArgs e)
    {
        Set();
    }

    #region Fields

    private readonly MediaPlayer player = new();

    private int p1Score;
    private int p2Score;

    public int width = 3;
    public int height = 3;
    public int win = 3;
    public int time = 5;

    private int music;

    public int Music
    {
        get => music;
        set
        {
            if (value is < 3 and > -1)
            {
                music = value;
                player.Open(musics[value]);
                player.Play();
            }
            else
            {
                player.Stop();
            }
        }
    }

    private readonly List<Uri> musics = new()
    {
        new Uri(
            @"pack://application:,,,/Resources/orchesta theme.wav"),
        new Uri(
            @"pack://application:,,,/Resources/8bit theme.wav"),
        new Uri(
            @"pack://application:,,,/Resources/organ theme.wav")
    };

    private int back;

    public int Back
    {
        get => back;
        set
        {
            if (value is < 4 and > -1)
            {
                back = value;
                Background = backs[value];
            }
            else
            {
                Background = new SolidColorBrush(Colors.DarkSlateGray);
            }
        }
    }

    private readonly List<Brush> backs = new()
    {
        new SolidColorBrush(Colors.DarkSlateGray),
        new ImageBrush(new BitmapImage(new Uri(
            @"pack://application:,,,/Resources/Mandelbrot.png"))),
        new ImageBrush(new BitmapImage(new Uri(
            @"pack://application:,,,/Resources/Snowflake.png"))),
        new ImageBrush(new BitmapImage(new Uri(
            @"pack://application:,,,/Resources/Square.png")))
    };

    private int mode;

    public int Mode
    {
        get => mode;
        set
        {
            if (value is < 4 and > -1)
                mode = value;
            else
                mode = 0;
        }
    }

    private readonly List<string> modes = new()
    {
        "Classic",
        "Random",
        "Mystery",
        "Two Turn"
    };

    public int P1Score
    {
        get => p1Score;
        set
        {
            txtP1Score.Text = $"Score: {value}";
            p1Score = value;
        }
    }

    public int P2Score
    {
        get => p2Score;
        set
        {
            txtP2Score.Text = $"Score: {value}";
            p2Score = value;
        }
    }


    private int p1token;

    public int P1Token
    {
        get => p1token;
        set
        {
            p2token = value;

        }
    }

    private int p2token;

    public int P2Token
    {
        get => p2token;
        set
        {
            p2token = value;

        }
    }

    private readonly List<ImageBrush> tokens = new()
    {
        new ImageBrush(),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Blundell.jpg"))),
        new ImageBrush( new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Fatyga.jpg"))),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/George.jpg"))),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Graham.jpg"))),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Jenkins.jpg"))),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/O'Brien.jpg"))),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Oxenham.jpg"))),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Piper.jpg"))),
        new ImageBrush( new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Read.jpg"))),
        new ImageBrush( new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Reid.jpg"))),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Saha.jpg"))),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Sheridan.jpg"))),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Smith.jpg"))),
        new ImageBrush(new BitmapImage(
            new Uri( @"pack://application:,,,/Resources/Faces/Yu.jpg"))),

    };


    #endregion

    private void symbP1_DropDownClosed(object sender, EventArgs e)
    {
        p1token = symbP1.SelectedIndex;
        if (p1token != 0)
        {
            P1Display.Fill = tokens[p1token];
        }
    }

    private void symbP2_DropDownClosed(object sender, EventArgs e)
    {
        p2token = symbP2.SelectedIndex;
        if (p2token != 0)
        {
            P2Display.Fill = tokens[p2token];
        }
    }
}