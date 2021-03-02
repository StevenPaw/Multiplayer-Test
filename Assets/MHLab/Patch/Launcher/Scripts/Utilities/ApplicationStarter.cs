using System.Diagnostics;
using System.IO;

namespace MHLab.Patch.Launcher.Scripts.Utilities
{
    public static class ApplicationStarter
    {
        public static void StartApplication(string filePath, string arguments)
        {
#if UNITY_STANDALONE_OSX
            PrepareApplicationMac(filePath, arguments).Start();
#elif UNITY_STANDALONE_LINUX
            PrepareApplicationLinux(filePath, arguments).Start();
#else
            PrepareStartApplicationWindows(filePath, arguments).Start();
#endif           
        }

        private static Process PrepareStartApplicationWindows(string filePath, string arguments)
        {
            var process = new Process();
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Verb = "runas";
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePath);

            return process;
        }

        private static Process PrepareApplicationMac(string filePath, string arguments)
        {
            var process = new Process();
            process.StartInfo.FileName = "open";
            process.StartInfo.Arguments = $"-a '{filePath}' -n --args '{arguments}'";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePath);

            return process;
        }

        private static Process PrepareApplicationLinux(string filePath, string arguments)
        {
            var process = new Process();
            process.StartInfo.FileName = filePath;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = Path.GetDirectoryName(filePath);

            return process;
        }
    }
}