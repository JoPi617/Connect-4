using System.ComponentModel;
using System.Windows;
using static System.Convert;

namespace Noughts_and_Crosses;

/// <summary>
///     Interaction logic for Config.xaml
/// </summary>
public partial class Config : Window
{
    public Page1 Home = null!;

    public Config()
    {
        InitializeComponent();
    }
    /// <summary>
    /// Close if win value is valid
    /// </summary>
    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
        if (sldWin.Value > sldWidth.Value && sldWin.Value > sldHeight.Value)
        {
            MessageBox.Show("Win size cannot be greater than width and height", "Error", MessageBoxButton.OK);
            return;
        }

        Close();
    }
    /// <summary>
    /// Set home parameters to current
    /// </summary>
    private void Window_Closing(object sender, CancelEventArgs e)
    {
        Home.width = ToInt32(sldWidth.Value);
        Home.height = ToInt32(sldHeight.Value);
        Home.win = ToInt32(sldWin.Value);
        Home.time = ToInt32(sldTime.Value);
        Home.Mode = drpModes.SelectedIndex;
        Home.Music = drpMusic.SelectedIndex;
        Home.Back = drpBack.SelectedIndex;
        Home.Visibility = Visibility.Visible;
        Home.diff = ToInt32(sldDiff.Value);
    }
    /// <summary>
    /// Set current things to home
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        sldWidth.Value = Home.width;
        sldHeight.Value = Home.height;
        sldWin.Value = Home.win;
        sldTime.Value = Home.time;
        drpModes.SelectedIndex = Home.Mode;
        drpMusic.SelectedIndex = Home.Music;
        drpBack.SelectedIndex = Home.Back;
        Background = Home.Background;
        sldDiff.Value = Home.diff;
    }
}