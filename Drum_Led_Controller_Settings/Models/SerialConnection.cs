using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;

namespace Drum_Led_Controller_Settings.Models
{
    class SerialConnection: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private readonly SerialPort serial;

        public ControllerSettings Settings { get; set; }
        public AsyncObservableCollection<String> PresetsList { get; set; }

        public string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
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
                    byte presetNameLength = (byte)serial.ReadByte();
                    byte[] presetName = new byte[presetNameLength];
                    serial.Read(presetName, 0, presetNameLength);
                    PresetsList.Add(Encoding.ASCII.GetString(presetName));
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

        public void SendFileToController (string SourceFilePathName, string PresetName)
        {
           if (serial.IsOpen)
            {
                serial.Write("PCADFL");
                FileStream sourceFileStream = File.OpenRead(SourceFilePathName);
                BinaryReader fileRead = new BinaryReader(sourceFileStream);
                
                serial.Write(BitConverter.GetBytes((byte)PresetName.Length), 0, 1);
                serial.Write(PresetName);

                int FileLength = (int)sourceFileStream.Length;
                serial.Write(BitConverter.GetBytes((UInt32)FileLength), 0, 4);
                byte[] data = new byte[FileLength];
                fileRead.Read(data, 0, FileLength);
                serial.Write(data, 0, FileLength);
                sourceFileStream.Close();
            }
        }

        public void UpdatePresetsListFile()
        {
            if (serial.IsOpen)
            {
                serial.Write("PCUDPL");
                MemoryStream memoryStream = new MemoryStream();
                 memoryStream.Write(BitConverter.GetBytes((UInt16)PresetsList.Count), 0, 2);
                 foreach (string presetName in PresetsList)
                 {
                     memoryStream.Write(BitConverter.GetBytes((byte)presetName.Length), 0, 1);
                     memoryStream.Write(Encoding.ASCII.GetBytes(presetName),0,presetName.Length);
                 }
                serial.Write(BitConverter.GetBytes((UInt32)memoryStream.Length),0,4);
                memoryStream.Seek(0, SeekOrigin.Begin);
                serial.Write(memoryStream.GetBuffer(),0,(int)memoryStream.Length);
            }
        }

        public void RemovePreset(string removePresetName)
        {
            if (serial.IsOpen)
            {
                serial.Write("PCREMP");
                serial.Write(BitConverter.GetBytes((byte)removePresetName.Length), 0, 1);
                serial.Write(removePresetName);
            }
        }

        public bool Connect(string PortName)
        {
            if (serial.IsOpen)
                serial.Close();
            serial.PortName = PortName;
            serial.BaudRate = 12000000;
            serial.DataReceived += OnDataReceived;
            serial.Open();
            if (serial.IsOpen)
            {
                serial.Write("PCREQS");
                Thread.Sleep(300);
                serial.Write("PCRQPL");
                return true;
            }
            return false;
        }

        public void CloseConnection()
        {
            if (serial.IsOpen)
                serial.Close();
        }

        public SerialConnection()
        {
            serial = new SerialPort();
        }
    }
}
