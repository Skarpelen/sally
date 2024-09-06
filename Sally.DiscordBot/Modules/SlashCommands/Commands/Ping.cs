using Discord.WebSocket;

namespace Sally.DiscordBot.Modules.SlashCommands.Commands
{
    public sealed class Ping : SlashCommandBase
    {
        public override string Name => "ping";

        public override string Description => "pong";

        public override async Task ExecuteAsync(SocketSlashCommand command, Dictionary<string, SocketSlashCommandDataOption> arguments)
        {
            await command.RespondAsync($"pong, {command.User.Username}");
        }
    }
}
