namespace Sally.DiscordBot.Services.YouGile.Internal
{
    using Sally.DiscordBot.Services.YouGile.Models;

    /// <summary>
    /// Стикер из YouGile и его состояния
    /// </summary>
    public sealed class Sticker
    {
        /// <summary>
        /// Id стикера
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Удален ли стикер
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Название стикера (не путать с названием состояний стикера!)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Иконка стикера
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Состояния стикера
        /// </summary>
        public Dictionary<string, StickerState> States { get; set; }

        public Sticker(StickerModel model)
        {
            Id = model.Id;
            IsDeleted = model.Deleted;
            Name = model.Name;
            Icon = model.Icon;
            States = model.States.ToDictionary(kvp => kvp.Id);
        }
    }
}
