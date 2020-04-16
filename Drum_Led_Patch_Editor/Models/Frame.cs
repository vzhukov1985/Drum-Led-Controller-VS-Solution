using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.IO;

namespace Drum_Led_Patch_Editor.Models
{
    class Frame: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private ObservableCollection<Color> pixels;
        public ObservableCollection<Color> Pixels
        {
            get { return pixels; }
            set
            {
                pixels = value;
                OnPropertyChanged("Pixels");
            }
        }

        public void DeSerializeFromRawBinaryRGB(Stream stream, int frameLength)
        {
            Pixels.Clear();
            for (int i = 0; i < frameLength; i++)
            {
                Pixels.Add(new Color()
                {
                    R = (byte)stream.ReadByte(),
                    G = (byte)stream.ReadByte(),
                    B = (byte)stream.ReadByte(),
                    A = 255
                });
            }
        }

        public void SerializeToRawBinaryRGB(Stream stream)
        {
            foreach (Color pixel in Pixels)
            {
                stream.WriteByte(pixel.R);
                stream.WriteByte(pixel.G);
                stream.WriteByte(pixel.B);
            }
        }

        public Frame()
        {
            Pixels = new ObservableCollection<Color>();
        }

        public Frame(int frameLength): this()
        {
            for (int i = 0; i < frameLength; i++)
            {
                Pixels.Add(new Color()
                {
                    R = 0,
                    G = 0,
                    B = 0,
                    A = 255
                });
            }
        }
    }
}
