namespace Sally.ServiceDefaults.API.Events.Features
{
    using Sally.ServiceDefaults.API.Events.EventArgs.Interfaces;
    using Sally.ServiceDefaults.API.Logger;

    /// <summary>
    /// Кастомный <see cref="EventHandler"/> делегат.
    /// </summary>
    /// <typeparam name="TEventArgs"><see cref="EventHandler{TEventArgs}"/> тип.</typeparam>
    /// <param name="ev"><see cref="EventHandler{TEventArgs}"/> инстанс.</param>
    public delegate void CustomEventHandler<TEventArgs>(TEventArgs ev);

    public class Event<T> : ISallyEvent
    {
        /// <summary>
        /// Создает новый экземпляр класса <see cref="Event"/>
        /// </summary>
        public Event()
        {
        }

        private event CustomEventHandler<T> InnerEvent;

        /// <summary>
        /// Подписывает <see cref="CustomEventHandler{TEventArgs}"/> на внутреннее событие.
        /// </summary>
        /// <param name="event">Событие <see cref="Event{T}"/> на которое подписывается <see cref="CustomEventHandler{T}"/>.</param>
        /// <param name="handler"><see cref="CustomEventHandler{T}"/> для подписки на <see cref="Event{T}"/>.</param>
        /// <returns>Событие <see cref="Event{T}"/> с подписанным обработчиком.</returns>
        public static Event<T> operator +(Event<T> @event, CustomEventHandler<T> handler)
        {
            @event.Subscribe(handler);
            return @event;
        }

        /// <summary>
        /// Отписывает <see cref="CustomEventHandler{TEventArgs}"/> от внутреннего события.
        /// </summary>
        /// <param name="event">Событие <see cref="Event{T}"/> от которого отписывается <see cref="CustomEventHandler{T}"/>.</param>
        /// <param name="handler"><see cref="CustomEventHandler{T}"/> которое будет отписано от <see cref="Event{T}"/>.</param>
        /// <returns>Событие <see cref="Event{T}"/> с отписанным обработчиком.</returns>
        public static Event<T> operator -(Event<T> @event, CustomEventHandler<T> handler)
        {
            @event.Unsubscribe(handler);
            return @event;
        }

        /// <summary>
        /// Подписывает экземпляр <see cref="CustomEventHandler{T}"/> к внутреннему событию.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Subscribe(CustomEventHandler<T> handler)
        {
            InnerEvent += handler;
        }

        /// <summary>
        /// Отписывает экземпляр <see cref="CustomEventHandler{T}"/> от внутреннего события.
        /// </summary>
        /// <param name="handler">The handler to add.</param>
        public void Unsubscribe(CustomEventHandler<T> handler)
        {
            InnerEvent -= handler;
        }

        /// <summary>
        /// Выполняет все <see cref="CustomEventHandler{TEventArgs}"/> обработчики безопасно.
        /// </summary>
        /// <param name="arg">Аргумент события.</param>
        /// <exception cref="ArgumentNullException">Аргумент события <see langword="null"/>.</exception>
        public void InvokeSafely(T arg)
        {
            if (InnerEvent is null)
            {
                return;
            }

            foreach (CustomEventHandler<T> handler in InnerEvent.GetInvocationList().Cast<CustomEventHandler<T>>())
            {
                try
                {
                    handler(arg);
                }
                catch (Exception ex)
                {
                    Log.Error($"Method \"{handler.Method.Name}\" of the class \"{handler.Method.ReflectedType.FullName}\" caused an exception when handling the event \"{GetType().FullName}\"\n{ex}");
                }
            }
        }
    }
}
