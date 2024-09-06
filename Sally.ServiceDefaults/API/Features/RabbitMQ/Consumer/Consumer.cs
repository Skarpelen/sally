using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Sally.ServiceDefaults.API.Features.RabbitMQ.Consumer
{
    using Sally.ServiceDefaults.API.Events.EventArgs.YouGile;
    using Sally.ServiceDefaults.API.Events.Handlers;
    using Sally.ServiceDefaults.API.Features.RabbitMQ.Models;
    using Sally.ServiceDefaults.API.Logger;

    public class Consumer
    {
        public void Consume()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "rabbitmq",
                    Port = int.Parse(Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672"),
                    UserName = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest",
                    Password = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest"
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: "yougileEventQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                Log.Info("Ожидаю события с RabbitMQ");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += ConsumeEventOnRecieved;

                channel.BasicConsume(queue: "yougileEventQueue", autoAck: true, consumer: consumer);

                while (true)
                {
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        private void ConsumeEventOnRecieved(object model, BasicDeliverEventArgs ev)
        {
            Log.Info("Получил событие");

            var body = ev.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            var youGileEvent = JsonConvert.DeserializeObject<YouGileEvent>(message);

            if (youGileEvent is not null)
            {
                YouGile.OnYouGileEventReceived(new YouGileEventReceivedEventArgs(youGileEvent));
            }
            else
            {
                Log.Info("Не удалось преобразовать сообщение с RabbitMQ в класс YouGileEvent");
            }
        }
    }
}