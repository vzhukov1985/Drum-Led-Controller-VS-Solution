using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Drum_Led_Controller_Settings.Views
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PresetAvailableDialog : Window
    {
        public PresetAvailableDialog()
        {
            InitializeComponent();
        }
        public void OK_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
