using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class Stopwatch
    {
        [JsonPropertyName("running")]
        public bool Running { get; set; }

        [JsonPropertyName("time")]
        public int Time { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
    }
}
