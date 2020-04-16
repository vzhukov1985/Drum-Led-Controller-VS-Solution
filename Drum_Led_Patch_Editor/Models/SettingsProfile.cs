using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Xml.Serialization;
using System.IO;

namespace Drum_Led_Patch_Editor.Models
{
    [System.Serializable]
    public class SettingsProfile: PresetSettingsClass
    {
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private bool isDefault;
        public bool IsDefault
        {
            get { return isDefault; }
            set
            {
                isDefault = value;
                OnPropertyChanged("IsDefault");
            }
        }

        public SettingsProfile(bool autoCreateLEDStrips): base(autoCreateLEDStrips)
        {
            Name = "Default";
            IsDefault = false;
        }

        public SettingsProfile(PresetSettingsClass presetSettings): base(presetSettings)
        {
            Name = "Default";
            IsDefault = false;
        }

        public SettingsProfile() : this(false)
        {
        }
    }
}
