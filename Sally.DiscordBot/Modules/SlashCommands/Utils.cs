using System.Text.RegularExpressions;
using Discord.WebSocket;

namespace Sally.DiscordBot.Modules.SlashCommands
{
    using Sally.DiscordBot.Modules.SlashCommands.Structs;

    public static class Utils
    {
        /// <summary>
        /// Паттерн параметров команды
        /// </summary>
        private static readonly Regex SyntaxPattern = new(@"(<|\[)(.+?)(\.\.\.)?(?|\])");

        /// <summary>
        /// Переводит зашифрованный синтаксис команды в ее параметры
        /// </summary>
        /// <param name="syntax">Синтаксис команды</param>
        /// <returns>Параметры команды</returns>
        public static CommandParameter[] ParseParameters(string syntax)
        {
            // Количество параметров равно количество совпадений
            var matches = SyntaxPattern.Matches(syntax);
            var parameters = new CommandParameter[matches.Count];

            for (var i = 0; i < matches.Count; i++)
            {
                // $1 - открывающий символ
                // $2 - название команды
                // $3 - пустая строка или троеточие
                // $4 - закрывающий символ
                var groups = matches[i].Groups;

                parameters[i] = new CommandParameter
                {
                    Name = groups[2].Value,
                    IsRequired = groups[1].Value == "<",
                    IsRepeated = groups[3].Value == "..."
                };
            }

            return parameters;
        }

        /// <summary>
        /// Собирает словарь с аргументами команды, где ключ - название аргумента
        /// </summary>
        /// <param name="parameters">Параметры команды с информацией об аргументах</param>
        /// <param name="options">Передаваемые значения в команду</param>
        /// <returns>Аргументы команды в словаре</returns>
        public static Dictionary<string, SocketSlashCommandDataOption> ParseArguments(CommandParameter[] parameters, IReadOnlyCollection<SocketSlashCommandDataOption> options)
        {
            var newOptions = options.ToArray();
            var parsedArguments = new Dictionary<string, SocketSlashCommandDataOption>();

            for (var i = 0; parameters.Length > i; i++)
            {
                var parameter = parameters[i];

                if (options.Count <= i)
                {
                    parsedArguments.Add(parameter.Name, null);
                    continue;
                }

                parsedArguments.Add(parameter.Name, newOptions[i]);
            }

            return parsedArguments;
        }
    }
}
