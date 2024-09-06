using Discord;
using Discord.WebSocket;

namespace Sally.DiscordBot.Services.YouGile.Commands
{
    using Sally.DiscordBot.Modules.SlashCommands;
    using Sally.DiscordBot.Modules.SlashCommands.Structs;

    public sealed class SetLogChannel : SlashCommandBase
    {
        public override string Name => "set-log-channel";

        public override string Description => "Устанавливает выбранный канал как канал для логов с YouGile";

        public override CommandParameter[] Parameters { get; } =
        {
            new()
            {
                Name = "project",
                Descriprion = "Название проекта",
                IsRequired = true,
                Type = ApplicationCommandOptionType.String
            },
            new()
            {
                Name = "channel",
                Descriprion = "Канал для логов",
                IsRequired = true,
                Type = ApplicationCommandOptionType.Channel
            }
        };

        public override async Task ExecuteAsync(SocketSlashCommand command, Dictionary<string, SocketSlashCommandDataOption> arguments)
        {
            var projectName = arguments["project"].Value.ToString()!;

            if (!Program.Config.YouGileConfig.ConnectionSettings.TryGetValue(projectName, out var projectSettings))
            {
                await command.RespondAsync("Проект с таким названием не найден. Проверьте корректное название через команду `/info`.");
                return;
            }

            if (!arguments.TryGetValue("channel", out var channel) || channel.Value is not SocketGuildChannel logChannel || logChannel.GetChannelType() is not ChannelType.Text)
            {
                await command.RespondAsync("Указанный канал не может использоваться для логов.");
                return;
            }

            projectSettings.TaskLogChannelId = logChannel.Id;

            Program.Config.YouGileConfig.ConnectionSettings[projectName] = projectSettings;
            Program.Config.Update();

            await command.RespondAsync($"Канал <#{logChannel.Id}> теперь будет использоваться для логов");
        }
    }
}
