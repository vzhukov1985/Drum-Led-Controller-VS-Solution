﻿using System;
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
using Drum_Led_Patch_Editor.Services;

namespace Drum_Led_Patch_Editor.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(new WindowsDialogService(this), new WindowsFileService());
        }

        public void CloseApplication()
        {
            Close();
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            CloseApplication();
        }
    }
}
