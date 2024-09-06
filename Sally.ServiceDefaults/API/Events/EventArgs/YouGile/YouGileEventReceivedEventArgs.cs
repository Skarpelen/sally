namespace Sally.ServiceDefaults.API.Events.EventArgs.YouGile
{
    using Sally.ServiceDefaults.API.Events.EventArgs.Interfaces;
    using Sally.ServiceDefaults.API.Features.RabbitMQ.Models;
    using Sally.ServiceDefaults.API.Features.RabbitMQ.Structs;

    public class YouGileEventReceivedEventArgs : ISallyEvent
    {
        public YouGileEventReceivedEventArgs(YouGileEvent youGileEvent)
        {
            ObjectType = youGileEvent.ObjectType;
            EventType = youGileEvent.EventType;
        }

        public ObjectType ObjectType { get; set; }

        public EventType EventType { get; set; }
    }
}
