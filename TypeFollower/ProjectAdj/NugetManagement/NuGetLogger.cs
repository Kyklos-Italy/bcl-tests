using NuGet;

namespace ProjectAdj.NugetManagement
{
    public class NuGetLogger : ILogger
    {
        Common.Logging.ILog __logger;

        public void LogDebug(string data) => __logger.Debug(data);
        public void LogVerbose(string data) => __logger.Debug(data);
        public void LogInformation(string data) => __logger.Info(data);
        public void LogMinimal(string data) => __logger.Debug(data);
        public void LogWarning(string data) => __logger.Warn(data);
        public void LogError(string data) => __logger.Error(data);
        public void LogSummary(string data) => __logger.Debug(data);
        public void LogInformationSummary(string data)
        {
            __logger.Info(data);
        }

        public void LogErrorSummary(string data)
        {
            __logger.Error(data);
        }

		public void Log(MessageLevel level, string message, params object[] args)
		{
			__logger.Debug(message);
		}

		public FileConflictResolution ResolveFileConflict(string message)
		{
			__logger.Debug(message);
			return FileConflictResolution.Ignore;
		}

		//public void Log(LogLevel level, string data)
		//{
		//	__logger.Info(data);
		//}

		//public Task LogAsync(LogLevel level, string data)
		//{
		//	return Task.Run(() => __logger.Info(data));
		//}

		//public void Log(ILogMessage message)
		//{
		//	__logger.Info(message);
		//}

		//public Task LogAsync(ILogMessage message)
		//{
		//	return Task.Run(() => __logger.Info(message));
		//}

		public NuGetLogger(Common.Logging.ILog logger)
        {
            __logger = logger;
        }
    }
}
