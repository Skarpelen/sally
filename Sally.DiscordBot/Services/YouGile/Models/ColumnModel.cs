using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class ColumnModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("color")]
        public int Color { get; set; }

        [JsonPropertyName("boardId")]
        public string BoardId { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
