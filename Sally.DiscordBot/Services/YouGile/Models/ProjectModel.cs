using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class ProjectModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("timestamp")]
        public ulong Timestamp { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
