namespace Sally.DiscordBot.Services.YouGile.Internal
{
    using Sally.DiscordBot.Services.YouGile.Models;

    /// <summary>
    /// Столбец из YouGile
    /// </summary>
    public sealed class Column
    {
        /// <summary>
        /// Id столбца (guid)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Удален ли столбец
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Название столбца
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Цвет столбца
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// Доска где расположен столбец
        /// </summary>
        public Board Board { get; set; }

        public Column(ColumnModel column, Board board)
        {
            Id = column.Id;
            IsDeleted = column.Deleted;
            Title = column.Title;
            Color = column.Color;
            Board = board;
        }
    }
}
