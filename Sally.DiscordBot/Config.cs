using YamlDotNet.Serialization;

namespace Sally.DiscordBot
{
    using Sally.DiscordBot.Services.YouGile;
    using Sally.ServiceDefaults.API.ConfigLoader;

    public sealed class Config : ConfigBase<Config>
    {
        [YamlMember(Description = "Токен дискорд бота")]
        public string DiscordBotToken { get; set; } = "token";

        [YamlMember(Description = "Конфигурация YouGile модуля")]
        public YouGileConfig YouGileConfig { get; set; } = new();

        public Config(string filePath) : base(filePath)
        {
        }

        public Config()
        {
        }
    }
}
