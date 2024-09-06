namespace Sally.ServiceDefaults.API.Features.RabbitMQ.Structs
{
    /// <summary>
    /// Тип события для подписки
    /// </summary>
    public enum EventType
    {
        // события для объектов project,board,column,task,sticker,department,group_chat,chat_message
        None = 0,
        Created,
        Deleted,
        Restored,
        Moved,
        Renamed,
        Updated,

        // события для объектов user
        Added,
        Removed
    }
}
