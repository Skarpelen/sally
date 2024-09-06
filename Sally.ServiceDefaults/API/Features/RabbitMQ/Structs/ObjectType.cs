namespace Sally.ServiceDefaults.API.Features.RabbitMQ.Structs
{
    /// <summary>
    /// Тип объекта для подписки на его события
    /// </summary>
    public enum ObjectType
    {
        Project,
        Board,
        Column,
        Task,
        Sticker,
        Department,
        GroupChat,
        ChatMessage,
        User
    }
}
