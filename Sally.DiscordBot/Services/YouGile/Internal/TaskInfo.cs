using Discord;
using System.Text.RegularExpressions;

namespace Sally.DiscordBot.Services.YouGile.Internal
{
    using Sally.DiscordBot.Extensions;
    using Sally.DiscordBot.Modules.PublicationStorage;
    using Sally.DiscordBot.Services.YouGile.Models;
    using Sally.DiscordBot.Services.YouGile.Structs;
    using Sally.ServiceDefaults.API.Logger;

    /// <summary>
    /// Задача из YouGile
    /// </summary>
    public sealed class TaskInfo
    {
        /// <summary>
        /// Id задачи (цифра в DEV-#)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Реальный Id задачи в YouGile
        /// </summary>
        public string YouGileId { get; set; }

        /// <summary>
        /// Заголовок задачи
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание задачи (содержит HTML теги)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Столбец, в котором находится задача
        /// </summary>
        public Column Column { get; set; }

        /// <summary>
        /// Стикеры задачи
        /// </summary>
        public Dictionary<Sticker, string> Stickers { get; set; }

        /// <summary>
        /// Сотрудники привязанные к задаче
        /// </summary>
        public List<User> Assigned { get; set; }

        /// <summary>
        /// Unix Timestamp времени создания задачи
        /// </summary>
        public long CreatedAt { get; set; }

        /// <summary>
        /// Дедлайн
        /// </summary>
        public Deadline? Deadline { get; set; }

        /// <summary>
        /// Запись на форуме с эмбедом задачи. <see langword="null"/> если записи нет.
        /// </summary>
        public IUserMessage? PostMessage { get; set; }

        /// <summary>
        /// Столбец, в котором находилась задача до обновления
        /// </summary>
        public TaskInfo? Snapshot { get; set; }

        public TaskInfo(TaskModel model, Column column, Dictionary<Sticker, string> stickers, List<User> assigned)
        {
            YouGileId = model.Id;
            Name = model.Title;
            Description = model.Description ?? "Нет описания";
            Column = column;
            Stickers = stickers;
            Assigned = assigned;
            CreatedAt = model.Timestamp;
            Deadline = model.Deadline;
            Snapshot = null;
        }

        /// <summary>
        /// Конструктор для снапшота
        /// </summary>
        /// <param name="model">Оригинал</param>
        public TaskInfo(TaskInfo model)
        {
            Id = model.Id;
            YouGileId = model.YouGileId;
            Name = model.Name;
            Description = model.Description;
            Column = model.Column;
            Stickers = model.Stickers;
            Assigned = model.Assigned;
            CreatedAt = model.CreatedAt;
            Deadline = model.Deadline;
            PostMessage = model.PostMessage;

            Snapshot = null;
        }

        /// <summary>
        /// Логгирует перемещение между столбцами и добавление/удаление исполнителей в карточке
        /// </summary>
        public async Task LogChangesAsync()
        {
            if (Snapshot is null)
            {
                return;
            }

            var logChannel = Column.Board.Project.TaskLogChannel!;

            if (Column.Id != Snapshot.Column.Id)
            {
                await logChannel.SendMessageAsync(embed: GetMovedEmbededInfo());
            }

            foreach (var oldAssignee in Snapshot.Assigned)
            {
                if (Assigned.Any(a => a.Id == oldAssignee.Id))
                {
                    continue;
                }

                await logChannel.SendMessageAsync(embed: GetAssigneeRemovedEmbed(oldAssignee));
            }

            foreach (var newAssignee in Assigned)
            {
                if (Snapshot.Assigned.Any(a => a.Id == newAssignee.Id))
                {
                    continue;
                }

                await logChannel.SendMessageAsync(embed: GetAssigneeAddedEmbed(newAssignee));
            }

            Snapshot = null;
        }

        /// <summary>
        /// Создает новую или обновляет уже имеющуюся публикацию
        /// </summary>
        /// <param name="settings">Настройки проекта из конфигов</param>
        /// <param name="storageHandler">Хранилище сохраненных задач</param>
        public async Task CreateOrUpdatePublication(YouGileDiscordConnectionSettings settings, PublicationStorageHandler storageHandler)
        {
            var newEmbed = GetEmbededInfo();

            if (PostMessage is null)
            {
                if (storageHandler.IsPublicationSaved(Id, Column.Board.Project.Id) || !ShouldCreateTask(settings))
                {
                    return;
                }

                var post = await Column.Board.Project.PublicationChannel!.CreatePostAsync($"DEV-{Id} | {Name}", ThreadArchiveDuration.OneWeek, embed: newEmbed);
                PostMessage = await post.GetHeadMessageAsync();
                storageHandler.AddPublication(this);
                Log.Info($"Создал новую публикацию {this}");
                return;
            }

            var oldEmbed = PostMessage!.Embeds.FirstOrDefault() as Embed;

            if (oldEmbed == null || oldEmbed.Equals(newEmbed) && oldEmbed.Description.Equals(newEmbed.Description))
            {
                return;
            }

            await PostMessage.ModifyAsync(properties =>
            {
                properties.Embed = newEmbed;
            });

            Log.Info($"Обновил публикацию {this}");
        }

