using Discord;

namespace Sally.DiscordBot.Modules.SlashCommands.Structs
{
    /// <summary>
    /// Параметр команды
    /// </summary>
    public struct CommandParameter
    {
        /// <summary>
        /// Параметр команды
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отображаемое название параметра
        /// </summary>
        public string Descriprion { get; set; }

        /// <summary>
        /// Обязателен ли параметр
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Повторяется ли параметр
        /// </summary>
        public bool IsRepeated { get; set; }

        /// <summary>
        /// Тип параметра
        /// </summary>
        public ApplicationCommandOptionType Type { get; set; }

        /// <summary>
        /// Возвращает представление синтаксиса
        /// </summary>
        public override string ToString()
        {
            return string.Format(
                "{0}{1}{2}{3}",
                IsRequired ? '<' : '[',
                string.IsNullOrEmpty(Descriprion) ? Name : Descriprion,
                IsRepeated ? "..." : string.Empty,
                IsRequired ? '>' : ']'
                );
        }
    }
}
