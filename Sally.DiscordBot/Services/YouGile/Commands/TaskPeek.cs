using Discord;
using Discord.WebSocket;

namespace Sally.DiscordBot.Services.YouGile.Commands
{
    using Sally.DiscordBot.Modules.SlashCommands;
    using Sally.DiscordBot.Modules.SlashCommands.Structs;
    using Sally.DiscordBot.Services.YouGile;

    public sealed class TaskPeek : SlashCommandBase
    {
        public override string Name => "task-peek";

        public override string Description => "Выводит информацию о задаче";

        public override CommandParameter[] Parameters { get; } =
        {
            new()
            {
                Name = "task-id",
                Descriprion = "Id карточки",
                IsRequired = true,
                Type = ApplicationCommandOptionType.Integer
            }
        };

        public override async Task ExecuteAsync(SocketSlashCommand command, Dictionary<string, SocketSlashCommandDataOption> arguments)
        {
            if (!arguments.TryGetValue("task-id", out var option) || !int.TryParse(option.Value.ToString(), out var taskId))
            {
                await command.RespondAsync("Please provide a task ID.");
                return;
            }

            var taskInfo = _youGileService.GetTaskInfo(taskId);

            if (taskInfo is null)
            {
                await command.RespondAsync($"Таск с ID DEV-{taskId} не найден.");
                return;
            }

            await command.RespondAsync(embed: taskInfo.GetEmbededInfo());
        }

        private readonly YouGileService _youGileService;

        public TaskPeek(YouGileService youGileService)
        {
            _youGileService = youGileService;
        }
    }
}
