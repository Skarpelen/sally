using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class StickerModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("states")]
        public StickerState[] States { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
