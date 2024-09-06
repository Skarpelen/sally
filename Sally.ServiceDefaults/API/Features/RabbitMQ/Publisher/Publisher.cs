using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Sally.ServiceDefaults.API.Features.RabbitMQ.Publisher
{
    using Sally.ServiceDefaults.API.Logger;

    public class Publisher
    {
        public bool QueueMessage(object message)
        {
            var retVal = false;

            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq",
                    Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
                    UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
                    Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest"
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "yougileEventQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var stringContent = JsonConvert.SerializeObject(message);
                    var body = Encoding.UTF8.GetBytes(stringContent);

                    channel.BasicPublish(exchange: "", routingKey: "yougileEventQueue", basicProperties: null, body: body);
                }

                retVal = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            return retVal;
        }
    }
}
