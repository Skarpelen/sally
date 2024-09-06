using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Responses
{
    public sealed class KeysGetResponse
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("companyId")]
        public string CompanyId { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }
    }
}
