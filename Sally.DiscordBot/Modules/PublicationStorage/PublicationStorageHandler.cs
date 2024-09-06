using Discord;
using System.Text.Json;

namespace Sally.DiscordBot.Modules.PublicationStorage
{
    using Sally.DiscordBot.Modules.PublicationStorage.Structs;
    using Sally.DiscordBot.Services.YouGile.Internal;
    using Sally.ServiceDefaults.API.Logger;

    public sealed class PublicationStorageHandler
    {
        private string _filePath = Path.Combine(Program.DefaultSavePath, "task_publications.json");

        private HashSet<TaskPublication> _publications;

        public PublicationStorageHandler()
        {
            LoadPublications();
        }

        private void LoadPublications()
        {
            if (File.Exists(_filePath))
            {
                Log.Info("Нашел старое хранилище публикаций, десериализую");
                var json = File.ReadAllText(_filePath);
                _publications = JsonSerializer.Deserialize<HashSet<TaskPublication>>(json)!;
                Log.Info("Count = " + _publications.Count);
            }
            else
            {
                Log.Warning("Старого хранилища публикаций не обнаружено, создаю новое");
                _publications = new HashSet<TaskPublication>();
            }
        }

        public async Task SavePublicationsAsync()
        {
            Log.Info("Сохраняю публикации в json...");

            using var stream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(stream, _publications);
        }

        public void AddPublication(TaskInfo task)
        {
            var publication = new TaskPublication()
            {
                Id = task.Id,
                ProjectId = task.Column.Board.Project.Id,
                Url = task.PostMessage.GetJumpUrl()
            };

            if (_publications.Add(publication))
            {
                Log.Info($"Добавил публикацию {task} к json списку");
            }
        }

        public bool IsPublicationSaved(int taskId, string projectId)
        {
            return _publications.Count(p => p.Id == taskId && p.ProjectId == projectId) > 0;
        }
    }
}
