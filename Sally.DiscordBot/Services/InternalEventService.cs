using Discord.WebSocket;

namespace Sally.DiscordBot.Services
{
    using Sally.DiscordBot.Modules.PublicationStorage;
    using Sally.DiscordBot.Services.YouGile;

    public sealed class InternalEventService
    {
        private readonly DiscordSocketClient _client;

        private readonly IServiceProvider _services;

        private readonly YouGileService _yougileService;

        private readonly PublicationStorageHandler _publicationStorageHandler;

        public InternalEventService(DiscordSocketClient client, IServiceProvider services, YouGileService youGileService, PublicationStorageHandler publicationStorageHandler)
        {
            _client = client;
            _services = services;
            _yougileService = youGileService;
            _publicationStorageHandler = publicationStorageHandler;
        }

        public void Initialize()
        {
            _client.GuildAvailable += SaveChannelAsyncOnGuildAvailable;
        }

        /// <summary>
        /// Ищет и сохраняет каналы-форумы из конфигов
        /// </summary>
        /// <param name="guild">Дискорд-сообщество</param>
        private async Task SaveChannelAsyncOnGuildAvailable(SocketGuild guild)
        {
            await _yougileService.SaveChannelsForGuid(guild);
        }
    }
}
