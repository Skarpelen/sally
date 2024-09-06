using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Sally.DiscordBot
{
    using Sally.DiscordBot.Modules.PublicationStorage;
    using Sally.DiscordBot.Modules.SlashCommands;
    using Sally.DiscordBot.Services;
    using Sally.DiscordBot.Services.YouGile;
    using Sally.ServiceDefaults.API.Features.RabbitMQ.Consumer;
    using Sally.ServiceDefaults.API.Logger;

    public class Program
    {
        public static Config Config => _instance._config;

        public static string DefaultSavePath { get; set; } = "/etc/sally";

        private Config _config;

        private static DiscordSocketClient _client;

        private static CommandService _commands;

        private static IServiceProvider _services;

        private static Program _instance = new();

        public static async Task Main()
        {
            var config = new Config(Path.Combine(DefaultSavePath, "BotConfig.yml"));
            _instance._config = config.Load();

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
            });

            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                CaseSensitiveCommands = false,
            });

            _services = ConfigureServices();

            await MainAsync();
        }

        private static IServiceProvider ConfigureServices()
        {
            var map = new ServiceCollection()
                .AddLogging(configure => configure.AddConsole())
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton<LoggingService>()
                .AddSingleton<SlashCommandHandler>()
                .AddSingleton<InternalEventService>()
                .AddSingleton<YouGileService>()
                .AddSingleton<PublicationStorageHandler>();

            var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.IsSubclassOf(typeof(SlashCommandBase)) && !t.IsAbstract);

            foreach (var commandType in commandTypes)
            {
                map.AddSingleton(typeof(SlashCommandBase), commandType);
            }

            return map.BuildServiceProvider();
        }

        private static async Task MainAsync()
        {
            await InitCommands();

            var loggingService = _services.GetRequiredService<LoggingService>();
            loggingService.Initialize();

            var slashCommandHandler = _services.GetRequiredService<SlashCommandHandler>();
            slashCommandHandler.Initialize();

            var yougileService = _services.GetRequiredService<YouGileService>();

            try
            {
                await yougileService.InitializeAsync();
            }
            catch (Exception ex)
            {
                Log.Error($"Ошибка при чтении публикаций: {ex}");
            }

            var eventService = _services.GetRequiredService<InternalEventService>();
            eventService.Initialize();

            var consumerTask = Task.Run(() =>
            {
                var consumer = new Consumer();
                consumer.Consume();
            });

            await _client.LoginAsync(TokenType.Bot, Config.DiscordBotToken);
            await _client.StartAsync();

            await Task.WhenAny(consumerTask, Task.Delay(Timeout.Infinite));
        }

        private static async Task InitCommands()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
            _client.MessageReceived += HandleCommandAsync;
        }

        private static async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;

            if (msg is null)
            {
                return;
            }

            if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot)
            {
                return;
            }

            var pos = 0;
           
            if (msg.HasCharPrefix('!', ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos))
            {
                var context = new SocketCommandContext(_client, msg);

                var result = await _commands.ExecuteAsync(context, pos, _services);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }
}
