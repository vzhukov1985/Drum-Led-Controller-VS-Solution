using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Drum_Led_Controller_Settings.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.IO.Ports;
using Microsoft.Win32;
using Drum_Led_Controller_Settings.Views;
using System.Diagnostics;
using System.IO;

namespace Drum_Led_Controller_Settings.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public ControllerSettings Settings { get; set; }

        private SerialPort serial;

        public ObservableCollection<String> PresetsList { get; set; }


        private bool serialConnected;
        public bool SerialConnected
        {
            get { return serialConnected; }
            set
            {
                serialConnected = value;
                OnPropertyChanged("SerialConnected");
            }
        }

        private string[] availableComPorts;
        public string[] AvailableComPorts
        {
            get { return availableComPorts; }
            set
            {
                availableComPorts = value;
                OnPropertyChanged("AvailableComPorts");
            }
        }


        //Commands
        public RelayCommand ExitCommand { get; }
        public RelayCommand UpdateSerialPortsCommand { get; }
        public RelayCommand ConnectCommand { get; }
        public RelayCommand AddPresetCommand { get; }




        public void UpdateAvailableComPorts()
        {
            AvailableComPorts = SerialPort.GetPortNames();
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] InCommandHeader = new byte[6];
            serial.Read(InCommandHeader, 0, 6);
            string CommandHeader = System.Text.Encoding.ASCII.GetString(InCommandHeader);
            //Получение от контроллера настроек
            if (CommandHeader == "CPSETS")
            {
                byte[] val = new byte[2];
                serial.Read(val, 0, 2);
                Settings.TrigHitShowSpeed = BitConverter.ToUInt16(val, 0);

                for (int i = 0; i < 5; i++)
                {
                    serial.Read(val, 0, 2);
                    Settings.TriggerSettings[i].DetectPeriod = BitConverter.ToUInt16(val, 0);
                    serial.Read(val, 0, 2);
                    Settings.TriggerSettings[i].LowThreshold = BitConverter.ToUInt16(val, 0);
                    serial.Read(val, 0, 2);
                    Settings.TriggerSettings[i].HighThreshold = BitConverter.ToUInt16(val, 0);
                }
            }

            //Получение от контроллера списка презетов
            if (CommandHeader == "CPRQPL")
            {
                PresetsList.Clear();

                byte[] val = new byte[2];
                serial.Read(val, 0, 2);
                int presetsCount = BitConverter.ToUInt16(val, 0);

                for (int i = 0; i < presetsCount; i++)
                {
                    byte fileNameLength = (byte)serial.ReadByte();
                    byte[] fileName = new byte[fileNameLength];
                    serial.Read(fileName, 0, fileNameLength);
                    PresetsList.Add(Encoding.ASCII.GetString(fileName));
                }
            }

        }

        public void SendSettingsToController()
        {
            if (serial.IsOpen)
            {
                serial.Write("PCSETS");

                byte[] val = BitConverter.GetBytes((UInt16)Settings.TrigHitShowSpeed);
                serial.Write(val, 0, 2);
                for (int i = 0; i < Settings.TriggerSettings.Count; i++)
                {
                    val = BitConverter.GetBytes((UInt16)Settings.TriggerSettings[i].DetectPeriod);
                    serial.Write(val, 0, 2);
                    val = BitConverter.GetBytes((UInt16)Settings.TriggerSettings[i].LowThreshold);
                    serial.Write(val, 0, 2);
                    val = BitConverter.GetBytes((UInt16)Settings.TriggerSettings[i].HighThreshold);
                    serial.Write(val, 0, 2);
                }
            }
        }

        public void Connect(string PortName)
        {
            if (serial.IsOpen)
                serial.Close();
            serial.PortName = PortName;
            serial.BaudRate = 12000000;
            serial.DataReceived += OnDataReceived;
            serial.Open();
            if (serial.IsOpen)
            {
                SerialConnected = true;
                serial.Write("PCREQS");
                Thread.Sleep(300);
                serial.Write("PCRQPL");
            }
        }

        public void CloseConnection()
        {
            if (serial.IsOpen)
                serial.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        public void AddPreset()
        {
            OpenFileDialog dlgOpen = new OpenFileDialog();
            dlgOpen.Filter = "Drum LED Controller Presets|*.dlp";
            dlgOpen.Title = "Добавить презет...";

            if (dlgOpen.ShowDialog() == true)
            {
                String sourceFileName = dlgOpen.FileName;
                String destinationPresetName = sourceFileName;
                bool presetAvailable = false;
                foreach (String availablePreset in PresetsList)
                {
                    if (sourceFileName == availablePreset)
                    {
                        presetAvailable = true;
                    }
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
                            destinationPresetName = Path.GetFileNameWithoutExtension(sourceFileName);
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
            }


        }



        public MainWindowViewModel()
        {
            Settings = new ControllerSettings();
            Settings.OnSettingsUpdated = SendSettingsToController;
            for (int i = 0; i < Settings.TriggerSettings.Count; i++)
                Settings.TriggerSettings[i].OnSettingsUpdated = SendSettingsToController;

            serial = new SerialPort();


            PresetsList = new AsyncObservableCollection<string>();

            //Commands
            ExitCommand = new RelayCommand(o =>
            {
                serial.Close();
                (o as MainWindow).Close();
            });
            UpdateSerialPortsCommand = new RelayCommand(_ => { UpdateAvailableComPorts(); });
            ConnectCommand = new RelayCommand(o => Connect((string)o), o => ((string)o != null));
            AddPresetCommand = new RelayCommand(_ => { AddPreset(); });

        }

    }
}
