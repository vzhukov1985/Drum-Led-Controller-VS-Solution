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
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)27);

                //Snare
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)1);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)5);
                dlpFileWriter.Write((byte)15);

                //HiTom
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)1);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)22);
                dlpFileWriter.Write((byte)17);

                //MidTom
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)1);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)27);
                dlpFileWriter.Write((byte)17);

                //LowTom
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)1);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)0);
                dlpFileWriter.Write((byte)37);
                dlpFileWriter.Write((byte)24);

                //Frame Length
                dlpFileWriter.Write((byte)27);
                //Frame Count
                dlpFileWriter.Write((UInt16)131);
                //FPS
                dlpFileWriter.Write((byte)30);
                //Triggered Leds Brightness
                dlpFileWriter.Write((byte)255);
                //Programmed Leds Brightness
                dlpFileWriter.Write((byte)255);

                //Frames Data
                for (int i = 0; i < 131; i++)
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            BinaryWriter iniFileWriter = new BinaryWriter(File.Create("D:\\Settings.ini"));
            iniFileWriter.Write((UInt16)30); //trigHitShowSpeed

            for (int i = 0; i < 5; i++)
            {
                iniFileWriter.Write((UInt16)10); //LowThreshold
                iniFileWriter.Write((UInt16)1023); //HighThreshold
                iniFileWriter.Write((UInt16)20); //DetectPeriod 
            }

        }
    }
}
