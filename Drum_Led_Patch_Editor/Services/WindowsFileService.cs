using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Drum_Led_Patch_Editor.Helpers;
using Drum_Led_Patch_Editor.Models;
using System.Windows.Media;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Drum_Led_Patch_Editor.Services
{
    class WindowsFileService : IFileService
    {
        private string settingsFilePath;

        public string SettingsFilesPath
        {
            get { return settingsFilePath; }
            set { settingsFilePath = value; }
        }


        public PresetSettingsClass LoadDefaultSettings()
        {
            XmlSerializer xml = new XmlSerializer(typeof(PresetSettingsClass));
            using (StreamReader fileStream = new StreamReader(SettingsFilesPath + "DefaultProfile.xml"))
            {
                return (PresetSettingsClass)xml.Deserialize(fileStream);
            }
        }

        public void SaveDefaultSettings(PresetSettingsClass presetSettings)
        {
            XmlSerializer xml = new XmlSerializer(typeof(PresetSettingsClass));
            using (StreamWriter fileStream = new StreamWriter(SettingsFilesPath + "DefaultProfile.xml"))
            {
                xml.Serialize(fileStream, presetSettings);
            }
        }

        public ObservableCollection<SettingsProfile> LoadProfilesList()
        {
            XmlSerializer xml = new XmlSerializer(typeof(ObservableCollection<SettingsProfile>));
            using (StreamReader fileStream = new StreamReader(SettingsFilesPath + "Profiles.xml"))
            {
                return (ObservableCollection<SettingsProfile>)xml.Deserialize(fileStream);
            }
        }

        public void SaveProfilesList(ObservableCollection<SettingsProfile> settingsProfiles)
        {
            XmlSerializer xml = new XmlSerializer(typeof(ObservableCollection<SettingsProfile>));
            using (StreamWriter fileStream = new StreamWriter(SettingsFilesPath + "Profiles.xml"))
            {
                xml.Serialize(fileStream, settingsProfiles);
            }
        }

        public void OpenPreset(string fileName, PresetClass preset)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open);
            preset.DeSerializeFromDLP(fileStream);
            fileStream.Close();
        }

        public void SavePreset(string fileName, PresetClass preset)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Create);
            preset.SerializeToDLP(fileStream);
            fileStream.Close();
        }

        public AnimationClass ImportFromGlediatiorFile(string fileName, int destFrameLength, int sourceWidth, int sourceHeight)
        {
            AnimationClass animation = new AnimationClass();

            BinaryReader gledFileReader = new BinaryReader(File.OpenRead(fileName));

            int framesToAdd = (int)(gledFileReader.BaseStream.Length / (sourceWidth * 3 * sourceHeight));

            for (int i=0; i < framesToAdd;i++)
            {
                animation.Frames.Add(new Frame(destFrameLength));

                for (int j = 0; j < sourceWidth; j++)
                {
                    byte r, g, b;
                    r = gledFileReader.ReadByte();
                    g = gledFileReader.ReadByte();
                    b = gledFileReader.ReadByte();
                    animation.Frames[i].Pixels[j] = new Color()
                    {
                        R = r,
                        G = g,
                        B = b,
                        A = 255
                    };
                }

                if (sourceWidth > destFrameLength)
                    gledFileReader.ReadBytes((destFrameLength - sourceWidth) * 3);

                gledFileReader.ReadBytes(3 * sourceWidth * (sourceHeight - 1));
            }

            return animation;
        }


        public WindowsFileService()
        {
            SettingsFilesPath = System.AppDomain.CurrentDomain.BaseDirectory + "Settings\\";
        }

    }
}
