using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Responses
{
    using Sally.DiscordBot.Services.YouGile.Models;

    public sealed class ProjectListResponse
    {
        [JsonPropertyName("paging")]
        public Paging Paging { get; set; }

        [JsonPropertyName("content")]
        public ProjectModel[] Content { get; set; }
    }
}
