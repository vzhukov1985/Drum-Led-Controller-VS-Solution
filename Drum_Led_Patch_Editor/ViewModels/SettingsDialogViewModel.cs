using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Drum_Led_Patch_Editor.Models;
using Drum_Led_Patch_Editor.Helpers;
using Drum_Led_Patch_Editor.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using System.Windows.Input;
using Drum_Led_Patch_Editor.Views;
using System.Windows.Media;

namespace Drum_Led_Patch_Editor.ViewModels
{
    class SettingsDialogViewModel : INotifyPropertyChanged
    {
        private IDialogService DialogService;
        private IFileService FileService;

        private string profileNameText;
        public string ProfileNameText
        {
            get { return profileNameText; }
            set
            {
                profileNameText = value;
                OnPropertyChanged("ProfileNameText");
            }   
        }

        private PresetSettingsClass presetSettings;
        public PresetSettingsClass PresetSettings
        {
            get { return presetSettings; }      
            set
            {
                presetSettings = value;
                OnPropertyChanged("PresetSettings");
            }
        }



        public ObservableCollection<SettingsProfile> Profiles { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        public void LoadProfile(SettingsProfile profile)
        {
            PresetSettings = new PresetSettingsClass(profile);
        }

        public void SaveProfile(string ProfileName)
        {
            SettingsProfile newProfile = new SettingsProfile(PresetSettings)
            {
                Name = ProfileName
            };

            int overwriteIndex = -1;

            for (int i = 0; i < Profiles.Count; i++)
            {
                if (Profiles[i].Name == ProfileName)
                {
                    overwriteIndex = i;
                    break;
                }

            }

            if (overwriteIndex > -1)
            {
                Profiles[overwriteIndex] = newProfile;
            }
            else
            {
                Profiles.Add(newProfile);
            }

            SaveProfilesList();
        }

        public void DeleteProfile(SettingsProfile profile)
        {
            if (DialogService.ShowOkCancelDialog("Удаление профиля", $"Вы действительно хотите удалить профиль с именем {profile.Name}?") == true)
            {
                Profiles.Remove(profile);
                SaveProfilesList();
            }
        }

        public void SaveProfilesList()
        {
            FileService.SaveProfilesList(Profiles);
        }

        public void LoadProfileByText()
        {
            for (int i = 0; i < Profiles.Count; i++)
            {
                if (Profiles[i].Name == ProfileNameText)
                {
                    LoadProfile(Profiles[i]);
                    return;
                }
            }

        }

        //Commands
        public RelayCommand LoadProfileByListCommand { get; }
        public RelayCommand LoadProfileByTextCommand { get; }
        public RelayCommand SaveProfileCommand { get; }
        public RelayCommand DeleteProfileCommand { get; }
     
        public RelayCommand SaveDefaultSettingsCommand { get; }



        public SettingsDialogViewModel(IDialogService dialogService, IFileService fileService)
        {
            DialogService = dialogService;
            FileService = fileService;

            PresetSettings = new PresetSettingsClass(true);
            Profiles = FileService.LoadProfilesList();

            LoadProfileByListCommand = new RelayCommand(o => { LoadProfile((SettingsProfile)o); }, o => (o != null));
            LoadProfileByTextCommand = new RelayCommand(o => { LoadProfileByText(); }, o => ((KeyEventArgs)o).Key == Key.Enter);
            SaveProfileCommand = new RelayCommand(o =>
            {
                SaveProfile((string)o);
            },
            o => ((string)o != null) && (string)o != "");
            DeleteProfileCommand = new RelayCommand(o => { DeleteProfile((SettingsProfile)o); }, o => ((o != null) && (ProfileNameText == ((SettingsProfile)o).Name)));
            SaveDefaultSettingsCommand = new RelayCommand(_ => { FileService.SaveDefaultSettings(PresetSettings); });
        }

    }
}
