using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Drum_Led_Patch_Editor.Models;
using Drum_Led_Patch_Editor.Services;
using Drum_Led_Patch_Editor.Helpers;
using System.Xml.Serialization;
using System.Timers;

namespace Drum_Led_Patch_Editor.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
		private IDialogService DialogService;
		private IFileService FileService;

		private Timer animationTimer;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

		private PresetClass preset;
		public PresetClass Preset
		{
			get { return preset; }
			set
			{
				preset = value;
				OnPropertyChanged("Preset");
			}
		}

		private int currentFrameIndex;
		public int CurrentFrameIndex
		{
			get { return currentFrameIndex; }
			set
			{
				if (Preset != null)
				{
					if (value > (Preset.Animation.Frames.Count-1))
						value = 0;

					if (value <0)
						value = Preset.Animation.Frames.Count - 1;

					if (value == -1)
						value = 0;

					currentFrameIndex = value;

					if ((Preset.Animation.Frames.Count>0) && (Preset.Animation.Frames[value] != null))
					{
						CurrentFrame = Preset.Animation.Frames[value];
					}
					else 
					{
						CurrentFrame = null;
					}
					
				}

				OnPropertyChanged("CurrentFrameIndex");
			}
		}


		private Frame currentFrame;
		public Frame CurrentFrame
		{
			get { return currentFrame; }
			set
			{
				currentFrame = value;
				OnPropertyChanged("CurrentFrame");
			}
		}

		private bool isAnimationPlaying;
		public bool IsAnimationPlaying
		{
			get { return isAnimationPlaying; }
			set
			{
				isAnimationPlaying = value;

				if (value == true)
				{
					animationTimer.Interval = 1000 / Preset.FPS;
					animationTimer.Enabled = true;
				}
				else
				{
					animationTimer.Enabled = false;
				}

				OnPropertyChanged("IsAnimationPlaying");
			}
		}

		private string presetFullPathName;
		public string PresetFullPathName
		{
			get { return presetFullPathName; }
			set
			{
				presetFullPathName = value;
				OnPropertyChanged("PresetFullPathName");
			}
		}




		public void NewPreset()
		{
			Preset = new PresetClass(FileService.LoadDefaultSettings());
			Preset.Animation = new AnimationClass();
			PresetFullPathName = "Untitled.dlp";
			CurrentFrameIndex = 0;
		}

		public void OpenPreset()
		{
			if (DialogService.ShowOpenPresetFileDialog() == true)
			{
				PresetFullPathName = DialogService.FilePath;
				FileService.OpenPreset(PresetFullPathName, Preset);
				CurrentFrameIndex = 0;
			}
		}

		public void SavePreset()
		{
			if (PresetFullPathName.Contains("\\"))
			{
				FileService.SavePreset(PresetFullPathName, Preset);
			}
			else
			{
				SavePresetAs();
			}
		}

		public void SavePresetAs()
		{
			if (DialogService.ShowSavePresetAsFileDialog() == true)
			{
				PresetFullPathName = DialogService.FilePath;
				FileService.SavePreset(PresetFullPathName, Preset);
			}
		}

		public void ShowSettings()
		{
			if (DialogService.ShowSettingsDialog(Preset) == true)
			{
				PresetClass updatedPreset = new PresetClass(DialogService.PresetSettings);
				
				if (updatedPreset.GetProgrammedLedsFrameLength() > Preset.GetProgrammedLedsFrameLength())
				{
					updatedPreset.Animation = Preset.Animation;
					if (DialogService.ShowOkCancelDialog("Количество программируемых LED-ов было изменено", "Внимание! Количество программируемых LED-ов было увеличено, будут добавлены новые LED-ы") == true)
					{
						int pixelsToAdd = updatedPreset.GetProgrammedLedsFrameLength() - Preset.GetProgrammedLedsFrameLength();
						for (int i =0; i<Preset.Animation.Frames.Count; i++)
						{
							for (int j = 0; j < pixelsToAdd; j++)
							{
								Preset.Animation.Frames[i].Pixels.Add(new System.Windows.Media.Color()
								{
									R = 0,
									G = 0,
									B = 0,
									A = 255
								});
							}
						}
					}
				}

				if (updatedPreset.GetProgrammedLedsFrameLength() < Preset.GetProgrammedLedsFrameLength())
				{
					updatedPreset.Animation = Preset.Animation;
					if (DialogService.ShowOkCancelDialog("Количество программируемых LED-ов было изменено", "Внимание! Количество программируемых LED-ов было уменьшено, лишние LED-ы будут удалены") == true)
					{
						int pixelsToRemove = Preset.GetProgrammedLedsFrameLength() - updatedPreset.GetProgrammedLedsFrameLength();
						for (int i = 0; i < Preset.Animation.Frames.Count; i++)
						{
							for (int j = 0; j < pixelsToRemove; j++)
							{
								Preset.Animation.Frames[i].Pixels.Remove(Preset.Animation.Frames[i].Pixels.Last());
							}
						}
					}
				}

				Preset = updatedPreset;
			}
		}

		public void AddFrame()
		{
			int insertIndex = CurrentFrameIndex + 1;
			if (insertIndex > Preset.Animation.Frames.Count)
				insertIndex = Preset.Animation.Frames.Count;

			Preset.Animation.Frames.Insert(insertIndex, new Frame(Preset.GetProgrammedLedsFrameLength()));
			CurrentFrameIndex++;
		}
		public void RemoveFrame()
		{
			Preset.Animation.Frames.RemoveAt(CurrentFrameIndex);
			CurrentFrameIndex = CurrentFrameIndex;
		}

		public void ImportFromGlediator()
		{
			if (DialogService.ShowOpenGlediatorFileDialog() == true)
			{
				if (DialogService.ShowImportGlediatorParamsDialog(Preset.GetProgrammedLedsFrameLength(), Preset.GetProgrammedLedsFrameLength(), 4) == true)
					Preset.Animation = FileService.ImportFromGlediatiorFile(DialogService.FilePath, Preset.GetProgrammedLedsFrameLength(), DialogService.GlediatorFileWidth, DialogService.GlediatorFileHeight);
				CurrentFrameIndex = 0;
			}
		}

		private void animationTimerUpdate(Object source, ElapsedEventArgs e)
		{
			NextFrameCommand.Execute(null);
		}


		public RelayCommand NewPresetCommand { get; }
		public RelayCommand OpenPresetCommand { get; }
		public RelayCommand SavePresetCommand { get; }
		public RelayCommand SavePresetAsCommand { get; }
		public RelayCommand ShowSettingsCommand { get; }

		public RelayCommand PreviousFrameCommand { get; }
		public RelayCommand PlayStopAnimationCommand { get; }
		public RelayCommand NextFrameCommand { get; }

		public RelayCommand AddFrameCommand { get; }
		public RelayCommand RemoveFrameCommand { get; }

		public RelayCommand ImportFromGlediatorCommand { get; }

		public MainWindowViewModel(IDialogService dialogService, IFileService fileService)
		{
			DialogService = dialogService;
			FileService = fileService;

			animationTimer = new Timer();
			animationTimer.AutoReset = true;
			animationTimer.Elapsed += animationTimerUpdate;

			CurrentFrameIndex = 0;

			IsAnimationPlaying = false;

			NewPresetCommand = new RelayCommand(_ => { NewPreset(); });
			OpenPresetCommand = new RelayCommand(_ => { OpenPreset(); });
			SavePresetCommand = new RelayCommand(_ => { SavePreset(); });
			SavePresetAsCommand = new RelayCommand(_ => { SavePresetAs(); }); 
			ShowSettingsCommand = new RelayCommand(_ => { ShowSettings(); });

			PreviousFrameCommand = new RelayCommand(_ => { CurrentFrameIndex--; }, _ => ( CurrentFrame != null));
			PlayStopAnimationCommand = new RelayCommand(_ => { IsAnimationPlaying = !IsAnimationPlaying; }, _=>(Preset.Animation.Frames.Count>0));
			NextFrameCommand = new RelayCommand(_ => { CurrentFrameIndex++; }, _ => ( CurrentFrame != null));

			AddFrameCommand = new RelayCommand(_ => { AddFrame(); });
			RemoveFrameCommand = new RelayCommand(_ => { RemoveFrame(); }, _ => (Preset.Animation.Frames.Count > 0));

			ImportFromGlediatorCommand = new RelayCommand(_ => { ImportFromGlediator(); });

			NewPreset();
		}
	}
}
