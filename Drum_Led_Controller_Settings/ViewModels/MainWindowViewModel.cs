using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drum_Led_Controller_Settings.Models;
using Drum_Led_Controller_Settings.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace Drum_Led_Controller_Settings.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private SerialConnection Serial { get; set; }
        private string[] availableComPorts;
        private bool serialConnected;
        private int selectedPresetIndex;

        public ControllerSettings Settings { get; set; }
        public ObservableCollection<String> PresetsList { get; set; }

        public string[] AvailableComPorts
        {
            get { return availableComPorts; }
            set
            {
                availableComPorts = value;
                OnPropertyChanged("AvailableComPorts");
            }
        }

        public int SelectedPresetIndex
        {
            get { return selectedPresetIndex; }
            set
            {
                selectedPresetIndex = value;
                OnPropertyChanged("SelectedPresetIndex");
            }
        }

        public bool SerialConnected
        {
            get
            {
                return serialConnected;
            }
            set
            {
                serialConnected = value;
                OnPropertyChanged("SerialConnected");
            }
        }


        //Commands
        public RelayCommand ExitCommand { get; }
        public RelayCommand UpdateSerialPortsCommand { get; }
        public RelayCommand ConnectCommand { get; }
        public RelayCommand AddPresetCommand { get; }
        public RelayCommand RemovePresetCommand { get; }
        public RelayCommand MoveUpPresetCommand { get; }
        public RelayCommand MoveDownPresetCommand { get; }

        private void UpdateAvailableComPorts()
        {
            AvailableComPorts = Serial.GetPortNames();
        }


        public void AddPreset(int index)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog
            {
                Filter = "Drum LED Controller Presets|*.dlp",
                Title = "Добавить презет..."
            };

            if (dlgOpen.ShowDialog() == true)
            {
                String sourceFileName = dlgOpen.FileName;
                String destinationPresetName = Path.GetFileNameWithoutExtension(sourceFileName);
                bool presetAvailable = false;
                foreach (String availablePreset in PresetsList)
                {
                    if (Path.GetFileNameWithoutExtension(sourceFileName) == availablePreset)
                        presetAvailable = true;
                }

                bool addExistingPreset = false;

                if (presetAvailable)
                {
                    PresetAvailableDialog dlgPresAvail = new PresetAvailableDialog();
                    PresetAvailableDialogViewModel dlgPresAvailVM = (PresetAvailableDialogViewModel)dlgPresAvail.DataContext;
                    List<string> ListNames = dlgPresAvailVM.ListNames;

                    foreach (string PresetName in PresetsList)
                        ListNames.Add(PresetName);

                    if (dlgPresAvail.ShowDialog() == true)
                    {
                        if (dlgPresAvailVM.AddExisting == true)
                        {
                            addExistingPreset = true;
                        }
                        else
                        {
                            destinationPresetName = dlgPresAvailVM.NewName;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                if (addExistingPreset == false)
                    Serial.SendFileToController(sourceFileName, destinationPresetName);
                if (index > -1)
                { PresetsList.Insert(index, destinationPresetName); }
                else
                { PresetsList.Add(destinationPresetName); }

                Serial.UpdatePresetsListFile();

            }
        }

        public void RemovePreset()
        {
            RemovePresetDialog dlgRemove = new RemovePresetDialog();
            ((RemovePresetDialogViewModel)dlgRemove.DataContext).PresetName = PresetsList[SelectedPresetIndex];

            if (dlgRemove.ShowDialog() == true)
            {
                int PresetsWithTheSameName = (int)PresetsList.LongCount(presetCheck => presetCheck == PresetsList[SelectedPresetIndex]);
                if (PresetsWithTheSameName < 2)
                    Serial.RemovePreset(PresetsList[SelectedPresetIndex]);
                PresetsList.RemoveAt(SelectedPresetIndex);
                Serial.UpdatePresetsListFile();
            }
        }

        public void MoveUpPreset()
        {
            PresetsList.Move(SelectedPresetIndex, SelectedPresetIndex - 1);
            Serial.UpdatePresetsListFile();
        }

        public void MoveDownPreset()
        {
            PresetsList.Move(SelectedPresetIndex, SelectedPresetIndex + 1);
            Serial.UpdatePresetsListFile();
        }

        public MainWindowViewModel()
        {
            Serial = new SerialConnection();

            Settings = new ControllerSettings
            {
                OnSettingsUpdated = Serial.SendSettingsToController
            };
            for (int i = 0; i < Settings.TriggerSettings.Count; i++)
                Settings.TriggerSettings[i].OnSettingsUpdated = Serial.SendSettingsToController;

            PresetsList = new AsyncObservableCollection<string>();

            Serial.Settings = Settings;
            Serial.PresetsList = (AsyncObservableCollection<string>)PresetsList;
            SerialConnected = false;

            SelectedPresetIndex = -1;

            //Commands
            ExitCommand = new RelayCommand(o =>
            {
                Serial.CloseConnection();
                (o as MainWindow).Close();
            });

            UpdateSerialPortsCommand = new RelayCommand(_ => { UpdateAvailableComPorts(); });

            ConnectCommand = new RelayCommand(o =>
            {
                if (Serial.Connect((string)o) == true)
                    SerialConnected = true;
            },
            o => ((string)o != null));

            AddPresetCommand = new RelayCommand(o =>
            {
                AddPreset((int)o);
            },
            _ =>
            {
                return SerialConnected;
            });

            RemovePresetCommand = new RelayCommand(_ => { RemovePreset(); }, _ => { return (SelectedPresetIndex > -1); });

            MoveUpPresetCommand = new RelayCommand(_ => { MoveUpPreset(); }, _ => { return (SelectedPresetIndex > 0); });

            MoveDownPresetCommand = new RelayCommand(_ => { MoveDownPreset(); }, _ => { return ((SelectedPresetIndex > -1) && (SelectedPresetIndex < PresetsList.Count - 1)); });
        }

    }
}
