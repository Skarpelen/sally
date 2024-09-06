using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    public sealed class UserModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("isAdmin")]
        public bool IsAdmin { get; set; }

        [JsonPropertyName("realName")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("lastActivity")]
        public long LastActivity { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
