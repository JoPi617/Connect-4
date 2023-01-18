using System;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Noughts_and_Crosses;

/// <summary>
///     Interaction logic for Custom.xaml
/// </summary>
public partial class Custom : Window
{
    public Custom()
    {
        InitializeComponent();
    }


    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
        try //add character
        {
            var cha = char.Parse(txtEntry.Text);
            Symbols.Source.Add(cha + "");
            Close();
        }
        catch
        {
            try //convert hex to character
            {
                var bytes = Convert.FromHexString(txtEntry.Text); 
                var chars = Encoding.BigEndianUnicode.GetChars(bytes);
                string str = new(chars);

                Symbols.Source.Add(str);
                Close();
            }
            catch
            {
                MessageBox.Show("Unrecognised Unicode", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    /// <summary>
    /// Clear default on entry
    /// </summary>
    private void txtEntry_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        txtEntry.Text = "";
    }

    /// <summary>
    /// Return if enter pressed
    /// </summary>
    private void Grid_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) OK_Click(new object(), new RoutedEventArgs());
    }
}