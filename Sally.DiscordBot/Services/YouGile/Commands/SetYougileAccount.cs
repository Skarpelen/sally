using Discord;
using Discord.WebSocket;

namespace Sally.DiscordBot.Services.YouGile.Commands
{
    using Sally.DiscordBot.Modules.SlashCommands;
    using Sally.DiscordBot.Modules.SlashCommands.Structs;

    public sealed class SetYougileAccount : SlashCommandBase
    {
        public override string Name => "set-yougile-account";

        public override string Description => "Устанавливает логин и пароль от YouGile для бота";

        public override CommandParameter[] Parameters { get; } =
        {
            new()
            {
                Name = "login",
                Descriprion = "Название проекта",
                IsRequired = true,
                Type = ApplicationCommandOptionType.String
            },
            new()
            {
                Name = "password",
                Descriprion = "Канал для логов",
                IsRequired = true,
                Type = ApplicationCommandOptionType.String
            }
        };

        public override async Task ExecuteAsync(SocketSlashCommand command, Dictionary<string, SocketSlashCommandDataOption> arguments)
        {
            var login = arguments["login"].Value.ToString()!;
            var password = arguments["password"].Value.ToString()!;

            Program.Config.YouGileConfig.LogInEmail = login;
            Program.Config.YouGileConfig.LogInPassword = password;

            Program.Config.Update();

            await command.RespondAsync("Логин и пароль для аккаунта установлены. Используйте команду `/detect` для получения списка проектов");
        }
    }
}
