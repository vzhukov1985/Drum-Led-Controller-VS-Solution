using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drum_Led_Controller_Settings.Classes;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.IO.Ports;

namespace Drum_Led_Controller_Settings.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public ControllerSettings Settings { get; set; }

        private SerialPort serial;

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

        public void UpdateAvailableComPorts()
        {
            AvailableComPorts = SerialPort.GetPortNames();
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] InCommandHeader = new byte[6];
            serial.Read(InCommandHeader, 0, 6);
            string CommandHeader = System.Text.Encoding.ASCII.GetString(InCommandHeader);
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









        //Commands
        public RelayCommand ExitCommand { get; }
        public RelayCommand UpdateSerialPortsCommand { get; }
        public RelayCommand ConnectCommand { get; }



        public MainWindowViewModel()
        {
            Settings = new ControllerSettings();
            Settings.OnSettingsUpdated = SendSettingsToController;
            for (int i = 0; i < Settings.TriggerSettings.Count; i++)
                Settings.TriggerSettings[i].OnSettingsUpdated = SendSettingsToController;

            serial = new SerialPort();
            //Commands
            ExitCommand = new RelayCommand(o =>
            {
                serial.Close();
                (o as MainWindow).Close();
            });
            UpdateSerialPortsCommand = new RelayCommand(_ => { UpdateAvailableComPorts(); });
            ConnectCommand = new RelayCommand(o => Connect((string)o), o => ((string)o != null));
        }

    }
}
