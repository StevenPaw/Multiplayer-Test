using System;
using System.IO;
using System.Threading;
using MHLab.Patch.Core.Client;
using MHLab.Patch.Core.Client.IO;
using MHLab.Patch.Core.Client.Progresses;
using MHLab.Patch.Core.Utilities;
using MHLab.Patch.Launcher.Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MHLab.Patch.Launcher.Scripts
{
    public sealed class LauncherData : MonoBehaviour
    {
        public string RemoteUrl;
        public string LauncherExecutableName;
        public string GameExecutableName;

        public bool LaunchAnywayOnError;
        
        public Dispatcher Dispatcher;
        public ProgressBar ProgressBar;
        public Text DownloadSpeed;
        public Text ProgressPercentage;
        public Text Logs;
        public Text ElapsedTime;
        public Dialog Dialog;
        public Text SizeProgress;
        
        public const string WorkspaceFolderName = "PATCHWorkspace";
        
        private Timer _timer;
        private int _elapsed;

        public void DownloadComplete(object sender, EventArgs e)
        {
            
        }

        public void UpdateProgressChanged(UpdateProgress e)
        {
            Dispatcher.Invoke(() =>
            {
                var totalSteps = Math.Max(e.TotalSteps, 1);
                ProgressBar.Progress = (float) e.CurrentSteps / totalSteps;

                ProgressPercentage.text = (e.CurrentSteps * 100 / totalSteps) + "%";

                SizeProgress.text = FormatUtility.FormatSizeDecimal(e.CurrentSteps, 2) + "/" +
                                    FormatUtility.FormatSizeDecimal(e.TotalSteps, 2);
            });
            
            Log(e.StepMessage);
        }
        
        public void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                Logs.text = message;
            });
        }

        public void ResetComponents()
        {
            ProgressPercentage.text = string.Empty;
            DownloadSpeed.text = string.Empty;
            ElapsedTime.text = string.Empty;
            Logs.text = string.Empty;

            ProgressBar.Progress = 0;
        }

        public void StartTimer(Action updateDownloadSpeed)
        {
            _timer = new Timer((state) =>
            {
                _elapsed++;
                Dispatcher.Invoke(() =>
                {
                    var minutes = _elapsed / 60;
                    var seconds = _elapsed % 60;

                    ElapsedTime.text = string.Format("{0}:{1}", (minutes < 10) ? "0" + minutes : minutes.ToString(), (seconds < 10) ? "0" + seconds : seconds.ToString());
                    
                    updateDownloadSpeed.Invoke();
                });
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public void StopTimer()
        {
            _timer.Dispose();
        }
    }
}