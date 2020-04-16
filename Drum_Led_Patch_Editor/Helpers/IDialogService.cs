using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Drum_Led_Patch_Editor.Models;

namespace Drum_Led_Patch_Editor.Helpers
{
    interface IDialogService
    {
        string FilePath { get; set; }
        int GlediatorFileWidth { get; set; }
        int GlediatorFileHeight { get; set; }

        PresetSettingsClass PresetSettings { get; set; }

        bool? ShowOkCancelDialog(string Title, string Question);
        bool? ShowSettingsDialog(PresetSettingsClass presetSettings);
        bool ShowOpenPresetFileDialog();
        bool ShowSavePresetAsFileDialog();
        bool ShowOpenGlediatorFileDialog();
        bool ShowImportGlediatorParamsDialog(int destWidth, int sourceWidth, int sourceHeight);
    }
}
