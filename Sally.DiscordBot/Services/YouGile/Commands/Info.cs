using Discord.WebSocket;
using System.Text;

namespace Sally.DiscordBot.Services.YouGile.Commands
{
    using Sally.DiscordBot.Modules.SlashCommands;

    public sealed class Info : SlashCommandBase
    {
        public override string Name => "info";

        public override string Description => "Возвращает информацию о текущих привязанных проектах";

        public override async Task ExecuteAsync(SocketSlashCommand command, Dictionary<string, SocketSlashCommandDataOption> arguments)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Аккаунт, с которого идет обращение к YouGile: {Program.Config.YouGileConfig.LogInEmail}");
            sb.AppendLine();

            foreach (var settings in Program.Config.YouGileConfig.ConnectionSettings)
            {
                sb.AppendLine($"Проект: {settings.Key}");
                sb.AppendLine($"Канал для логов: <#{settings.Value.TaskLogChannelId}>");
                sb.AppendLine();
            }

            sb.AppendLine("Данные взяты с BotConfig.yml. Для более точечного изменения, перейдите в /etc/sally/BotConfig.yml и внесите изменения");

            await command.RespondAsync(sb.ToString());
        }
    }
}
