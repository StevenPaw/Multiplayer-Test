using System;

namespace MHLab.Patch.Utilities
{
    public static class ArgumentChecker
    {
        public static bool IsLaunchedWithCorrectParameter(string parameter, string expectedValue)
        {
            var args = Environment.GetCommandLineArgs();

            foreach (var arg in args)
            {
                if (!arg.StartsWith(parameter)) continue;
                
                var retrievedValue = arg.Replace(parameter + "=", "");

                return retrievedValue == expectedValue;
            }

            return false;
        }
    }
}