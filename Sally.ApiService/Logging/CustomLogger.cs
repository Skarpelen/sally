namespace Sally.ApiService.Logging
{
    using CustomLog = Sally.ServiceDefaults.API.Logger.Log;

    public class CustomLogger : ILogger
    {
        private readonly string _name;

        public CustomLogger(string name)
        {
            _name = name;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return default!;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel is not LogLevel.Trace;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var logEntry = formatter(state, exception);
            var message = $"[{_name}] {logEntry}";

            switch (logLevel)
            {
                case LogLevel.Trace:
                    break;
                case LogLevel.Debug:
                    break;
                case LogLevel.Information:
                    CustomLog.Info(message);
                    break;
                case LogLevel.Warning:
                    CustomLog.Warning(message);
                    break;
                case LogLevel.Error:
                case LogLevel.Critical:
                    CustomLog.Error(message);
                    break;
                case LogLevel.None:
                    break;
            }
        }
    }
}
