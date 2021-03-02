using System;
using UnityEngine;

namespace MHLab.Patch.Utilities
{
    public static class DebugHelper
    {
        public static string GetSystemInfo()
        {
            string info = "OS: " + SystemInfo.operatingSystem + " - Family: " + SystemInfo.operatingSystemFamily + "\n";
            info += "Processor: " + SystemInfo.processorType + " " + SystemInfo.processorFrequency + "MHz (" +
                    SystemInfo.processorCount + " vcore)\n";
            info += "Device: " + SystemInfo.deviceModel + " - Type: " + SystemInfo.deviceType + "\n";
            info += "System Memory: " + SystemInfo.systemMemorySize + "MB\n";
            info += "GPU: " + SystemInfo.graphicsDeviceName + " - Vendor: " + SystemInfo.graphicsDeviceVendor + " - Type: " +
                    SystemInfo.graphicsDeviceType + " - Version: " + SystemInfo.graphicsDeviceVersion + " - Memory: " +
                    SystemInfo.graphicsMemorySize + "MB - Shader: " + SystemInfo.graphicsShaderLevel + "\n";
            info += "Process: " + (Environment.Is64BitProcess ? 64 : 32) + " bits - CLR Version: " + Environment.Version + "\n";
            info += "Commandline arguments: " + Environment.CommandLine;

            return info;
        }
    }
}