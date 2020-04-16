using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drum_Led_Patch_Editor.Views;
using Drum_Led_Patch_Editor.ViewModels;
using Drum_Led_Patch_Editor.Models;
using Drum_Led_Patch_Editor.Helpers;
using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace Drum_Led_Patch_Editor.Services
{
    class WindowsDialogService: IDialogService
    {
        private Window DialogsOwner;

        public string FilePath { get; set; }
        public int GlediatorFileWidth { get; set; }
        public int GlediatorFileHeight { get; set; }
        public PresetSettingsClass PresetSettings { get; set; }

        public bool? ShowOkCancelDialog(string Title, string Question)
        {
            OkCancelDialog dlgOkCancel = new OkCancelDialog()
            {
                Owner = DialogsOwner
            };
            OkCancelDialogViewModel dlgOkCancelVM = (OkCancelDialogViewModel)dlgOkCancel.DataContext;
            dlgOkCancelVM.Title = Title;
            dlgOkCancelVM.Question = Question;

            return dlgOkCancel.ShowDialog();
        }

        public bool? ShowSettingsDialog(PresetSettingsClass presetSettings)
        {
            SettingsDialog dlgSettings = new SettingsDialog()
            {
                Owner = DialogsOwner
            };
            SettingsDialogViewModel dlgSettingsVM = (SettingsDialogViewModel)dlgSettings.DataContext;
            dlgSettingsVM.PresetSettings = new PresetSettingsClass(presetSettings);
            PresetSettings = dlgSettingsVM.PresetSettings;

            return dlgSettings.ShowDialog();
        }

        public bool OpenFileDialog(string title, string filter)
        {
            OpenFileDialog dlgOpen = new OpenFileDialog()
            {
                Title = title,
                Filter = filter
            };
            if (dlgOpen.ShowDialog() == true)
            {
                FilePath = dlgOpen.FileName;
                return true;
            }
            return false;
        }

        public bool ShowOpenPresetFileDialog()
        {
            return OpenFileDialog("Открыть программу", "Drums LED Controller Preset|*.dlp");
        }

        public bool SaveFileDialog(string title, string filter)
        {
            SaveFileDialog dlgSave = new SaveFileDialog()
            {
                Title = title,
                Filter = filter
            };
            if (dlgSave.ShowDialog() == true)
            {
                FilePath = dlgSave.FileName;
                return true;
            }
            return false;
        }

        public bool ShowSavePresetAsFileDialog()
        {
            if (SaveFileDialog("Сохранить программу как", "Drums LED Controller Preset|*.dlp"))
            {
                if (File.Exists(FilePath))
                    return (bool) ShowOkCancelDialog("Файл уже существует", "Файл с таким именем уже существует. Перезаписать?");
                return true;
            }
            return false;
        }


        public bool ShowOpenGlediatorFileDialog()
        {
            return OpenFileDialog("Импорт файла Glediator...", "Glediator Files|*.*");
        }

        public bool ShowImportGlediatorParamsDialog(int destWidth, int sourceWidth, int sourceHeight)
        {
            GlediatorImportDialog dlgImport = new GlediatorImportDialog()
            {
                Owner = DialogsOwner
            };
            GlediatorImportViewModel dlgImportVM = (GlediatorImportViewModel)dlgImport.DataContext;
            dlgImportVM.DestWidth = destWidth;
            dlgImportVM.SourceWidth = sourceWidth;
            dlgImportVM.SourceHeight = sourceHeight;

            if (dlgImport.ShowDialog() == true)
            {
                GlediatorFileWidth = dlgImportVM.SourceWidth;
                GlediatorFileHeight = dlgImportVM.SourceHeight;
                return true;
            }

            return false;
        }


        public WindowsDialogService(Window owner)
        {
            DialogsOwner = owner;
            PresetSettings = new PresetSettingsClass();
        }

    }
}
