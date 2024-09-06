namespace Sally.ServiceDefaults.API.Events.Handlers
{
    using Sally.ServiceDefaults.API.Events.EventArgs.YouGile;
    using Sally.ServiceDefaults.API.Events.Features;

    public static class YouGile
    {
        /// <summary>
        /// Вызывается когда YouGile отправляет событие
        public static Event<YouGileEventReceivedEventArgs> YouGileEventReceived { get; set; } = new();

        public static void OnYouGileEventReceived(YouGileEventReceivedEventArgs ev) => YouGileEventReceived.InvokeSafely(ev);
    }
}
