using Discord.WebSocket;

namespace Sally.DiscordBot.Services.YouGile.Internal
{
    using Sally.DiscordBot.Services.YouGile.Models;

    /// <summary>
    /// Проект из YouGile
    /// </summary>
    public sealed class Project
    {
        /// <summary>
        /// Id проекта (guid)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Удален ли проект
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Название проекта
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Когда был создан проект
        /// </summary>
        public ulong CreatedAt { get; set; }

        /// <summary>
        /// Привязанный к проекту форум для задач
        /// </summary>
        public SocketForumChannel? PublicationChannel { get; set; }

        /// <summary>
        /// Привязанный к проекту канал для логов передвижения карточек
        /// </summary>
        public SocketTextChannel? TaskLogChannel { get; set; }

        public Project(ProjectModel project)
        {
            Id = project.Id;
            IsDeleted = project.Deleted;
            Title = project.Title;
            CreatedAt = project.Timestamp;
        }
    }
}
