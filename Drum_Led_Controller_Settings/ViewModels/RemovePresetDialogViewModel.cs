using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Drum_Led_Controller_Settings.ViewModels
{
    class RemovePresetDialogViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        private string presetName;

        public string PresetName
        {
            get { return presetName; }
            set
            {
                presetName = value;
                OnPropertyChanged("PresetName");
            }
        }

        public RemovePresetDialogViewModel()
        {
            PresetName = "None";
        }

    }
}
