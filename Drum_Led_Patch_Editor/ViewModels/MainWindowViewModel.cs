using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Drum_Led_Patch_Editor.Models;

namespace Drum_Led_Patch_Editor.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private int brightness;
        private int fps;

        public ObservableCollection<LEDStrip> LEDStrips { get; set; }

        public int Brightness
        {
            get { return brightness; }
            set
            {
                if (value < 1)
                    value = 1;
                if (value > 255)
                    value = 255;

                brightness = value;
                OnPropertyChanged("Brightness");       
            }
        }

        public int FPS
        {
            get { return fps; }
            set
            {
                if (value < 1)
                    value = 1;
                if (value > 30)
                    value = 30;

                fps = value;
                OnPropertyChanged("FPS");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public MainWindowViewModel()
        {
            Brightness = 255;
            FPS = 30;
            LEDStrips = new ObservableCollection<LEDStrip>
            {
                new LEDStrip("Бочка"),
                new LEDStrip("Рабочий"),
                new LEDStrip("Малый Альт"),
                new LEDStrip("Средний Альт"),
                new LEDStrip("Том")
            };
        }

    }
}
