using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;



namespace Drum_Led_Controller_Settings.Models
{
    class ControllerSettings: INotifyPropertyChanged
    {
        private int trigHitShowSpeed;
        public delegate void SettingsUpdateHandler();

        public SettingsUpdateHandler OnSettingsUpdated { get; set; }


        public ObservableCollection<TriggerSetting> TriggerSettings { get; set; }

        public int TrigHitShowSpeed
        {
            get
            {
                return trigHitShowSpeed;
            }
            set
            {
                trigHitShowSpeed = value;
                OnPropertyChanged("TrigHitShowSpeed");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (OnSettingsUpdated != null) OnSettingsUpdated();
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public ControllerSettings()
        {
            TrigHitShowSpeed = 30;
            TriggerSettings = new ObservableCollection<TriggerSetting>
            {
                new TriggerSetting { Name = "Триггер 1 - Бочка", DetectPeriod = 20, HighThreshold = 1023, LowThreshold = 20 },
                new TriggerSetting { Name = "Триггер 2 - Рабочий", DetectPeriod = 20, HighThreshold = 1023, LowThreshold = 20 },
                new TriggerSetting { Name = "Триггер 3 - Малый альт", DetectPeriod = 20, HighThreshold = 1023, LowThreshold = 20 },
                new TriggerSetting { Name = "Триггер 4 - Средний альт", DetectPeriod = 20, HighThreshold = 1023, LowThreshold = 20 },
                new TriggerSetting { Name = "Триггер 5 - Том", DetectPeriod = 20, HighThreshold = 1023, LowThreshold = 20 },
            };
        }
    }
}
