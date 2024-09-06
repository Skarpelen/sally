using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Responses
{
    public sealed class KeysResponse
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
    }
}
