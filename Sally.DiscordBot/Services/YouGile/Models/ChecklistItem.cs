using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class ChecklistItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }
    }
}
