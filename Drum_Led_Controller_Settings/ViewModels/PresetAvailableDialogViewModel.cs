using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Drum_Led_Controller_Settings.ViewModels
{
    class PresetAvailableDialogViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }


        private bool addExisting;
        private bool addWithNewName;
        private string newName;
        private bool okEnabled;

        public List<string> ListNames;


        public PresetAvailableDialogViewModel()
        {
            AddExisting = true;
            AddWithNewName = false;
            NewName = "";
            ListNames = new List<string>();
        }

        public bool OkEnabled 
        { 
            get { return okEnabled; } 
            set
            {
                okEnabled = value;
                OnPropertyChanged("OkEnabled");
            }
        }

        public bool AddExisting
        {
            get { return addExisting; }
            set
            {
                addExisting = value;
                CheckOkEnabled();
                OnPropertyChanged("AddExisting");
            }
        }

        public bool AddWithNewName
        {
            get { return addWithNewName; }
            set
            {
                addWithNewName = value;
                CheckOkEnabled();
                OnPropertyChanged("AddWithNewName");
            }
        }

        public string NewName
        {
            get { return newName; }
            set
            {
                newName = value;
                CheckOkEnabled();
                OnPropertyChanged("NewName");
            }
        }

        public void CheckOkEnabled()
        {
            bool nameFound = false;

            if (ListNames != null)
            {
                foreach (string PresetName in ListNames)
                {
                    if (NewName == PresetName)
                        nameFound = true;
                }
            }

            if (((AddWithNewName == true) && (NewName.Length > 0) && (nameFound == false)) || (AddExisting == true))
            {
                OkEnabled = true;
            }
            else
            {
                OkEnabled = false;
            }
        }
    }
}
