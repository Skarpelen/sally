using YamlDotNet.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Structs
{
    /// <summary>
    /// Настройки интеграции YouGile и дискорда
    /// </summary>
    public sealed class YouGileDiscordConnectionSettings
    {
        [YamlMember(Description = "Id дискорд сервера, на который будут идти сообщения")]
        public ulong GuildId { get; set; }

        [YamlMember(Description = "Id канала-форума сообщества, где будут храниться задачи")]
        public ulong ForumChannelId { get; set; }

        [YamlMember(Description = "Id канала сообщества, куда будут отправляться логи о перемещении задач между столбцами")]
        public ulong TaskLogChannelId { get; set; }

        [YamlMember(Description = "Костыль, минимальное значение Id таска, который отслеживает бот. Иногда YouGile путает Id, берет откуда то фантомные 100 задач, это помогает решить проблему")]
        public int MinTaskValue { get; set; } = 1;

        [YamlMember(Description = "Костыль, минимальное значение времени создания таска, который отслеживает бот")]
        public long MinTaskTimestamp { get; set; } = 1;

        [YamlMember(Description = "Название столбца на доске, задачи с которого будут переноситься на форум. При заполнении все остальные столбцы будут игнорироваться. Несколько столбцов указывать через запятую")]
        public string ActiveColumn { get; set; } = string.Empty;

        [YamlMember(Description = "Название столбца на доске, задачи с которого будут игнорироваться и не переноситься в дискорд, если их там еще нет. Несколько столбцов указывать через запятую")]
        public string IgnoreColumn { get; set; } = string.Empty;
    }
}
