namespace Sally.ServiceDefaults.API.Events.Features
{
    using Sally.ServiceDefaults.API.Events.EventArgs.Interfaces;
    using Sally.ServiceDefaults.API.Logger;

    /// <summary>
    /// Кастомный <see cref="EventHandler"/> делегат без параметров.
    /// </summary>
    public delegate void CustomEventHandler();

    /// <summary>
    /// Реализация <see cref="IBrokerEvent"/>, которая инкапсулирует событие без аргументов.
    /// </summary>
    public class Event : ISallyEvent
    {
        /// <summary>
        /// Создает новый экземпляр класса <see cref="Event"/>
        /// </summary>
        public Event()
        {
        }

        private event CustomEventHandler InnerEvent;

        /// <summary>
        /// Подписывает <see cref="CustomEventHandler"/> на внутреннее событие.
        /// </summary>
        /// <param name="event">Событие <see cref="Event"/> на которое подписывается <see cref="CustomEventHandler"/>.</param>
        /// <param name="handler"><see cref="CustomEventHandler"/> для подписки на <see cref="Event"/>.</param>
        /// <returns>Событие <see cref="Event"/> с подписанным обработчиком.</returns>
        public static Event operator +(Event @event, CustomEventHandler handler)
        {
            @event.Subscribe(handler);
            return @event;
        }

        /// <summary>
        /// Отписывает <see cref="CustomEventHandler"/> от внутреннего события.
        /// </summary>
        /// <param name="event">Событие <see cref="Event"/> от которого отписывается <see cref="CustomEventHandler"/>.</param>
        /// <param name="handler"><see cref="CustomEventHandler"/> которое будет отписано от <see cref="Event"/>.</param>
        /// <returns>Событие <see cref="Event"/> с отписанным обработчиком.</returns>
        public static Event operator -(Event @event, CustomEventHandler handler)
        {
            @event.Unsubscribe(handler);
            return @event;
        }

        /// <summary>
        /// Подписывает экземпляр <see cref="CustomEventHandler"/> к внутреннему событию.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Subscribe(CustomEventHandler handler)
        {
            InnerEvent += handler;
        }

        /// <summary>
        /// Отписывает экземпляр <see cref="CustomEventHandler"/> от внутреннего события.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Unsubscribe(CustomEventHandler handler)
        {
            InnerEvent -= handler;
        }

        /// <summary>
        /// Выполняет все <see cref="CustomEventHandler"/> обработчики безопасно.
        /// </summary>
        public void InvokeSafely()
        {
            if (InnerEvent is null)
            {
                return;
            }

            foreach (CustomEventHandler handler in InnerEvent.GetInvocationList().Cast<CustomEventHandler>())
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    Log.Error($"Method \"{handler.Method.Name}\" of the class \"{handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                }
            }
        }
    }
}
