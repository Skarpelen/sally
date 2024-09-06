namespace Sally.DiscordBot.Services.YouGile.Internal
{
    using Sally.DiscordBot.Services.YouGile.Models;

    /// <summary>
    /// Доска задач из YouGile
    /// </summary>
    public sealed class Board
    {
        /// <summary>
        /// Id доски (guid)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Удалена ли доска
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Название доски
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Проект в котором находится доска
        /// </summary>
        public Project Project { get; set; }

        public Board(BoardModel board, Project project)
        {
            Id = board.Id;
            IsDeleted = board.Deleted;
            Title = board.Title;
            Project = project;
        }
    }
}
