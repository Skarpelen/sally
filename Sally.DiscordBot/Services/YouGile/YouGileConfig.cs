using YamlDotNet.Serialization;

namespace Sally.DiscordBot.Services.YouGile
{
    using Sally.DiscordBot.Services.YouGile.Structs;

    public sealed class YouGileConfig
    {
        [YamlMember(Description = "Почта аккаунта, от чьего лица бот будет получать доступ к доскам")]
        public string LogInEmail { get; set; } = "email@email.org";

        [YamlMember(Description = "Пароль аккаунта")]
        public string LogInPassword { get; set; } = "pass";

        [YamlMember(Description = "Id компании, в которой бот будет работать")]
        public string CompanyId { get; set; } = "00000000-1111-4444-3333-666666666666";

        [YamlMember(Description = "Настройки интеграции YouGile с дискордом. Ключ - Название проекта")]
        public Dictionary<string, YouGileDiscordConnectionSettings> ConnectionSettings { get; set; } = new()
        {
            {
                "Project1",
                new YouGileDiscordConnectionSettings()
                {
                    GuildId = 21,
                    ForumChannelId = 1250432616902033545,
                    TaskLogChannelId = 1258763739092750356,
                    MinTaskValue = 424,
                    MinTaskTimestamp = 1714562721338,
                    ActiveColumn = "В очереди 🐢",
                }
            },
            {
                "Project2",
                new YouGileDiscordConnectionSettings()
                {
                    GuildId = 12,
                    ForumChannelId = 1258013303079440514,
                    TaskLogChannelId = 1258763773255090238,
                    IgnoreColumn = "Готово 🎉,Черновики ✏️"
                }
            }
        };

        [YamlMember(Description = "Внешний ip адрес и порт для получения событий с YouGile(в конце должно стоять /)")]
        public string ExternalUrl { get; set; } = "http://200.1.12.7:9002/";
    }
}
