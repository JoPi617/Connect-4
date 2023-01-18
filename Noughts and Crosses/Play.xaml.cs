using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Noughts_and_Crosses;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly int[,] _board;
    private readonly DispatcherTimer _dispatcherTimer = new();
    private readonly int _firstTime;
    private readonly int _height;
    private readonly bool _isComp;
    private readonly Brush _p1Color;
    private readonly string _p1Name;
    private readonly Ellipse _p1Sym;
    private readonly Brush _p2Color;
    private readonly string _p2Name;
    private readonly Ellipse _p2Sym;
    private readonly int _width;
    private readonly int _win;
    private int _currentTime;
    private Ellipse _falling = new();
    private bool _isFalling;
    private bool _isP1Turn;
    private int _turns;
    private string _winner;
    public Page1 Home = null!;
    public string Mode;


    /// <summary>
    ///     Inititalise all values, and start music and tiner
    /// </summary>
    public MainWindow(int height, int width, int win, Ellipse newP1, Ellipse newP2, Brush p1Color, Brush p2Color,
        string p1Name, string p2Name, bool isComp, int time, string mode, Brush back, Uri music)
    {
        InitializeComponent();

        BoardSet(height, width);
        _isP1Turn = true;
        _board = new int[height, width];
        _height = height;
        _width = width;
        _p1Sym = newP1;
        _p2Sym = newP2;
        _p1Color = p1Color;
        _p2Color = p2Color;
        _p1Name = p1Name;
        _p2Name = p2Name;
        _win = win;
        _winner = "";
        _isComp = isComp;
        _currentTime = time * 100;
        _firstTime = time * 100;
        Mode = mode;


        Background = back;
        MediaPlayer player = new();
        player.Open(music);
        player.Play();


        _dispatcherTimer.Tick += dispatcherTimer_Tick;
        _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        _dispatcherTimer.Start();

        Grid.SetRow(_p1Sym, 1);
        Grid.SetColumn(_p1Sym, 2);
        Grid.Children.Add(_p1Sym);
        Grid.SetRow(_p2Sym, 1);
        Grid.SetColumn(_p2Sym, 2);
        Grid.Children.Add(_p2Sym);
        _p2Sym.Visibility = Visibility.Hidden;
    }

    /// <summary>
    ///     Check for CPU turn,decrement time, then check running out of time,
    /// </summary>
    private void dispatcherTimer_Tick(object? sender, EventArgs e)
    {
        if (_isComp && !_isP1Turn) CompTurn();
        if (ScoreCheck(_board)[0] != 0 || _isFalling) return;
        _currentTime--;
        if (_currentTime == -1)
        {
            if (_isP1Turn)
            {
                MessageBox.Show($"{_p1Name} ran out of time, {_p2Name} is the winner!",
                    "Winner!", MessageBoxButton.OK);
                Home.P2Score++;
            }
            else
            {
                MessageBox.Show($"{_p2Name} ran out of time, {_p1Name} is the winner!",
                    "Winner!", MessageBoxButton.OK);
                Home.P1Score++;
            }

            Close();
            Home.Visibility = Visibility.Visible;
        }

        txtTime.Text = _currentTime % 100 < 10
            ? $"{_currentTime / 100}.{"0" + _currentTime % 100}"
            : $"{_currentTime / 100}.{_currentTime % 100}";
    }

    /// <summary>
    ///     Initialise board given height and width with buttons
    /// </summary>
    /// <param name="height"></param>
    /// <param name="width"></param>
    private void BoardSet(int height, int width)
    {
        for (var i = 0; i < height; i++) brdMain.BoardGrid.RowDefinitions.Add(new RowDefinition());

        for (var i = 0; i < width; i++) brdMain.BoardGrid.ColumnDefinitions.Add(new ColumnDefinition());

        for (var row = 0; row < height; row++)
        for (var column = 0; column < width; column++)
        {
            var button = new Button
            {
                Name = "btn_" + row + "_" + column,
                Background = new ImageBrush(new BitmapImage(new Uri(
                    @"pack://application:,,,/Resources/Grid.png"))),
                BorderBrush = new SolidColorBrush(Colors.Transparent),
                BorderThickness = new Thickness(0, 0, 0, 0)
            };
            button.Click += Btn_Click;

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            brdMain.BoardGrid.Children.Add(button);
        }
    }

    private void Turn(Button btn)
    {
        var column = Grid.GetColumn(btn);


        PlaceCounter(column);


        var result = ScoreCheck(_board); // Check for win and draw
        if (result[0] != 0) Win(result);
        else if (FillCheck(_board)) Draw();

        _currentTime = _firstTime; //Reset timer

        switch (Mode)
        {
            case "Mystery":
            case "Classic": //Change turn
                _isP1Turn = !_isP1Turn;
                break;
            case "Random":
                var rand = new Random(); //Change on coin toss
                if (rand.Next(0, 2) == 1) _isP1Turn = !_isP1Turn;
                break;
            case "Two Turn":
                if (_turns == 1) //If turn taken, invert
                {
                    _turns = 0;
                    _isP1Turn = !_isP1Turn;
                }
                else
                {
                    _turns++;
                }

                break;
        }


        if (!_isP1Turn)
        {
            _p2Sym.Visibility = Visibility.Visible;
            _p1Sym.Visibility = Visibility.Hidden;
        }
        else
        {
            _p1Sym.Visibility = Visibility.Visible;
            _p2Sym.Visibility = Visibility.Hidden;
        }
    }

    private int FindEmpty(int column)
    {
        for (var row = _height - 1; row >= 0; row--)
            if (_board[row, column] == 0)
                return row;

        return -1;
    }

    private void PlaceCounter(int column)
    {
        var cellWidth = brdMain.ActualWidth / Convert.ToDouble(brdMain.BoardGrid.ColumnDefinitions.Count);
        var cellHeight = brdMain.ActualHeight / Convert.ToDouble(brdMain.BoardGrid.RowDefinitions.Count);


        var row = FindEmpty(column);
        if (row == -1) return;

        _falling = new Ellipse
        {
            Fill = _isP1Turn ? _p1Sym.Fill : _p2Sym.Fill,
            Height = cellHeight,
            Width = cellWidth
        };


        Grid.SetColumn(_falling, column);
        Grid.SetRow(_falling, row);
        brdMain.BoardGrid.Children.Insert(0, _falling);

        

        var fallingMargin = _falling.Margin;
        fallingMargin.Top = -row * cellHeight;
        fallingMargin.Bottom = -fallingMargin.Top;
        _falling.Margin = fallingMargin;

        var timer = new DispatcherTimer();
        timer.Interval = new TimeSpan(0, 0, 0, 0, 50);
        timer.Tick += AnimationTick;
        timer.Start();
        _isFalling = true;

        _board[row, column] = _isP1Turn ? -1 : 1;
    }

    private void AnimationTick(object? sender, EventArgs e)
    {
        var tick = 20;
        if (_falling.Margin.Top >= 0)
        {
            _isFalling = false;
            _falling.Margin = new Thickness(0, 0, 0, 0);
            return;
        }

        var fallingMargin = _falling.Margin;
        fallingMargin.Top += tick;
        fallingMargin.Bottom -= tick;
        _falling.Margin = fallingMargin;
    }

    /// <summary>
    ///     Check if every element in board is full
    /// </summary>
    private bool FillCheck(int[,] board)
    {
        for (var i = 0; i < _width; i++)
        for (var j = 0; j < _height; j++)
            if (board[j, i] == 0)
                return false;
        return true;
    }

    /// <summary>
    ///     Show draw screen and return
    /// </summary>
    private void Draw()
    {
        MessageBox.Show("It's a draw!", "Draw!", MessageBoxButton.OK);
        Hide();
        Home.Visibility = Visibility.Visible;
    }

    /// <summary>
    ///     Get line, show win screen, return
    /// </summary>
    /// <param name="result"></param>
    private void Win(int[] result)
    {
        Line(result);
        _winner = result[0].ToString();

        if (_winner == "-1")
        {
            MessageBox.Show($"{_p1Name} is the winner!", "Winner!", MessageBoxButton.OK);
            Home.P1Score++;
        }
        else
        {
            MessageBox.Show($"{_p2Name} is the winner!", "Winner!", MessageBoxButton.OK);
            Home.P2Score++;
        }

        Close();
        Home.Visibility = Visibility.Visible;
    }

    /// <summary>
    ///     Draw line with two grid references and colour
    /// </summary>
    /// <param name="input"></param>
    private void Line(int[] input)
    {
        var cellWidth = brdMain.ActualWidth / Convert.ToDouble(brdMain.BoardGrid.ColumnDefinitions.Count);
        var cellHeight = brdMain.ActualHeight / Convert.ToDouble(brdMain.BoardGrid.RowDefinitions.Count);

        var line = new Line
        {
            Stroke = input[0] == -1 ? _p1Color : _p2Color,
            X1 = (input[1] + 0.5) * cellWidth,
            X2 = (input[3] + 0.5) * cellWidth,
            Y1 = (input[2] + 0.5) * cellHeight,
            Y2 = (input[4] + 0.5) * cellHeight,
            StrokeThickness = 10
        };
        Grid.SetColumnSpan(line, _width);
        Grid.SetRowSpan(line, _height);
        Grid.Children.Add(line);
    }

    /// <summary>
    ///     Check winner in every sub-grid
    /// </summary>
    private int[] ScoreCheck(int[,] board)
    {
        for (var row = 0; row < _height - _win + 1; row++)
        for (var column = 0; column < _width - _win + 1; column++)
        {
            var result = SubCheck(board, column, row, _win);
            if (result[0] != 0) return result;
        }

        return new[] { 0 };
    }

    /// <summary>
    ///     For sub-grid, check every row, column and diagonal
    /// </summary>
    private int[] SubCheck(int[,] board, int x, int y, int size)
    {
        var sum = 0;
        for (var row = 0; row < size; row++) // check rows
        {
            for (var column = 0; column < size; column++) sum += board[y + row, x + column];

            if (sum == size) return new[] { 1, x, y + row, x + size - 1, y + row };
            if (sum == size * -1) return new[] { -1, x, y + row, x + size - 1, y + row };
            sum = 0;
        }

        sum = 0;
        for (var column = 0; column < size; column++) // check columns
        {
            for (var row = 0; row < size; row++) sum += board[y + row, x + column];

            if (sum == size) return new[] { 1, x + column, y, x + column, y + size - 1 };
            if (sum == -1 * size) return new[] { -1, x + column, y, x + column, y + size - 1 };
            sum = 0;
        }

        sum = 0;
        for (var i = 0; i < size; i++) sum += board[y + i, x + i];

        if (sum == size) return new[] { 1, x, y, x + size - 1, y + size - 1 };

        if (sum == -1 * size) return new[] { -1, x, y, x + size - 1, y + size - 1 };

        sum = 0;
        var rev = size - 1;
        for (var i = 0; i < size; i++)
        {
            sum += board[y + i, x + rev];
            rev--;
        }

        if (sum == size) return new[] { 1, x, y + size - 1, x + size - 1, y };

        if (sum == -1 * size) return new[] { -1, x, y + size - 1, x + size - 1, y };

        return new[] { 0 };
    }

    /// <summary>
    ///     Call click method with given button, if is empty
    /// </summary>
    private void Btn_Click(object sender, RoutedEventArgs e)
    {
        if (_isFalling) return;
        if (sender is Button btn && FindEmpty(Grid.GetColumn(btn)) != -1) Turn(btn);
    }

    /// <summary>
    ///     Call click method for best button
    /// </summary>
    private void CompTurn()
    {
        if (_isFalling) return;
        var best = FindBest(_board);
        foreach (var obj in brdMain.BoardGrid.Children)
            if (obj is Button btn && Grid.GetColumn(btn) == best)
                Turn(btn);
    }

    /// <summary>
    ///     Scale font size
    /// </summary>
    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        foreach (var child in brdMain.BoardGrid.Children)
            switch (child)
            {
                case Button btn:
                    try
                    {
                        if (btn.ActualHeight > btn.ActualWidth)
                            btn.FontSize = btn.ActualWidth * 0.7;
                        else
                            btn.FontSize = btn.ActualHeight * 0.7;
                    }
                    catch
                    {
                    }

                    break;
                case Ellipse ellipse:
                {
                    var cellWidth = brdMain.ActualWidth / Convert.ToDouble(brdMain.BoardGrid.ColumnDefinitions.Count);
                    var cellHeight = brdMain.ActualHeight / Convert.ToDouble(brdMain.BoardGrid.RowDefinitions.Count);

                    ellipse.Height = cellHeight;
                    ellipse.Width = cellWidth;
                    break;
                }
            }
    }

    /// <summary>
    ///     Stop timer and show home
    /// </summary>
    private void Window_Closing(object sender, CancelEventArgs e)
    {
        _dispatcherTimer.Stop();
        Home.Visibility = Visibility.Visible;
    }

    #region Minimax

    private int Minimax(int[,] board, int depth, bool isMax, int alpha, int beta)
    {
        var result = ScoreCheck(board);
        if (result[0] != 0) return result[0] * 11 + result[0] > 0 ? depth : -depth;
        if (FillCheck(board) || depth > 7) return 0;

        if (isMax)
        {
            var bestVal = int.MinValue;
            for (var column = 0; column < _width; column++)
            {
                var row = FindEmpty(column);
                    board[row, column] = 1;
                    var value = Minimax(board, depth + 1, !isMax, alpha, beta);
                    board[row, column] = 0;
                    bestVal = Math.Max(bestVal, value);
                    alpha = Math.Max(alpha, bestVal);
                    if (beta <= alpha) break;

                    if (beta <= alpha) break;
            }

            return bestVal;
        }
        else
        {
            var bestVal = int.MaxValue;
            for (var column = 0; column < _width; column++)
            {
                if (FindEmpty(column) != -1)
                {
                    var row = FindEmpty(column);
                    board[row, column] = -1;
                    var value = Minimax(board, depth + 1, !isMax, alpha, beta);
                    board[row, column] = 0;
                    bestVal = Math.Min(bestVal, value);
                    alpha = Math.Min(alpha, bestVal);
                    if (beta <= alpha) break;
                }

                if (beta <= alpha) break;
            }

            return bestVal;
        }
    }

    private int FindBest(int[,] board)
    {
        var bestVal = int.MinValue;
        var best = 0;

        for (var row = 0; row < _height; row++)
        for (var column = 0; column < _width; column++)
            if (board[row, column] == 0)
            {
                board[row, column] = 1;
                var moveVal = Minimax(board, 0, false, int.MinValue, int.MaxValue);
                board[row, column] = 0;
                if (moveVal > bestVal)
                {
                    best = column;
                    bestVal = moveVal;
                }
            }


        return best;
    }

    #endregion
}