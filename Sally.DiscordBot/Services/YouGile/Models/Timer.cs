using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class Timer
    {
        [JsonPropertyName("running")]
        public bool Running { get; set; }

        [JsonPropertyName("seconds")]
        public int Seconds { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
    }
}
