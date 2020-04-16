using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Drum_Led_Patch_Editor.ViewModels
{
    class GlediatorImportViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private int destWidth;
        public int DestWidth
        {
            get { return destWidth; }
            set
            {
                destWidth = value;
                OnPropertyChanged("DestWidth");
            }
        }

        private int sourceWidth;
        public int SourceWidth
        {
            get { return sourceWidth; }
            set
            {
                sourceWidth = value;
                OnPropertyChanged("SourceWidth");
            }
        }

        private int sourceHeight;
        public int SourceHeight
        {
            get { return sourceHeight; }
            set
            {
                sourceHeight = value;
                OnPropertyChanged("SourceHeight");
            }
        }

    }
}
