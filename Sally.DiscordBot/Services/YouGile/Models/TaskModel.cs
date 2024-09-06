using System.Text.Json.Serialization;

namespace Sally.DiscordBot.Services.YouGile.Models
{
    using Sally.DiscordBot.Services.YouGile.Utils;

    public sealed class TaskModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("columnId")]
        public string ColumnId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("archived")]
        public bool Archived { get; set; }

        [JsonPropertyName("archivedTimestamp")]
        public long ArchivedTimestamp { get; set; }

        [JsonPropertyName("completed")]
        public bool Completed { get; set; }

        [JsonPropertyName("completedTimestamp")]
        public long CompletedTimestamp { get; set; }

        [JsonPropertyName("subtasks")]
        public string[]? Subtasks { get; set; }

        [JsonPropertyName("assigned")]
        [JsonConverter(typeof(AssignedConverter))]
        public string[] Assigned { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("deadline")]
        public Deadline? Deadline { get; set; }

        [JsonPropertyName("timeTracking")]
        public TimeTracking? TimeTracking { get; set; }

        [JsonPropertyName("checklists")]
        public Checklist[]? Checklists { get; set; }

        [JsonPropertyName("stickers")]
        public Dictionary<string, string>? Stickers { get; set; }

        [JsonPropertyName("stopwatch")]
        public Stopwatch? Stopwatch { get; set; }

        [JsonPropertyName("timer")]
        public Timer? Timer { get; set; }

        public override string ToString()
        {
            return Title ?? Id;
        }
    }
}
