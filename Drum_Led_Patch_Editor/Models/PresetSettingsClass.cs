using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.IO;

namespace Drum_Led_Patch_Editor.Models
{
    [System.Serializable]
    public class PresetSettingsClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

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

        public int GetProgrammedLedsFrameLength()
        {
            int res = 0;
            foreach (LEDStrip strip in LEDStrips)
            {
                if (strip.ProgrammedLedsCount > res)
                    res = strip.ProgrammedLedsCount;
            }
            return res;
        }

        public virtual void DeSerializeFromDLP(Stream stream)
        {
            Brightness = stream.ReadByte();
            FPS = stream.ReadByte();
            for (int i = 0; i < 5; i++)
            {
                LEDStrips[i].ConnectionDirection = (ConnectionDirectionType)stream.ReadByte();
                int buf = stream.ReadByte();
                if (buf == 0) { LEDStrips[i].HasAdditionalStrip = false; } else { LEDStrips[i].HasAdditionalStrip = true; }
                LEDStrips[i].AdditionalStripBehaviour = (AdditionalStripBehaviourType)stream.ReadByte();
                LEDStrips[i].TriggerLedsColorBehaviour = (TriggerLedsColorBehaviourType)stream.ReadByte();
                LEDStrips[i].TriggerLedsColor = new System.Windows.Media.Color()
                {
                    R = (byte)stream.ReadByte(),
                    G = (byte)stream.ReadByte(),
                    B = (byte)stream.ReadByte(),
                    A = 255
                };
                LEDStrips[i].TriggerLedsCount = stream.ReadByte();
                LEDStrips[i].ProgrammedLedsCount = stream.ReadByte();
            }
        }

        public virtual void SerializeToDLP(Stream stream)
        {
            stream.WriteByte((byte)Brightness);
            stream.WriteByte((byte)FPS);
            foreach (LEDStrip strip in LEDStrips)
            {
                stream.WriteByte((byte)strip.ConnectionDirection);
                if (strip.HasAdditionalStrip) { stream.WriteByte((byte)1); } else { stream.WriteByte((byte)0); }
                stream.WriteByte((byte)strip.AdditionalStripBehaviour);
                stream.WriteByte((byte)strip.TriggerLedsColorBehaviour);
                stream.WriteByte((byte)strip.TriggerLedsColor.R);
                stream.WriteByte((byte)strip.TriggerLedsColor.G);
                stream.WriteByte((byte)strip.TriggerLedsColor.B);
                stream.WriteByte((byte)strip.TriggerLedsCount);
                stream.WriteByte((byte)strip.ProgrammedLedsCount);
            }
        }

        public PresetSettingsClass(PresetSettingsClass sourcePresetSettings) : this()
        {
            FPS = sourcePresetSettings.FPS;
            Brightness = sourcePresetSettings.Brightness;

            foreach (LEDStrip strip in sourcePresetSettings.LEDStrips)
            {
                LEDStrips.Add(new LEDStrip
                {
                    AdditionalStripBehaviour = strip.AdditionalStripBehaviour,
                    ConnectionDirection = strip.ConnectionDirection,
                    HasAdditionalStrip = strip.HasAdditionalStrip,
                    Name = strip.Name,
                    ProgrammedLedsCount = strip.ProgrammedLedsCount,
                    TriggerLedsColor = strip.TriggerLedsColor,
                    TriggerLedsColorBehaviour = strip.TriggerLedsColorBehaviour,
                    TriggerLedsCount = strip.TriggerLedsCount
                });
            }
        }

        public PresetSettingsClass(bool autoCreateLEDStrips) : this()
        {
            if (autoCreateLEDStrips)
            {
                LEDStrips.Add(new LEDStrip("Бочка"));
                LEDStrips.Add(new LEDStrip("Рабочий"));
                LEDStrips.Add(new LEDStrip("Малый Альт"));
                LEDStrips.Add(new LEDStrip("Средний Альт"));
                LEDStrips.Add(new LEDStrip("Том"));
            }
        }

        public PresetSettingsClass()
        {
            Brightness = 255;
            FPS = 30;
            LEDStrips = new ObservableCollection<LEDStrip>();
        }
    }
}
