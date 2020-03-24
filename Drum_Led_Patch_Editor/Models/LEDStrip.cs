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
    enum ConnectionDirectionType
    {
        TrigProg,
        ProgTrig
    }

    enum AdditionalStripBehaviourType
    {
        AlwaysOff,
        AlwaysOn,
        TriggerActivated
    }

    enum TriggerLedsColorBehaviourType
    {
        Constant,
        Random
    }

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

        #region Properties

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }
        internal ConnectionDirectionType ConnectionDirection
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

        internal AdditionalStripBehaviourType AdditionalStripBehaviour
        {
            get { return additionalStripBehaviour; }
            set
            {
                additionalStripBehaviour = value;
                OnPropertyChanged("AdditionalStripBehaviour");
            }
        }

        internal TriggerLedsColorBehaviourType TriggerLedsColorBehaviour
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

        #endregion Properties


        #region PropertyChanged Interface
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
#endregion PropertyChanged Interface


        public LEDStrip(string Name)
        {
            this.Name = Name;
            ConnectionDirection = ConnectionDirectionType.ProgTrig;
            HasAdditionalStrip = false;
            AdditionalStripBehaviour = AdditionalStripBehaviourType.AlwaysOff;
            TriggerLedsColorBehaviour = TriggerLedsColorBehaviourType.Random;
            TriggerLedsColor = Color.FromRgb(0, 0, 0);
            TriggerLedsCount = 0;
            ProgrammedLedsCount = 5;
        }
    }
}
