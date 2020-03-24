using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Drum_Led_Controller_Settings.Classes
{
    class TriggerSetting:INotifyPropertyChanged
    {
        private String name;

        private int detectPeriod;
        private int lowThreshold;
        private int highThreshold;

        public delegate void SettingsUpdateHandler();

        public SettingsUpdateHandler OnSettingsUpdated { get; set; }


        public string Name { get => name; set => name = value; }

        public int DetectPeriod
        {
            get { return detectPeriod; }
            set
            {
                detectPeriod = value;
                OnPropertyChanged("DetectPeriod");
            }
        }

        public int LowThreshold
        {
            get { return lowThreshold; }
            set
            {
                if (value >= (highThreshold - 1))
                    value = highThreshold - 1;
                lowThreshold = value;
                OnPropertyChanged("LowThreshold");
            }
        }

        public int HighThreshold
        {
            get { return highThreshold; }
            set
            {
                if (value <= (lowThreshold + 1))
                    value = lowThreshold + 1;
                highThreshold = value;
                OnPropertyChanged("HighThreshold");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (OnSettingsUpdated != null) OnSettingsUpdated();
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
