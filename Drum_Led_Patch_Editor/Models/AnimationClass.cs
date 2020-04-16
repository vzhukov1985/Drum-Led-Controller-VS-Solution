using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.IO;

namespace Drum_Led_Patch_Editor.Models
{
    class AnimationClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private ObservableCollection<Frame> frames;
        public ObservableCollection<Frame> Frames
        {
            get { return frames; }
            set
            {
                frames = value;
                OnPropertyChanged("Frames");
            }
        }

        public void DeSerializeFromRawBinaryRGB(Stream stream, int framesCount, int frameLength)
        {
            Frames.Clear();
            for (int i = 0; i <framesCount; i++)
            {
                Frame newFrame = new Frame();
                newFrame.DeSerializeFromRawBinaryRGB(stream, frameLength);
                Frames.Add(newFrame);
            }
        }

        public void SerializeToRawBinaryRGB(Stream stream)
        {
            foreach (Frame frame in Frames)
                frame.SerializeToRawBinaryRGB(stream);
        }

        public AnimationClass()
        {
            Frames = new ObservableCollection<Frame>();
        }
    }
}
