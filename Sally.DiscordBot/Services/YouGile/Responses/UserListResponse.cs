using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Responses
{
    using Sally.DiscordBot.Services.YouGile.Models;

    public sealed class UserListResponse
    {
        [JsonPropertyName("paging")]
        public Paging Paging { get; set; }

        [JsonPropertyName("content")]
        public UserModel[] Content { get; set; }
    }
}
