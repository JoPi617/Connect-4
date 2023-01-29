using System;
using System.ComponentModel;
using System.Threading.Tasks;
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
    private int _diff;


    /// <summary>
    ///     Inititalise all values, and start music and tiner
    /// </summary>
    public MainWindow(int height, int width, int win, Ellipse newP1, Ellipse newP2, Brush p1Color, Brush p2Color,
        string p1Name, string p2Name, bool isComp, int time, string mode, Brush back, Uri music, int diff)
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
        _diff = diff;
    }

    /// <summary>
    ///     Check for CPU turn,decrement time, then check running out of time,
    /// </summary>
    private void dispatcherTimer_Tick(object? sender, EventArgs e)
    {
        if (_isComp && !_isP1Turn && !_isFalling)
            CompTurn();
        if (ScoreCheck(_board,_win,_height,_width)[0] != 0 || _isFalling) return;
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


        var result = ScoreCheck(_board, _win,_height,_width); // Check for win and draw
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
        var row = FindEmpty(column);
        if (row == -1) return;
        var cellWidth = brdMain.ActualWidth / Convert.ToDouble(brdMain.BoardGrid.ColumnDefinitions.Count);
        var cellHeight = brdMain.ActualHeight / Convert.ToDouble(brdMain.BoardGrid.RowDefinitions.Count);


        _falling = new Ellipse
        {
            Fill = Mode != "Mystery" ? _isP1Turn ? _p1Sym.Fill : _p2Sym.Fill : Brushes.BurlyWood,
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
        var view = new Viewbox();
        var board = brdMain;
        if (_isFalling) return;

        if (_diff > 6)
        {
            var txt = new TextBlock()
            {
                Text = "CPU is\r\n thinking",
                Foreground = Brushes.BurlyWood,
                Background = Brushes.Transparent,
            };
            Grid.SetColumn(view,0);
            Grid.SetRow(view,0);
            view.Child = txt;
            Grid.Children.Add(view);

            Grid.Children.Remove(board);
        }

        var best = FindBest(_board);
        foreach (var obj in brdMain.BoardGrid.Children)
            if (obj is Button btn && Grid.GetColumn(btn) == best)
            {
                Turn(btn);
                break;
            }

        try
        {
            Grid.Children.Remove(view);
            Grid.Children.Add(board);
        }
        catch { }
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

    static double Minimax(int[,] board, int depth, bool isMax, double alpha, double beta, int win, int height, int width, int diff)
    {
        var result = ScoreCheck(board, win, height, width);
        if (result[0] != 0) return result[0] * 11 + (result[0] > 0 ? depth : -depth);
        if (FillCheck(board, width, height)) return 0;
        if (depth == diff)
        {
            var score = ScoreAnalyse(board, height, width, win);
            return score[0] - score[1];
        }


        if (isMax)
        {
            var bestVal = double.MinValue;
            for (int column = 0; column < width; column++)
            {
                if (FindEmpty(column, height, board) != -1)
                {
                    var row = FindEmpty(column, height, board);
                    var temp = board.Clone() as int[,];
                    temp![row, column] = 1;
                    var value = Minimax(temp, depth + 1, !isMax, alpha, beta, win, height, width,diff);
                    bestVal = Math.Max(bestVal, value);
                    alpha = Math.Max(alpha, bestVal);
                    if (beta <= alpha) break;
                }
                if (beta <= alpha) break;
            }

            return bestVal;
        }
        else
        {
            var bestVal = double.MaxValue;
            for (int column = 0; column < width; column++)
            {
                if (FindEmpty(column, height, board) != -1)
                {
                    var row = FindEmpty(column, height, board);
                    var temp = board.Clone() as int[,];
                    temp![row, column] = -1;
                    var value = Minimax(temp, depth + 1, !isMax, alpha, beta, win, height, width,diff);
                    bestVal = Math.Min(bestVal, value);
                    alpha = Math.Min(alpha, bestVal);
                    if (beta <= alpha) break;
                }
                if (beta <= alpha) break;
            }
            return bestVal;
        }
    }

    int FindBest(int[,] board)
    {
        double bestVal = int.MinValue;
        int best = 0;

        Parallel.For((long)0, _width, column =>
        {
            var row = FindEmpty(column, _height, _board);
            if (row>0&&board[row, column] == 0)
            {
                var temp = board.Clone() as int[,];
                temp![row, column] = 1;
                var moveVal = Minimax(temp, 0, false,
                    double.MinValue, double.MaxValue,
                    _win, _height, _width, _diff);
                if (moveVal > bestVal)
                {
                    best = (int)column;
                    bestVal = moveVal;
                }
            }
        });

        return best;
    }

    #endregion

    #region Checks

    static int[] ScoreCheck(int[,] board, int win, int height, int width)
    {
        for (var row = 0; row < height - win + 1; row++)
            for (var column = 0; column < width - win + 1; column++)
            {
                var result = SubCheck(board, column, row, win);
                if (result[0] != 0) return result;
            }

        return new[] { 0 };
    }

    static int[] SubCheck(int[,] board, int x, int y, int win)
    {
        var sum = 0;
        for (var row = 0; row < win; row++) // check rows
        {
            for (var column = 0; column < win; column++) sum += board[y + row, x + column];

            if (sum == win) return new[] { 1, x, y + row, x + win - 1, y + row };
            if (sum == win * -1) return new[] { -1, x, y + row, x + win - 1, y + row };
            sum = 0;
        }

        sum = 0;
        for (var column = 0; column < win; column++) // check columns
        {
            for (var row = 0; row < win; row++) sum += board[y + row, x + column];

            if (sum == win) return new[] { 1, x + column, y, x + column, y + win - 1 };
            if (sum == -1 * win) return new[] { -1, x + column, y, x + column, y + win - 1 };
            sum = 0;
        }

        sum = 0;
        for (var i = 0; i < win; i++) sum += board[y + i, x + i];

        if (sum == win) return new[] { 1, x, y, x + win - 1, y + win - 1 };

        if (sum == -1 * win) return new[] { -1, x, y, x + win - 1, y + win - 1 };

        sum = 0;
        var rev = win - 1;
        for (var i = 0; i < win; i++)
        {
            sum += board[y + i, x + rev];
            rev--;
        }

        if (sum == win) return new[] { 1, x, y + win - 1, x + win - 1, y };

        if (sum == -1 * win) return new[] { -1, x, y + win - 1, x + win - 1, y };

        return new[] { 0 };
    }

    static bool FillCheck(int[,] board, int width, int height)
    {
        for (var i = 0; i < width; i++)
            for (var j = 0; j < height; j++)
                if (board[j, i] == 0)
                    return false;
        return true;
    }

    #endregion

    static int FindEmpty(long column, int height, int[,] board)
    {
        for (int row = height - 1; row >= 0; row--)
        {
            if (board[row, column] == 0)
            {
                return row;
            }
        }

        return -1;
    }


    static double[] ScoreAnalyse(int[,] board, int height, int width, int win)
    {
        double score1 = 0;
        double score2 = 0;
        for (var row = 0; row < height - win + 1; row++)
            for (var column = 0; column < width - win + 1; column++)
            {
                var result = SubAnalyse(board, column, row, 1, win);
                score1 += result[0] / 100.0;
                score2 += result[1] / 100.0;
            }

        for (var row = 0; row < height - win + 1; row++)
            for (var column = 0; column < width - win + 1; column++)
            {
                var result = SubAnalyse(board, column, row, 2, win);
                score1 += result[0] / 10000.0;
                score2 += result[1] / 10000.0;
            }

        return new[] { score1, score2 };
    }

    static int[] SubAnalyse(int[,] board, int x, int y, int minus, int win)
    {
        var need = win - minus;
        var score1 = 0;
        var score2 = 0;
        var sum = 0;
        for (var row = 0; row < win; row++) // check rows
        {
            for (var column = 0; column < win; column++) sum += board[y + row, x + column];

            if (sum == need) score2++;
            if (sum == need * -1) score1++;
            sum = 0;
        }

        sum = 0;
        for (var column = 0; column < win; column++) // check columns
        {
            for (var row = 0; row < win; row++) sum += board[y + row, x + column];

            if (sum == need) score2++;
            if (sum == -1 * need) score1++;
            sum = 0;
        }

        sum = 0;
        for (var i = 0; i < win; i++) sum += board[y + i, x + i];

        if (sum == need) score2++;
        if (sum == -1 * need) score1++;

        sum = 0;
        var rev = win - 1;
        for (var i = 0; i < win; i++)
        {
            sum += board[y + i, x + rev];
            rev--;
        }

        if (sum == need) score2++;
        if (sum == -1 * need) score1++;

        return new[] { score1, score2 };
    }
}