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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using Drum_Led_Patch_Editor.ViewModels;
using Drum_Led_Patch_Editor.Services;

namespace Drum_Led_Patch_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        public SettingsDialog()
        {
            InitializeComponent();
            DataContext = new SettingsDialogViewModel(new WindowsDialogService(this), new WindowsFileService());
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
