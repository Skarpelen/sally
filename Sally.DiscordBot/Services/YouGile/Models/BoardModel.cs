using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class BoardModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("projectId")]
        public string ProjectId { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
