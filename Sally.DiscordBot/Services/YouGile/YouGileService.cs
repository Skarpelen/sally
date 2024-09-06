using Discord.WebSocket;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Sally.DiscordBot.Services.YouGile
{
    using Sally.DiscordBot.Extensions;
    using Sally.DiscordBot.Modules.PublicationStorage;
    using Sally.DiscordBot.Services.YouGile.Internal;
    using Sally.DiscordBot.Services.YouGile.Responses;
    using Sally.DiscordBot.Services.YouGile.Structs;
    using Sally.ServiceDefaults.API.Events.EventArgs.YouGile;
    using Sally.ServiceDefaults.API.Features.RabbitMQ.Structs;
    using Sally.ServiceDefaults.API.Logger;
    using System.Text.RegularExpressions;
    using EventSource = ServiceDefaults.API.Events.Handlers.YouGile;

    /// <summary>
    /// Сервис для взаимодействия с YouGile
    /// </summary>
    public class YouGileService
    {
        /// <summary>
        /// Список задач, id - номер задачи, DEV-id
        /// </summary>
        public Dictionary<string, TaskInfo> Tasks { get; private set; } = new();

        /// <summary>
        /// Зарегистрированные стикеры, ключ - их Id
        /// </summary>
        public Dictionary<string, Sticker> Stickers { get; private set; } = new();

        /// <summary>
        /// Зарегистрированные сотрудники, ключ - их Id
        /// </summary>
        public Dictionary<string, User> Users { get; private set; } = new();

        /// <summary>
        /// Зарегистрированные колонки в досках, ключ - их Id
        /// </summary>
        public Dictionary<string, Column> Columns { get; private set; } = new();

        /// <summary>
        /// Зарегистрированные доски, ключ - их Id
        /// </summary>
        public Dictionary<string, Board> Boards { get; private set; } = new();

        /// <summary>
        /// Зарегистрированные проекты, ключ - их Id
        /// </summary>
        public Dictionary<string, Project> Projects { get; private set; } = new();

        /// <summary>
        /// Основа обращения к API YouGile
        /// </summary>
        private const string ApiBaseUrl = "https://ru.yougile.com/api-v2";

        /// <summary>
        /// События YouGile которые бот будет слушать
        /// </summary>
        private readonly List<WebhookRegistration> EventHooks = new()
        {
            new WebhookRegistration { Url = $"{Program.Config.YouGileConfig.ExternalUrl}api/event/card-moved", Event = "task-moved" },
            new WebhookRegistration { Url = $"{Program.Config.YouGileConfig.ExternalUrl}api/event/card-updated", Event = "task-updated" },
            new WebhookRegistration { Url = $"{Program.Config.YouGileConfig.ExternalUrl}api/event/card-renamed", Event = "task-renamed" },
            new WebhookRegistration { Url = $"{Program.Config.YouGileConfig.ExternalUrl}api/event/card-created", Event = "task-created" }
        };

        /// <summary>
        /// Клиент для запросов
        /// </summary>
        private readonly HttpClient _httpClient = new();

        /// <summary>
        /// Хранилище сохраненных публикаций
        /// </summary>
        private readonly PublicationStorageHandler _publicationStorageHandler;

        public YouGileService(PublicationStorageHandler publicationStorageHandler)
        {
            _publicationStorageHandler = publicationStorageHandler;
        }

        /// <summary>
        /// Ищет задачу среди сохраненных по ее Id
        /// </summary>
        /// <param name="id">Id задачи, указывается в DEV-#</param>
        /// <returns>Найденная задача или <see langword="null"/></returns>
        public TaskInfo? GetTaskInfo(int id)
        {
            return Tasks.Values.FirstOrDefault(task => task.Id == id);
        }

        /// <summary>
        /// Ищет проект среди сохраненных по его названию
        /// </summary>
        /// <param name="title">Название проекта</param>
        /// <returns>Найденный проект или <c langword="null"/c></returns>
        public Project? GetProject(string title)
        {
            return Projects.Values.FirstOrDefault(project => project.Title == title);
        }

        /// <summary>
        /// Ищет и сохраняет каналы-форумы из конфигов
        /// </summary>
        /// <param name="guild">Дискорд-сообщество</param>
        public async Task SaveChannelsForGuid(SocketGuild guild)
        {
            foreach (var settings in Program.Config.YouGileConfig.ConnectionSettings)
            {
                if (settings.Value.GuildId != guild.Id)
                {
                    continue;
                }

                var publicationsChannel = guild.GetForumChannel(settings.Value.ForumChannelId);

                if (publicationsChannel is null)
                {
                    Log.Error($"Не удалось найти канал-форум для публикаций с ID {settings.Value.ForumChannelId} на сервере {guild.Name}");
                    continue;
                }

                var logChannel = guild.GetTextChannel(settings.Value.TaskLogChannelId);

                if (logChannel is null)
                {
                    Log.Error($"Не удалось найти канал для логов карточек с ID {settings.Value.TaskLogChannelId} на сервере {guild.Name}");
                    continue;
                }

                var project = GetProject(settings.Key);

                if (project is null)
                {
                    Log.Error($"Не загружен проект {settings.Key} объявленный в конфигах");
                    continue;
                }

                project.PublicationChannel = publicationsChannel;
                project.TaskLogChannel = logChannel;
                Log.Info($"Найден каналы для публикаций и логов: {publicationsChannel.Name} и {logChannel.Name}. Анализирую записи канала для публикаций...");

                await LocatePublicationsAsync(publicationsChannel, project);

                await UpdatePublicationsAtForumAsync(project);
            }
        }

        /// <summary>
        /// Находит все публикации в канале, относящиеся к проекту и заполняет относящиеся к ним <see cref="TaskInfo"/>
        /// </summary>
        /// <param name="channel">Канал-форум</param>
        /// <param name="project">Проект</param>
        private async Task LocatePublicationsAsync(SocketForumChannel channel, Project project)
        {
            var activePosts = await channel.GetActiveThreadsAsync();
            var publicationsCount = 0;
            var forumPostStyle = new Regex(@"DEV-(\d+)");

            var currentProjectTasks = Tasks.Values.Where(task => task.Column.Board.Project == project);

            foreach (var post in activePosts)
            {
                var match = forumPostStyle.Match(post.Name);

                if (!match.Success)
                {
                    continue;
                }

                if (!int.TryParse(match.Groups[1].Value, out var id))
                {
                    Log.Warning($"Не удалось определить задачу для поста с заголовком {post.Name}");
                    continue;
                }

                var task = Tasks.Values.FirstOrDefault(t => t.Id == id);

                if (task is null)
                {
                    Log.Warning($"Существует запись для задачи {id}, но она не найдена в YouGile");
                    continue;
                }

                task.PostMessage = await post.GetHeadMessageAsync();
                publicationsCount++;

                _publicationStorageHandler.AddPublication(task);
            }

            await _publicationStorageHandler.SavePublicationsAsync();
            Log.Info($"Считано публикаций в канале {channel.Name}: {publicationsCount}");
        }

        /// <summary>
        /// Авторизуется в YouGile и скачивает все необходимые для работы данные
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            CleanUp();

            try
            {
                await LogInAsync(Program.Config.YouGileConfig.LogInEmail, Program.Config.YouGileConfig.LogInPassword, Program.Config.YouGileConfig.CompanyId);
            }
            catch (Exception ex)
            {
                Log.Error($"Не удалось авторизоваться в YouGile с почтой {Program.Config.YouGileConfig.LogInEmail}: {ex.Message}");
                await Task.FromException(ex);
                return;
            }

            await CreateWebhooksAsync();

            await GetAllUsers();
            await GetAllProjects();
            await GetAllBoards();
            await GetAllColumns();
            await GetAllStickers();
            await GetAndUpdateAllTasks();

            RegisterEvents();
        }

        /// <summary>
        /// Очищает все списки и словари
        /// </summary>
        private void CleanUp()
        {
            Tasks.Clear();
            Projects.Clear();
            Boards.Clear();
            Stickers.Clear();
            Columns.Clear();
            Users.Clear();
        }

        /// <summary>
        /// Авторизуется в YouGile, добавляя к <see cref="_httpClient"/> коды авторизации
        /// </summary>
        /// <param name="login">email пользователя</param>
        /// <param name="password">Пароль пользователя</param>
        /// <param name="companyId">Id компании</param>
        /// <exception cref="Exception">Возможные различные ошибки при авторизации</exception>
        private async Task LogInAsync(string login, string password, string companyId)
        {
            var loginRequest = new
            {
                login,
                password,
                companyId
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{ApiBaseUrl}/auth/keys/get", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to authenticate with YouGile API.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<List<KeysGetResponse>>(responseContent);

            var validKey = string.Empty;

            if (responseData is null || responseData.Count == 0)
            {
                validKey = await CreateKeyAsync(json);
            }
            else
            {
                validKey = responseData.FirstOrDefault(key => !key.Deleted)?.Key;
            }

            if (string.IsNullOrEmpty(validKey))
            {
                throw new Exception("No valid keys available.");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", validKey);
        }

        /// <summary>
        /// Создает новый ключ авторизации
        /// </summary>
        /// <param name="loginRequest">Логин, пароль и id компании в json строке</param>
        /// <returns>Новый ключ авторизации</returns>
        /// <exception cref="Exception">Возможные различные ошибки при авторизации</exception>
        private async Task<string> CreateKeyAsync(string loginRequest)
        {
            var content = new StringContent(loginRequest, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{ApiBaseUrl}/auth/keys", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Не удалось авторизоваться в YouGile API.");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<KeysResponse>(responseContent);

            return responseData is null ? throw new Exception("Не удалось создать ключ YouGile API.") : responseData.Key;
        }

        /// <summary>
        /// Создание подписок на события, если их еще нет
        /// </summary>
        private async Task CreateWebhooksAsync()
        {
            Log.Info("Проверка списка подписок на события...");

            var webhooksResponse = await _httpClient.GetAsync($"{ApiBaseUrl}/webhooks");

            if (!webhooksResponse.IsSuccessStatusCode)
            {
                throw new Exception("Не удалось получить список подписок в YouGile API.");
            }

            var responseContent = await webhooksResponse.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<List<Webhook>>(responseContent)!;

            foreach (var webhookRegistration in EventHooks)
            {
                if (responseData.Any(w => w.Url == webhookRegistration.Url && w.Event == webhookRegistration.Event))
                {
                    continue;
                }

                Log.Warning($"Не найдено ключей подписки для URL {webhookRegistration.Url} и события {webhookRegistration.Event}, создаю новые");

                var webhookRequest = new
                {
                    url = webhookRegistration.Url,
                    @event = webhookRegistration.Event
                };

                var json = JsonSerializer.Serialize(webhookRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var postResponse = await _httpClient.PostAsync($"{ApiBaseUrl}/webhooks", content);

                if (!postResponse.IsSuccessStatusCode)
                {
                    Log.Error($"Не удалось создать подписку для URL {webhookRegistration.Url} и события {webhookRegistration.Event}");
                }
            }

            Log.Info("Подписки на все события актуализированы");
        }

        /// <summary>
        /// Скачивает с YouGile все задачи
        /// </summary>
        private async Task GetAndUpdateAllTasks()
        {
            Log.Info("Процесс закачки актуальных задач начался...");

            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/tasks?limit=1000");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Не вышло получить список задач с YouGile.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var rawTasks = JsonSerializer.Deserialize<TaskListResponse>(content)!;
            var tasks = new List<TaskInfo>();

            foreach (var task in rawTasks.Content)
            {
                if (string.IsNullOrEmpty(task.ColumnId) || !Columns.TryGetValue(task.ColumnId, out var column))
                {
                    continue;
                }

                var stickers = task.Stickers?
                    .Where(stickerId => Stickers.TryGetValue(stickerId.Key, out _))
                    .ToDictionary(stickerId => Stickers[stickerId.Key], stickerId => stickerId.Value)
                    ?? new Dictionary<Sticker, string>();
                var assigned = Users.Values.Where(user => task.Assigned?.Contains(user.Id) ?? false).ToList();

                tasks.Add(new TaskInfo(task, column, stickers, assigned));
            }

            foreach (var project in Projects.Values)
            {
                var settings = Program.Config.YouGileConfig.ConnectionSettings[project.Title];
                var minTaskValue = settings.MinTaskValue;
                var minTaskTimestamp = settings.MinTaskTimestamp;

                var sortedTasks = tasks.Where(t => t.Column.Board.Project == project && t.CreatedAt >= minTaskTimestamp).OrderBy(t => t.CreatedAt).ToList();

                lock (Tasks)
                {
                    for (var i = 0; i < sortedTasks.Count; i++)
                    {
                        var task = sortedTasks[i];
                        task.Id = i + minTaskValue;

                        if (!Tasks.TryAdd(task.YouGileId, task))
                        {
                            var oldTask = Tasks[task.YouGileId];
                            task.Snapshot = new TaskInfo(oldTask);
                            task.PostMessage = oldTask.PostMessage;
                            Tasks[task.YouGileId] = task;
                        }
                    }
                }
            }

            Log.Info($"Нашел и актуализировал {Tasks.Count} задач");
        }

        /// <summary>
        /// Скачивает с YouGile все стикеры
        /// </summary>
        private async Task GetAllStickers()
        {
            Log.Info("Процесс закачки актуальных стикеров начался...");

            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/string-stickers?limit=1000");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Не вышло получить список стикеров с YouGile.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var rawStickers = JsonSerializer.Deserialize<StickersListResponse>(content)!;

            foreach (var sticker in rawStickers.Content)
            {
                Stickers.TryAdd(sticker.Id, new Sticker(sticker));
            }

            Log.Info($"Нашел и актуализировал {Stickers.Count} стикеров");
        }

        /// <summary>
        /// Скачивает с YouGile всех сотрудников
        /// </summary>
        private async Task GetAllUsers()
        {
            Log.Info("Процесс закачки актуальных сотрудников начался...");

            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/users?limit=1000");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Не вышло получить список сотрудников с YouGile.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var rawUsers = JsonSerializer.Deserialize<UserListResponse>(content)!;

            foreach (var user in rawUsers.Content)
            {
                Users.TryAdd(user.Id, new User(user));
            }

            Log.Info($"Нашел и актуализировал {Users.Count} сотрудников");
        }

        /// <summary>
        /// Скачивает с YouGile все столбцы
        /// </summary>
        private async Task GetAllColumns()
        {
            Log.Info("Процесс закачки актуальных столбцов начался...");

            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/columns?limit=1000");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Не вышло получить список столбцов с YouGile.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var rawColumns = JsonSerializer.Deserialize<ColumnListResponse>(content)!;

            foreach (var column in rawColumns.Content)
            {
                var board = Boards[column.BoardId];
                Columns.TryAdd(column.Id, new Column(column, board));
            }

            Log.Info($"Нашел и актуализировал {Columns.Count} столбцов");
        }

        /// <summary>
        /// Скачивает с YouGile все доски
        /// </summary>
        private async Task GetAllBoards()
        {
            Log.Info("Процесс закачки актуальных досок начался...");

            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/boards?limit=1000");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Не вышло получить список досок с YouGile.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var rawBoards = JsonSerializer.Deserialize<BoardListResponse>(content)!;

            foreach (var board in rawBoards.Content)
            {
                var project = Projects[board.ProjectId];
                Boards.TryAdd(board.Id, new Board(board, project));
            }

            Log.Info($"Нашел и актуализировал {Boards.Count} досок");
        }

        /// <summary>
        /// Скачивает с YouGile все проекты
        /// </summary>
        private async Task GetAllProjects()
        {
            Log.Info("Процесс закачки актуальных проектов начался...");

            var response = await _httpClient.GetAsync($"{ApiBaseUrl}/projects?limit=1000");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Не вышло получить список проектов с YouGile.");
            }

            var content = await response.Content.ReadAsStringAsync();
            var rawProjects = JsonSerializer.Deserialize<ProjectListResponse>(content)!;

            foreach (var project in rawProjects.Content)
            {
                Projects.TryAdd(project.Id, new Project(project));
            }

            Log.Info($"Нашел и актуализировал {Projects.Count} проектов");
        }

        /// <summary>
        /// Регистрация обработчиков событий
        /// </summary>
        private void RegisterEvents()
        {
            EventSource.YouGileEventReceived += HandleYouGileEventOnYouGileEventReceivedAsync;
        }

        /// <summary>
        /// Общий обработчик событий с YouGile
        /// </summary>
        private async void HandleYouGileEventOnYouGileEventReceivedAsync(YouGileEventReceivedEventArgs ev)
        {
            switch (ev.ObjectType)
            {
                case ObjectType.Task:
                    await GetAndUpdateAllTasks();
                    await UpdatePublicationsAtForumsAsync();
                    break;
            }
        }

        /// <summary>
        /// Обновляет данные задач на всех сохраненных форумах
        /// </summary>
        private async Task UpdatePublicationsAtForumsAsync()
        {
            foreach (var project in Projects.Values)
            {
                if (project.PublicationChannel is null)
                {
                    Log.Warning($"У проекта {project.Title} не определен форум для публикаций");
                    continue;
                }

                await UpdatePublicationsAtForumAsync(project);
            }
        }

        /// <summary>
        /// Обновляет все публикации на форуме до их актуального состояния
        /// <param name="project">Проект, чьи задачи обновляются</param>
        /// </summary>
        public async Task UpdatePublicationsAtForumAsync(Project project)
        {
            if (project.PublicationChannel is null)
            {
                return;
            }

            Log.Info($"Обновление данных на сервере {project.PublicationChannel.Guild.Name} в канале {project.PublicationChannel.Name} для проекта {project.Title}.");

            var settings = Program.Config.YouGileConfig.ConnectionSettings[project.Title];
            var currentProjectTasks = Tasks.Values.Where(task => task.Column.Board.Project == project);

            foreach (var task in currentProjectTasks)
            {
                await task.LogChangesAsync();
                await task.CreateOrUpdatePublication(settings, _publicationStorageHandler);
            }

            Log.Info("Обновление окончено");
            await _publicationStorageHandler.SavePublicationsAsync();
        }
    }
}
