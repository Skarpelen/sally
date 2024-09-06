using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class StickerState
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }
    }
}
