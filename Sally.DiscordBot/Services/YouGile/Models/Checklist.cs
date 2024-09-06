using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class Checklist
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("items")]
        public ChecklistItem[] Items { get; set; }
    }
}
