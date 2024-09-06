namespace Sally.DiscordBot.Services.YouGile.Internal
{
    using Sally.DiscordBot.Services.YouGile.Models;
    
    /// <summary>
    /// Сотрудник из YouGile
    /// </summary>
    public sealed class User
    {
        /// <summary>
        /// Id сотрудника (guid)
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Электронная почта
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Является ли сотрудник администратором
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Имя сотрудника
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Статус сотрудника
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Unix Timestamp последней активности сотрудника
        /// </summary>
        public long LastActivity { get; set; }

        public User(UserModel user)
        {
            Id = user.Id;
            Email = user.Email;
            IsAdmin = user.IsAdmin;
            Name = user.Name;
            Status = user.Status;
            LastActivity = user.LastActivity;
        }
    }
}