        /// <summary>
        /// Проверяет, нужно ли создавать задачу на основе настроек проекта.
        /// </summary>
        /// <param name="settings">Настройки проекта</param>
        /// <returns>Возвращает <see langword="true"/>, если задачу нужно создать; иначе <see langword="false"/></returns>
        private bool ShouldCreateTask(YouGileDiscordConnectionSettings settings)
        {
            var projectTitle = Column.Board.Project.Title;

            var activeColumns = settings.ActiveColumn.Split(',').Select(s => s.Trim()).ToList();
            var ignoreColumns = settings.IgnoreColumn.Split(',').Select(s => s.Trim()).ToList();

            if (!string.IsNullOrEmpty(settings.ActiveColumn) && activeColumns.Count > 0)
            {
                return activeColumns.Contains(Column.Title);
            }
            else if (!string.IsNullOrEmpty(settings.IgnoreColumn) && ignoreColumns.Count > 0)
            {
                return !ignoreColumns.Contains(Column.Title);
            }
            else
            {
                // Если и ActiveColumn и IgnoreColumn пусты, то не фильтруем по столбцам
                return true;
            }
        }

        /// <summary>
        /// Конвертирует информацию задачи в Discord Embed
        /// </summary>
        /// <returns>Задача в эмбеде</returns>
        public Embed GetEmbededInfo()
        {
            return new EmbedBuilder()
                .WithTitle($"{this}")
                .WithDescription(RemoveHtmlTags(Description))
                .WithColor(Color.Blue)
                .AddField("Статус", Column.Title, true)
                .AddField("\u200B", "\u200B", true) // Placeholder for alignment
                .AddField("Метки", Stickers.Count == 0 ? "-" : GetStickersInfo(), true)
                .AddField("Исполнители", Assigned.Count == 0 ? "-" : GetUsersInfo(), true)
                .AddField("\u200B", "\u200B", true) // Placeholder for alignment
                .AddField("\u200B", "\u200B", true) // Placeholder for alignment
                .AddField("Создан", $"<t:{CreatedAt.ToString().Remove(CreatedAt.ToString().Length - 3)}:R>", true)
                .AddField("\u200B", "\u200B", true) // Placeholder for alignment
                .AddField("Дедлайн", Deadline is null ? "-" : $"<t:{Deadline.DeadlineTime.ToString().Remove(Deadline.DeadlineTime.ToString().Length - 3)}:R>", true)
                .Build();
        }

        /// <summary>
        /// Конвертирует информацию о предыдущем и текущем столбце задачи в Discord Embed
        /// </summary>
        /// <returns>Информация о переносе задачи между столбцами в эмбеде</returns>
        private Embed GetMovedEmbededInfo()
        {
            return new EmbedBuilder()
                .WithTitle($"Статус изменён: #{Id} {Name}")
                .WithColor(Color.Blue)
                .AddField("Было", Snapshot.Column.Title, true)
                .AddField("Стало", Column.Title , true)
                .WithFooter(DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                .Build();
        }

        /// <summary>
        /// Формирует эмбед о создании задачи
        /// </summary>
        /// <returns>Информация в эмбеде о создании задачи</returns>
        private Embed GetCreatedEmbeddedInfo()
        {
            return new EmbedBuilder()
                .WithTitle($"Задача создана: #{Id} {Name}")
                .WithColor(Color.Green)
                .WithFooter(DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                .Build();
        }

        /// <summary>
        /// Формирует эмбед о добавлении исполнителя
        /// </summary>
        /// <param name="assignee">Исполнитель</param>
        /// <returns>Эмбед о добавлении исполнителя</returns>
        private Embed GetAssigneeAddedEmbed(User assignee)
        {
            return new EmbedBuilder()
                .WithTitle($"Исполнитель добавлен: #{Id} {Name}")
                .WithDescription($"Исполнитель: {assignee.Name}")
                .WithColor(Color.Orange)
                .WithFooter(DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                .Build();
        }

        /// <summary>
        /// Формирует эмбед об удалении исполнителя
        /// </summary>
        /// <param name="assignee">Исполнитель</param>
        /// <returns>Эмбед об удалении исполнителя</returns>
        private Embed GetAssigneeRemovedEmbed(User assignee)
        {
            return new EmbedBuilder()
                .WithTitle($"Исполнитель удалён: #{Id} {Name}")
                .WithDescription($"Исполнитель: {assignee.Name}")
                .WithColor(Color.DarkRed)
                .WithFooter(DateTime.Now.ToString("dd.MM.yyyy HH:mm"))
                .Build();
        }

        /// <summary>
        /// Собирает информацию о стикерах в одну строку
        /// </summary>
        /// <returns>Названия текущих состояний стикеров задачи через запятую</returns>
        private string GetStickersInfo()
        {
            return string.Join(", ",
                Stickers.Select(sticker => sticker.Key.States[sticker.Value].Name));
        }

        /// <summary>
        /// Собирает информацию о сотрудниках в одну строку
        /// </summary>
        /// <returns>Имена сотрудников назначенных на задачу через запятую</returns>
        private string GetUsersInfo()
        {
            return string.Join(", ", Assigned.Select(user => user.Name));
        }

        /// <summary>
        /// Заменяет HTML теги на их строковые аналоги
        /// </summary>
        /// <param name="input">Строка с тегами</param>
        /// <returns>Обработанная строка без тегов</returns>
        private static string RemoveHtmlTags(string input)
        {
            // Заменяем теги <br> и <br/> на новые строки
            var output = Regex.Replace(input, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);

            // Заменяем теги <li> на маркер списка
            output = Regex.Replace(output, @"<li\s*/?>", "\n* ", RegexOptions.IgnoreCase);

            // Заменяем теги <p> на перенос строки
            output = Regex.Replace(output, @"<p\s*/?>", "\n", RegexOptions.IgnoreCase);

            // Удаляем все остальные теги
            output = Regex.Replace(output, "<.*?>", string.Empty);

            // Удаляем лишние пробелы и пустые строки
            output = Regex.Replace(output, @"^\s+$[\r\n]*", string.Empty, RegexOptions.Multiline);

            return output.Trim();
        }

        public override string ToString()
        {
            return $"DEV-{Id} | {Name}";
        }
    }
}
