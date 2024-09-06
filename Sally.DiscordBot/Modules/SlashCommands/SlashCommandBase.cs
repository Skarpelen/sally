using Discord.WebSocket;

namespace Sally.DiscordBot.Modules.SlashCommands
{
    using Sally.DiscordBot.Modules.SlashCommands.Structs;
    using Sally.ServiceDefaults.API.Logger;

    /// <summary>
    /// Базовый класс для всех слеш команд проекта
    /// </summary>
    public abstract class SlashCommandBase
    {
        /// <summary>
        /// Пустой словарь
        /// </summary>
        private static readonly Dictionary<string, SocketSlashCommandDataOption> EmptyDictionary = new(0);

        /// <summary>
        /// Название команды
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Описание команды
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Синтаксис команды
        /// </summary>
        public virtual string Syntax { get; } = string.Empty;

        /// <summary>
        /// Параметры команды
        /// </summary>
        public virtual CommandParameter[] Parameters { get; } = Array.Empty<CommandParameter>();

        /// <summary>
        /// Дискорд сервер, к которому принадлежит команда
        /// </summary>
        public SocketGuild Guild { get; set; }

        /// <summary>
        /// Количество необходимых параметров
        /// </summary>
        private readonly int _requiredParametersCount;

        public SlashCommandBase()
        {
            if (!string.IsNullOrEmpty(Syntax))
            {
                Parameters = Utils.ParseParameters(Syntax);
            }

            if (Parameters.Length == 0)
            {
                return;
            }

            Syntax = string.Join(" ", Parameters.Select(parameter => parameter.ToString()));

            _requiredParametersCount = Parameters.Count(parameter => parameter.IsRequired);
        }

        /// <summary>
        /// Основная логика команды
        /// </summary>
        /// <param name="command">Команда</param>
        /// <param name="arguments">Аргументы команды</param>
        public abstract Task ExecuteAsync(SocketSlashCommand command, Dictionary<string, SocketSlashCommandDataOption> arguments);

        /// <summary>
        /// Метод для экзекуции команды извне самой команды. Передает команду в класс основную логику слеш команды с распаршенными аргументами
        /// </summary>
        /// <param name="command">Команда</param>
        public virtual async Task ExecuteAsync(SocketSlashCommand command)
        {
            try
            {
                Dictionary<string, SocketSlashCommandDataOption> parsedArguments;
                
                if (Parameters.Length == 0)
                {
                    var i = 0;

                    parsedArguments = command.Data.Options.Count == 0
                        ? EmptyDictionary
                        : command.Data.Options.ToDictionary(_ => (i++).ToString(), value => value);
                }
                else
                {
                    if (_requiredParametersCount > command.Data.Options.Count)
                    {
                        Log.Warning($"Эта часть обработки команд в разработке");
                    }

                    parsedArguments = Utils.ParseArguments(Parameters, command.Data.Options);
                }

                await ExecuteAsync(command, parsedArguments);
            }
            catch (Exception ex)
            {
                var incidentId = Guid.NewGuid().ToString();

                Log.Error($"Can't execute command: {incidentId}");
                Log.Error($"Name: {Name}");
                Log.Error($"User: {command.User.Username}");
                Log.Error($"Arguments: {string.Join(", ", command.Data.Options.Select(o => o.Name))}");
                Log.Error(ex.ToString());

                return;
            }
        }
    }
}
