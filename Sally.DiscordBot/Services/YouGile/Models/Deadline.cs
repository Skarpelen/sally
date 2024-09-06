using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class Deadline
    {
        [JsonPropertyName("deadline")]
        public long DeadlineTime { get; set; }

        [JsonPropertyName("startDate")]
        public long StartDate { get; set; }

        [JsonPropertyName("withTime")]
        public bool WithTime { get; set; }
    }
}
