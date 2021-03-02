using System;
using System.IO;
using MHLab.Patch.Core;
using MHLab.Patch.Core.Client;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Launcher.Scripts.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MHLab.Patch.Launcher.Scripts
{
    public sealed class LauncherUpdater : LauncherBase
    {
        public int SceneToLoad;
        
        private PatcherUpdater _patcherUpdater;
        
        protected override void Initialize(UpdatingContext context)
        {
            context.OverrideSettings<SettingsOverride>((originalSettings, settingsOverride) =>
            {
                originalSettings.DebugMode              = settingsOverride.DebugMode;
                originalSettings.PatcherUpdaterSafeMode = settingsOverride.PatcherUpdaterSafeMode;
            });
            
            NetworkChecker = new NetworkChecker();
            
            _patcherUpdater = new PatcherUpdater(context);
            _patcherUpdater.Downloader.DownloadComplete += Data.DownloadComplete;
            
            context.RegisterUpdateStep(_patcherUpdater);

            context.Runner.PerformedStep += (sender, updater) =>
            {
                if (context.IsDirty(out var reasons, out var data))
                {
                    var stringReasons = "";

                    foreach (var reason in reasons)
                    {
                        stringReasons += $"{reason}, ";
                    }

                    stringReasons = stringReasons.Substring(0, stringReasons.Length - 2);
                    
                    context.Logger.Debug($"Context is set to dirty: updater restart required. The files {stringReasons} have been replaced.");
                    
                    if (data.Count > 0)
                    {
                        if (data[0] is UpdaterSafeModeDefinition)
                        {
                            var definition = (UpdaterSafeModeDefinition) data[0];
                            UpdateRestartNeeded(definition.ExecutableToRun);
                            return;
                        }
                    }
                    
                    UpdateRestartNeeded();
                }
            };
        }

        protected override string UpdateProcessName => "Launcher updating";

        protected override void OverrideSettings(ILauncherSettings settings)
        {
            string rootPath = string.Empty;
            
#if UNITY_EDITOR
            rootPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), LauncherData.WorkspaceFolderName, "TestLauncher");
            Directory.CreateDirectory(rootPath);
#elif UNITY_STANDALONE_WIN
            rootPath = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
#elif UNITY_STANDALONE_LINUX
            rootPath = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
#elif UNITY_STANDALONE_OSX
            rootPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName).FullName;
#endif
            
            settings.RootPath = FilesManager.SanitizePath(rootPath);
        }
        
        protected override void UpdateStarted()
        {
            Data.StartTimer(UpdateDownloadSpeed);
        }
        
        protected override void UpdateDownloadSpeed()
        {
            _patcherUpdater.Downloader.DownloadSpeedMeter.Tick();
            
            if (_patcherUpdater.Downloader.DownloadSpeedMeter.DownloadSpeed > 0)
            {
                Data.DownloadSpeed.text = _patcherUpdater.Downloader.DownloadSpeedMeter.FormattedDownloadSpeed;
            }
            else
            {
                Data.DownloadSpeed.text = string.Empty;
            }
        }
        
        protected override void UpdateCompleted()
        {
            Data.Dispatcher.Invoke(() =>
            {
                Data.ProgressBar.Progress = 1;
                Data.ProgressPercentage.text = "100%";
            });

            var repairer = new Repairer(Context);
            var updater = new Updater(Context);

            if (repairer.IsRepairNeeded() || updater.IsUpdateAvailable())
            {
                UpdateRestartNeeded();
                return;
            }
            
            Data.Log(Context.LocalizedMessages.UpdateProcessCompleted);
            Context.Logger.Info($"===> [{UpdateProcessName}] process COMPLETED! <===");
            StartGameScene();
        }

        protected override void StartApp()
        {
            StartGameScene();
        }

        private void StartGameScene()
        {
            Data.Dispatcher.Invoke(() => 
            {
                SceneManager.LoadScene(SceneToLoad);
            });
        }

        protected override void UpdateFailed(Exception e)
        {
            Data.Log(Context.LocalizedMessages.UpdateProcessFailed);
            Context.Logger.Error(e, $"===> [{UpdateProcessName}] process FAILED! <=== - {e.Message} - {e.StackTrace}");

            if (Data.LaunchAnywayOnError)
            {
                StartGameScene();
            }
        }

        protected override void UpdateRestartNeeded(string executableName = "")
        {
            Data.Log(Context.LocalizedMessages.UpdateRestartNeeded);
            Context.Logger.Info($"===> [{UpdateProcessName}] process INCOMPLETE: restart is needed! <===");
            
            EnsureExecutePrivileges(PathsManager.Combine(Context.Settings.RootPath, Data.LauncherExecutableName));
            
            string filePath;

            if (!string.IsNullOrWhiteSpace(executableName))
            {
                filePath = PathsManager.Combine(Context.Settings.RootPath, executableName);
            }
            else
            {
                filePath = PathsManager.Combine(Context.Settings.RootPath, Data.LauncherExecutableName);
            }

            try
            {
                ApplicationStarter.StartApplication(
                    Path.Combine(Context.Settings.RootPath, Data.LauncherExecutableName), "");

                Data.Dispatcher.Invoke(Application.Quit);
            }
            catch (Exception ex)
            {
                Context.Logger.Error(null, $"Unable to start the Launcher at {filePath}.");
                UpdateFailed(ex);
            }
        }

        public void GenerateDebugReport()
        {
            GenerateDebugReport("debug_report_pregame.txt");
        }
        
        private void OnDisable()
        {
            _patcherUpdater.Downloader.Cancel();            
        }
    }
}