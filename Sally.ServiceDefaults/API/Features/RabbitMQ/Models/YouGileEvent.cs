namespace Sally.ServiceDefaults.API.Features.RabbitMQ.Models
{
    using Sally.ServiceDefaults.API.Features.RabbitMQ.Structs;

    public class YouGileEvent
    {
        public ObjectType ObjectType { get; set; }

        public EventType EventType { get; set; }
    }
}
