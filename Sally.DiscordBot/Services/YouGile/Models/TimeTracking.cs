using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class TimeTracking
    {
        [JsonPropertyName("plan")]
        public int Plan { get; set; }

        [JsonPropertyName("work")]
        public int Work { get; set; }
    }
}
