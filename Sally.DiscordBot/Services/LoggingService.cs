using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Sally.DiscordBot.Services
{
    using Sally.ServiceDefaults.API.Logger;

    public sealed class LoggingService
    {
        private readonly DiscordSocketClient _client;

        private readonly CommandService _command;

        public LoggingService(DiscordSocketClient client, CommandService command)
        {
            _client = client;
            _command = command;
        }

        public void Initialize()
        {
            _client.Log += LogAsync;
            _command.Log += LogAsync;
        }

        private Task LogAsync(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Log.Error(message.Message);
                    break;
                case LogSeverity.Warning:
                    Log.Warning(message.Message);
                    break;
                case LogSeverity.Info:
                    Log.Info(message.Message);
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Log.Info(message.Message);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
