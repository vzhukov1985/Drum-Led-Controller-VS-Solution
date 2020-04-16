using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Drum_Led_Patch_Editor.Models;

namespace Drum_Led_Patch_Editor.Helpers
{
    interface IFileService
    {
        PresetSettingsClass LoadDefaultSettings();
        void SaveDefaultSettings(PresetSettingsClass presetSettings);
        ObservableCollection<SettingsProfile> LoadProfilesList();
        void SaveProfilesList(ObservableCollection<SettingsProfile> settingsProfiles);

        void OpenPreset(string fileName, PresetClass preset);
        void SavePreset(string fileName, PresetClass preset);
        
        AnimationClass ImportFromGlediatiorFile(string fileName, int destFrameLength, int sourceWidth, int sourceHeight);
    }
}
