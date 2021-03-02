using System;
using MHLab.Patch.Core.Client.Progresses;
using UnityEngine.Events;

namespace MHLab.Patch.Utilities
{
    public sealed class ProgressReporterEvent : UnityEvent<UpdateProgress>
    {
        
    }
    
    public sealed class ProgressReporter : IProgress<UpdateProgress>
    {
        public ProgressReporterEvent ProgressChanged; 
        
        public ProgressReporter()
        {
            ProgressChanged = new ProgressReporterEvent();
        }

        private void OnProgressChanged(UpdateProgress progress)
        {
            var handler = ProgressChanged;

            if (handler != null)
            {
                try
                {
                    handler.Invoke(progress);
                }
                catch
                {}
            }
        }
        
        public void Report(UpdateProgress value)
        {
            OnProgressChanged(value);
        }
    }
}