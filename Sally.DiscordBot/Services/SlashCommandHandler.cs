using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Sally.DiscordBot.Services
{
    using Sally.DiscordBot.Modules.SlashCommands;
    using Sally.ServiceDefaults.API.Logger;

    public sealed class SlashCommandHandler
    {
        private readonly DiscordSocketClient _client;

        private readonly IServiceProvider _services;

        private readonly List<SlashCommandBase> _commands;

        public SlashCommandHandler(DiscordSocketClient client, IServiceProvider services)
        {
            _client = client;
            _services = services;
            _commands = _services.GetServices<SlashCommandBase>().ToList();
        }

        public void Initialize()
        {
            _client.Ready += OnReady;
            _client.SlashCommandExecuted += HandleSlashCommandAsync;
        }

        private async Task OnReady()
        {
            var guildIds = Program.Config.YouGileConfig.ConnectionSettings.Values.Select(settings => settings.GuildId);

            foreach (var guildId in guildIds)
            {
                var guild = _client.GetGuild(guildId);

                foreach (var command in _commands)
                {
                    command.Guild = guild;

                    var builder = new SlashCommandBuilder()
                        .WithName(command.Name)
                        .WithDescription(command.Description);

                    foreach (var parameter in command.Parameters)
                    {
                        var optionBuilder = new SlashCommandOptionBuilder()
                            .WithName(parameter.Name)
                            .WithDescription(parameter.Descriprion)
                            .WithRequired(parameter.IsRequired)
                            .WithType(parameter.Type);

                        builder.AddOption(optionBuilder);
                    }

                    try
                    {
                        await guild.CreateApplicationCommandAsync(builder.Build());
                    }
                    catch (HttpException ex)
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(ex.Errors, Newtonsoft.Json.Formatting.Indented);
                        Log.Error(json);
                    }
                }
            }
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            var matchingCommand = _commands.FirstOrDefault(c => c.Name == command.Data.Name);

            if (matchingCommand is not null)
            {
                await matchingCommand.ExecuteAsync(command);
            }
        }
    }
}
