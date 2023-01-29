using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Noughts_and_Crosses
{
    /// <summary>
    /// Interaction logic for Symbols.xaml
    /// </summary>
    public class Symbols
    {
        public static ObservableCollection<string> Source { get; set; } = new(new List<string>()
            {
                "Disc",
                "Beale",
                "Blundell",
                "Cole",
                "Crilly",
                "Fatyga",
                "George",
                "Graham",
                "Jenkins",
                "O'Brien",
                "Oxenham",
                "Piper",
                "Read",
                "Reid",
                "Saha",
                "Sheridan",
                "Smith",
                "Yu",
            });

        public static IEnumerable<string> Modes { get; set; } = new List<string>()
        {
            "Classic",
            "Random",
            "Mystery",
            "Two Turn"
        };
    }
}
