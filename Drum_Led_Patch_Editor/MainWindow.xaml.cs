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

namespace Drum_Led_Patch_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.InitialDirectory = "C:\\Working Projects\\Drum_Led_Controller\\Test";
            if (dlgOpen.ShowDialog() == true)
            {
                BinaryReader jinxFileReader = new BinaryReader(File.OpenRead(dlgOpen.FileName));
                BinaryWriter dlpFileWriter = new BinaryWriter(File.Create(System.IO.Path.GetDirectoryName(dlgOpen.FileName)+"\\Test.dlp"));

                //Kick
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)1);
                dlpFileWriter.Write((byte)2);
                dlpFileWriter.Write((byte)1);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)10);
                dlpFileWriter.Write((byte)18);

                //Snare
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)255);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)5);
                dlpFileWriter.Write((byte)15);

                //HiTom
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)255);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)4);
                dlpFileWriter.Write((byte)14);

                //MidTom
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)255);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)255);
                dlpFileWriter.Write((byte)3);
                dlpFileWriter.Write((byte)7);

                //LowTom
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)255);
                dlpFileWriter.Write((byte)255);
                dlpFileWriter.Write((byte)255);
                dlpFileWriter.Write((byte)2);
                dlpFileWriter.Write((byte)8);

                //Frame Length
                dlpFileWriter.Write((byte)18);
                //Frame Count
                dlpFileWriter.Write((UInt16)130);
                //FPS
                dlpFileWriter.Write((byte)30);
                //Triggered Leds Brightness
                dlpFileWriter.Write((byte)255);
                //Programmed Leds Brightness
                dlpFileWriter.Write((byte)90);

                //Frames Data
                for (int i = 0; i < 130; i++)
                {
                    byte[] frame = new byte[18 * 3];
                    frame = jinxFileReader.ReadBytes(18 * 3);
                    dlpFileWriter.Write(frame, 0, 18 * 3);

                    //Skip 3 lines
                    frame = jinxFileReader.ReadBytes(18 * 3);
                    frame = jinxFileReader.ReadBytes(18 * 3);
                    frame = jinxFileReader.ReadBytes(18 * 3);
                }
            }
        }
    }
}
