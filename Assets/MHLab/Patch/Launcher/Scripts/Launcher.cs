using System;
using System.IO;
using MHLab.Patch.Core;
using MHLab.Patch.Core.Client;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.IO;
using MHLab.Patch.Launcher.Scripts.Utilities;
using UnityEngine;

namespace MHLab.Patch.Launcher.Scripts
{
    public sealed class Launcher : LauncherBase
    {
        private Repairer _repairer;
        private Updater _updater;
        
        protected override void Initialize(UpdatingContext context)
        {
            context.OverrideSettings<SettingsOverride>((originalSettings, settingsOverride) =>
            {
                originalSettings.DebugMode              = settingsOverride.DebugMode;
                originalSettings.PatcherUpdaterSafeMode = settingsOverride.PatcherUpdaterSafeMode;
            });
            
            NetworkChecker = new NetworkChecker();
            
            _repairer = new Repairer(context);
            _repairer.Downloader.DownloadComplete += Data.DownloadComplete;

            _updater = new Updater(context);
            _updater.Downloader.DownloadComplete += Data.DownloadComplete;
            
            context.RegisterUpdateStep(_repairer);
            context.RegisterUpdateStep(_updater);

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

        protected override string UpdateProcessName => "Game updating";

        protected override void OverrideSettings(ILauncherSettings settings)
        {
            string rootPath = string.Empty;
            
#if UNITY_EDITOR
            rootPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), LauncherData.WorkspaceFolderName, "TestLauncher");
            Directory.CreateDirectory(rootPath);
#elif UNITY_STANDALONE_WIN
            rootPath = Directory.GetParent(Application.dataPath).FullName;
#elif UNITY_STANDALONE_LINUX
            rootPath = Directory.GetParent(Application.dataPath).FullName;
#elif UNITY_STANDALONE_OSX
            rootPath = Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName;
#endif
            
            settings.RootPath = FilesManager.SanitizePath(rootPath);
        }
        
        protected override void UpdateStarted()
        {
            Data.StartTimer(UpdateDownloadSpeed);
        }

        protected override void UpdateDownloadSpeed()
        {
            _repairer.Downloader.DownloadSpeedMeter.Tick();
            _updater.Downloader.DownloadSpeedMeter.Tick();
            
            if (_repairer.Downloader.DownloadSpeedMeter.DownloadSpeed > 0)
            {
                Data.DownloadSpeed.text = _repairer.Downloader.DownloadSpeedMeter.FormattedDownloadSpeed;
            }
            else if (_updater.Downloader.DownloadSpeedMeter.DownloadSpeed > 0)
            {
                Data.DownloadSpeed.text = _updater.Downloader.DownloadSpeedMeter.FormattedDownloadSpeed;
            }
            else
            {
                Data.DownloadSpeed.text = string.Empty;
            }
        }

        protected override void UpdateCompleted()
        {
            Data.Log(Context.LocalizedMessages.UpdateProcessCompleted);
            Context.Logger.Info($"===> [{UpdateProcessName}] process COMPLETED! <===");
            
            Data.Dispatcher.Invoke(() =>
            {
                Data.ProgressBar.Progress = 1;
                Data.ProgressPercentage.text = "100%";
            });
            
            EnsureExecutePrivileges(PathsManager.Combine(Context.Settings.GetGamePath(), Data.GameExecutableName));
            EnsureExecutePrivileges(PathsManager.Combine(Context.Settings.RootPath, Data.LauncherExecutableName));
            
            Data.Dispatcher.Invoke(() =>
            {
                Invoke(nameof(StartGame), 1.5f);
            });
        }

        protected override void UpdateFailed(Exception e)
        {
            Data.Log(Context.LocalizedMessages.UpdateProcessFailed);
            Context.Logger.Error(e, $"===> [{UpdateProcessName}] process FAILED! <=== - {e.Message} - {e.StackTrace}");

            if (Data.LaunchAnywayOnError)
            {
                StartGame();
            }
        }

        protected override void UpdateRestartNeeded(string executableName = "")
        {
            Data.Log(Context.LocalizedMessages.UpdateRestartNeeded);
            Context.Logger.Info($"===> [{UpdateProcessName}] process INCOMPLETE: restart is needed! <===");
            
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
        
        protected override void StartApp()
        {
            StartGame();
        }

        private void StartGame()
        {
            var filePath = PathsManager.Combine(Context.Settings.GetGamePath(), Data.GameExecutableName);
            ApplicationStarter.StartApplication(filePath, $"{Context.Settings.LaunchArgumentParameter}={Context.Settings.LaunchArgumentValue}");
            Application.Quit();
        }

        public void GenerateDebugReport()
        {
            GenerateDebugReport("debug_report_launcher.txt");
        }
        
        private void OnDestroy()
        {
            _repairer.Downloader.Cancel();
            Debug.Log("Download canceled");
        }
    }
}