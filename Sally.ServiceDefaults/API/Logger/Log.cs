using System.Reflection;

namespace Sally.ServiceDefaults.API.Logger
{
    public static class Log
    {
        private const int TimestampLength = 23; // длина строки времени формата "yyyy-MM-dd HH:mm:ss.fff"

        /// <summary>
        /// Информационное сообщение в логи
        /// </summary>
        /// <param name="message">Сообщение</param>
        public static void Info(object message)
        {
            Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Info, ConsoleColor.Cyan);
        }

        /// <summary>
        /// Информационное сообщение в логи
        /// </summary>
        /// <param name="message">Сообщение</param>
        public static void Info(string message)
        {
            Send("[" + Assembly.GetCallingAssembly().GetName().Name + "] " + message, LogLevel.Info, ConsoleColor.Cyan);
        }

        /// <summary>
        /// Предупреждающее сообщение в логи
        /// </summary>
        /// <param name="message">Сообщение</param>
        public static void Warning(object message)
        {
            Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Warn, ConsoleColor.Magenta);
        }

        /// <summary>
        /// Предупреждающее сообщение в логи
        /// </summary>
        /// <param name="message">Сообщение</param>
        public static void Warning(string message)
        {
            Send("[" + Assembly.GetCallingAssembly().GetName().Name + "] " + message, LogLevel.Warn, ConsoleColor.Magenta);
        }

        /// <summary>
        /// Сообщение об ошибке в логи
        /// </summary>
        /// <param name="message">Сообщение</param>
        public static void Error(object message)
        {
            Send($"[{Assembly.GetCallingAssembly().GetName().Name}] {message}", LogLevel.Error, ConsoleColor.DarkRed);
        }

        /// <summary>
        /// Сообщение об ошибке в логи
        /// </summary>
        /// <param name="message">Сообщение</param>
        public static void Error(string message)
        {
            Send("[" + Assembly.GetCallingAssembly().GetName().Name + "] " + message, LogLevel.Error, ConsoleColor.DarkRed);
        }

        /// <summary>
        /// Сообщение trace в логи
        /// </summary>
        /// <param name="message">Сообщение</param>
        public static void Trace(object message)
        {
            //_logger?.LogTrace(message.ToString());
        }

        private static void Send(object message, LogLevel level, ConsoleColor color = ConsoleColor.Gray)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var formattedMessage = $"[{timestamp}] [{level.ToString().ToUpper()}] {message}";
            SendRaw(formattedMessage, color);
        }

        private static void Send(string message, LogLevel level, ConsoleColor color = ConsoleColor.Gray)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var formattedMessage = "[" + timestamp + "] [" + level.ToString().ToUpper() + "] " + message;
            SendRaw(formattedMessage, color);
        }

        private static void SendRaw(object message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message.ToString(), color);
            Console.ResetColor();
        }

        private static void SendRaw(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message, color);
            Console.ResetColor();
        }
    }
}
