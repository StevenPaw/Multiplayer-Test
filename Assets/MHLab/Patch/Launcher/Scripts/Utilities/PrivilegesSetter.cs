using System.Diagnostics;
using MHLab.Patch.Core.IO;

namespace MHLab.Patch.Launcher.Scripts.Utilities
{
    public static class PrivilegesSetter
    {
        public static void EnsureExecutePrivileges(string filePath)
        {
#if UNITY_STANDALONE_OSX
            EnsurePrivilegesMac(filePath);
#elif UNITY_STANDALONE_LINUX
            EnsurePrivilegesLinux(filePath);
#else
            EnsurePrivilegesWindows(filePath);
#endif
        }
        
        private static void EnsurePrivilegesWindows(string filePath)
        {
            var processChmod = new Process();
            processChmod.StartInfo.FileName = "ICACLS";
            processChmod.StartInfo.Arguments = "\"" + filePath + "\" /grant \"Users\":M";
            processChmod.Start();
        }

        private static void EnsurePrivilegesMac(string filePath)
        {
            var filename = filePath + "/Contents/MacOS/" + PathsManager.GetFilename(filePath).Replace(".app", "");
            
            var processChmod = new Process();
            processChmod.StartInfo.FileName = "chmod";
            processChmod.StartInfo.Arguments = "+x \"" + filename + "\"";
            processChmod.Start();
		    
            var processAttr = new Process();
            processAttr.StartInfo.FileName = "xattr";
            processAttr.StartInfo.Arguments = "-d com.apple.quarantine \"" + filePath + "\"";
            processAttr.Start();
        }

        private static void EnsurePrivilegesLinux(string filePath)
        {
            var processChmod = new Process();
            processChmod.StartInfo.FileName = "chmod";
            processChmod.StartInfo.Arguments = "+x \"" + filePath + "\"";
            processChmod.Start();
        }
    }
}