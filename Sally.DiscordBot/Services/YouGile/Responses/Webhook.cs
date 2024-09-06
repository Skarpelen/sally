using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Responses
{
    public sealed class Webhook
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }

        [JsonPropertyName("lastSuccess")]
        public long LastSuccess { get; set; }

        [JsonPropertyName("failuresSinceLastSuccess")]
        public long FailuresSinceLastSuccess { get; set; }
    }
}
