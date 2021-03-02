using Serilog;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using MHLab.Patch.Core.IO;
using ILogger = MHLab.Patch.Core.Logging.ILogger;

namespace MHLab.Patch.Utilities.Logging
{
    public sealed class Logger : ILogger
    {
        private readonly Serilog.Core.Logger _logger;
        
        private bool _isDebug;
        
        public Logger(string logfilePath, bool isDebug)
        {
            _isDebug = isDebug;
            
            var configuration = new LoggerConfiguration();
            if (isDebug)    
                configuration.MinimumLevel.Debug();
            else
                configuration.MinimumLevel.Information();
            
            configuration.WriteTo.File(logfilePath, rollingInterval: RollingInterval.Day, shared: true);
            
            _logger = configuration.CreateLogger();
        }
        
        public void Debug(string messageTemplate, [CallerFilePath] string callerPath = "", [CallerLineNumber] long callerLine = 0, [CallerMemberName] string callerMember = "")
        {
            _logger.Debug(BuildMessage(messageTemplate, callerPath, callerLine, callerMember));
        }

        public void Info(string messageTemplate, [CallerFilePath] string callerPath = "", [CallerLineNumber] long callerLine = 0, [CallerMemberName] string callerMember = "")
        {
            _logger.Information(BuildMessage(messageTemplate, callerPath, callerLine, callerMember));
        }

        public void Warning(string messageTemplate, [CallerFilePath] string callerPath = "", [CallerLineNumber] long callerLine = 0, [CallerMemberName] string callerMember = "")
        {
            _logger.Warning(BuildMessage(messageTemplate, callerPath, callerLine, callerMember));
        }

        public void Error(Exception exception, string messageTemplate, [CallerFilePath] string callerPath = "", [CallerLineNumber] long callerLine = 0, [CallerMemberName] string callerMember = "")
        {
            _logger.Error(exception, BuildMessage(messageTemplate, callerPath, callerLine, callerMember));
        }

        private string BuildMessage(string messageTemplate, string callerPath, long callerLine, string callerMember)
        {
            if (_isDebug)
            {
                var caller = Path.GetFileName(callerPath);
                messageTemplate = $"[{caller}::{callerMember}@{callerLine}] {messageTemplate}";
            }
            return messageTemplate;
        }
    }
}
