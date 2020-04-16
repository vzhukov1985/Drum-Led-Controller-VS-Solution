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
using Drum_Led_Patch_Editor.ViewModels;

namespace Drum_Led_Patch_Editor.Views
{
    /// <summary>
    /// Interaction logic for GlediatorImportDialog.xaml
    /// </summary>
    public partial class GlediatorImportDialog : Window
    {
        public GlediatorImportDialog()
        {
            InitializeComponent();
            DataContext = new GlediatorImportViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
