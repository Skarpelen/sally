namespace Sally.DiscordBot.Modules.PublicationStorage.Structs
{
    /// <summary>
    /// Созданная публикация на форуме в дискорде
    /// </summary>
    public struct TaskPublication
    {
        /// <summary>
        /// Id задачи к которой привязана публикация
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id проекта, к которой привязана задача
        /// </summary>
        public string ProjectId { get; set; }

        /// <summary>
        /// Ссылка на пост
        /// </summary>
        public string Url { get; set; }
    }
}
