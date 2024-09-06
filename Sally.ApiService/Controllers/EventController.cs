using Microsoft.AspNetCore.Mvc;

namespace Sally.ApiService.Controllers
{
    using Sally.ServiceDefaults.API.Features.RabbitMQ.Models;
    using Sally.ServiceDefaults.API.Features.RabbitMQ.Publisher;
    using Sally.ServiceDefaults.API.Features.RabbitMQ.Structs;
    using Sally.ServiceDefaults.API.Logger;

    [ApiController]
    [Route("api/[controller]")]
    public sealed class EventController : ControllerBase
    {
        private Publisher _publisher = new();

        [HttpPost("card-moved")]
        public IActionResult CardMoved()
        {
            var rndId = Random.Shared.Next();
            Log.Info($"Получил событие {rndId} на передвижение задачи между столбцами");

            if (_publisher.QueueMessage(new YouGileEvent { ObjectType = ObjectType.Task, EventType = EventType.Moved }))
            {
                Log.Info($"Переслал событие {rndId} к боту");
                return Accepted("Message forwarded to bot");
            }

            Log.Info($"Не удалось переслать событие {rndId} к боту");
            return Problem("Failed to forward message to bot");
        }

        [HttpPost("card-updated")]
        public IActionResult CardUpdated()
        {
            var rndId = Random.Shared.Next();
            Log.Info($"Получил событие {rndId} на обновление данных в задаче");

            if (_publisher.QueueMessage(new YouGileEvent { ObjectType = ObjectType.Task, EventType = EventType.Updated }))
            {
                Log.Info($"Переслал событие {rndId} к боту");
                return Accepted("Message forwarded to bot");
            }

            Log.Info($"Не удалось переслать событие {rndId} к боту");
            return Problem("Failed to forward message to bot");
        }

        [HttpPost("card-renamed")]
        public IActionResult CardRenamed()
        {
            var rndId = Random.Shared.Next();
            Log.Info($"Получил событие {rndId} на переименование задачи");

            if (_publisher.QueueMessage(new YouGileEvent { ObjectType = ObjectType.Task, EventType = EventType.Renamed }))
            {
                Log.Info($"Переслал событие {rndId} к боту");
                return Accepted("Message forwarded to bot");
            }

            Log.Info($"Не удалось переслать событие {rndId} к боту");
            return Problem("Failed to forward message to bot");
        }

        [HttpPost("card-created")]
        public IActionResult CardCreated()
        {
            var rndId = Random.Shared.Next();
            Log.Info($"Получил событие {rndId} на создание новой задачи");

            if (_publisher.QueueMessage(new YouGileEvent { ObjectType = ObjectType.Task, EventType = EventType.Created }))
            {
                Log.Info($"Переслал событие {rndId} к боту");
                return Accepted("Message forwarded to bot");
            }

            Log.Info($"Не удалось переслать событие {rndId} к боту");
            return Problem("Failed to forward message to bot");
        }
    }
}
