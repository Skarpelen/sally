using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Responses
{
    using Sally.DiscordBot.Services.YouGile.Models;

    public sealed class StickersListResponse
    {
        [JsonPropertyName("paging")]
        public Paging Paging { get; set; }

        [JsonPropertyName("content")]
        public StickerModel[] Content { get; set; }
    }
}
