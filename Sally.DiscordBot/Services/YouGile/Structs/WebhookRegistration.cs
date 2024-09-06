namespace Sally.DiscordBot.Services.YouGile.Structs
{
    /// <summary>
    /// Подписка на событие YouGile
    /// </summary>
    public sealed class WebhookRegistration
    {
        /// <summary>
        /// Ссылка куда YouGile будет отсылать события
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Тип события
        /// </summary>
        public string Event { get; set; }
    }
}
