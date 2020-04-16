using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Drum_Led_Patch_Editor.Models
{
    public enum ConnectionDirectionType
    {
        TrigProg,
        ProgTrig
    }

    public enum AdditionalStripBehaviourType
    {
        AlwaysOff,
        AlwaysOn,
        TriggerActivated
    }

    public enum TriggerLedsColorBehaviourType
    {
        Constant,
        Random
    }
    
    [System.Serializable]
    public class LEDStrip : INotifyPropertyChanged
    {
        private string name;
        private ConnectionDirectionType connectionDirection;
        private bool hasAdditionalStrip;
        private AdditionalStripBehaviourType additionalStripBehaviour;
        private TriggerLedsColorBehaviourType triggerLedsColorBehaviour;
        private Color triggerLedsColor;
        private int triggerLedsCount;
        private int programmedLedsCount;


        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        public ConnectionDirectionType ConnectionDirection
        {
            get { return connectionDirection; }
            set { 
                connectionDirection = value;
                OnPropertyChanged("ConnectionDirection");
            }
        }

        public bool HasAdditionalStrip
        {
            get { return hasAdditionalStrip; }
            set
            {
                hasAdditionalStrip = value;
                OnPropertyChanged("HasAdditionalStrip");
            }
        }

        public AdditionalStripBehaviourType AdditionalStripBehaviour
        {
            get { return additionalStripBehaviour; }
            set
            {
                additionalStripBehaviour = value;
                OnPropertyChanged("AdditionalStripBehaviour");
            }
        }

        public TriggerLedsColorBehaviourType TriggerLedsColorBehaviour
        {
            get { return triggerLedsColorBehaviour; }
            set
            {
                triggerLedsColorBehaviour = value;
                OnPropertyChanged("TriggerLedsColorBehaviour");
            }
        }

        public Color TriggerLedsColor
        {
            get { return triggerLedsColor; }
            set
            {
                triggerLedsColor = value;
                OnPropertyChanged("TriggerLedsColor");
            }
        }

        public int TriggerLedsCount
        {
            get { return triggerLedsCount; }
            set
            {
                triggerLedsCount = value;
                OnPropertyChanged("TriggerLedsCount");
            }
        }

        public int ProgrammedLedsCount
        {
            get { return programmedLedsCount; }
            set
            {
                programmedLedsCount = value;
                OnPropertyChanged("ProgrammedLedsCount");
            }
        }




        #region PropertyChanged Interface
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
#endregion PropertyChanged Interface


        public LEDStrip()
        {
            Name = "None";
            ConnectionDirection = ConnectionDirectionType.TrigProg;
            HasAdditionalStrip = false;
            AdditionalStripBehaviour = AdditionalStripBehaviourType.AlwaysOff;
            TriggerLedsColorBehaviour = TriggerLedsColorBehaviourType.Random;
            TriggerLedsColor = Color.FromRgb(0, 0, 0);
            TriggerLedsCount = 0;
            ProgrammedLedsCount = 5;
        }

        public LEDStrip(string Name):this()
        {
            this.Name = Name;
        }
    }
}
