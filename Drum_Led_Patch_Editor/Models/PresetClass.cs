using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;

namespace Drum_Led_Patch_Editor.Models
{
    [Serializable]
    class PresetClass : PresetSettingsClass
    {
        private AnimationClass animation;
        public AnimationClass Animation
        {
            get { return animation; }
            set
            {
                animation = value;
                OnPropertyChanged("Animation");
            }
        }

        public override void DeSerializeFromDLP(Stream stream)
        {
            base.DeSerializeFromDLP(stream);
            int frameLength = stream.ReadByte(); //FramLength Only For Controller
            byte[] buf = new byte[2];
            stream.Read(buf, 0, 2);
            int framesCount = BitConverter.ToUInt16(buf, 0);
            Animation.DeSerializeFromRawBinaryRGB(stream, framesCount, frameLength);
        }

        public override void SerializeToDLP(Stream stream)
        {
            base.SerializeToDLP(stream);
            stream.WriteByte((byte)GetProgrammedLedsFrameLength());
            byte[] buf = BitConverter.GetBytes((UInt16)Animation.Frames.Count);
            stream.Write(buf, 0, buf.Length);
            Animation.SerializeToRawBinaryRGB(stream);
        }

        public PresetClass() : base()
        {
            Animation = new AnimationClass();
        }

        public PresetClass(PresetSettingsClass presetSettings):base(presetSettings)
        {
            Animation = new AnimationClass();
        }
    }
}
