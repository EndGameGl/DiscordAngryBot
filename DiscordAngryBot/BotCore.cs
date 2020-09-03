using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Bans;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.Groups;
using DiscordAngryBot.APIHandlers;
using DiscordAngryBot.GatewayEventHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using DiscordAngryBot.CustomObjects.Filters;
using System.Diagnostics;
using DiscordAngryBot.CustomObjects.Caches;
using DiscordAngryBot.CustomObjects.SQLIteHandler;
using Newtonsoft.Json;
using System.Text;
using DiscordAngryBot.CustomObjects.Logs;
using DiscordAngryBot.CustomObjects.DiscordCommands;
using static DiscordAngryBot.MessageHandlers.CommandHandler;
using static DiscordAngryBot.ReactionHandlers.ReactionHandler;
using System.Reflection;

namespace DiscordAngryBot
{
    /// <summary>
    /// Ядро бота, хранящее данные
    /// </summary>
    public class BotCore
    {
        /// <summary>
        /// Логи для вывода на сайт
        /// </summary>
        private static List<DataLog> dataLogs = new List<DataLog>();
        /// <summary>
        /// Список гильдий и связанных с ними данных
        /// </summary>
        private static List<CustomGuildDataCache> customGuildDataCaches = new List<CustomGuildDataCache>();
        /// <summary>
        /// Сервер API бота
        /// </summary>
        private static APIServer apiServer;
        /// <summary>
        /// Клиент дискорда
        /// </summary>
        private static DiscordSocketClient discordSocketClient;
        /// <summary>
        /// Объект, содержащий настройки бота дискорда
        /// </summary>
        private static BotSettings settings = new BotSettings();
        /// <summary>
        /// Рандом, используемый в боте
        /// </summary>
        public static Random Random = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// Функция запуска бота
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) 
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;
            //apiServer = new APIServer("http://192.168.1.9:20001", new MediaTypeHeaderValue("text/html"));
            RegisterCommands();
            MainAsync().GetAwaiter().GetResult(); 
        }
       
        /// <summary>
        /// Установка обработчиков и запуск серва
        /// </summary>
        /// <returns></returns>
        private async static Task MainAsync()
        {          
            //await apiServer.RunAPIServer();
            await ValidateLocalFiles();
            discordSocketClient = new DiscordSocketClient(
                new DiscordSocketConfig 
                { 
                    LogLevel = LogSeverity.Info,
                    MessageCacheSize = 10,                                      
                });
            AddHandlersToClient(discordSocketClient);
            await discordSocketClient.LoginAsync(TokenType.Bot, File.ReadAllText(@"Token\Token.txt"));
            await discordSocketClient.StartAsync();
            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// Настройка бота для серверов
        /// </summary>
        /// <returns></returns>
        private static async Task SetUpBotData()
        {
            foreach (var guildCache in customGuildDataCaches)
            {
                await RunGuildLoaders(guildCache);
            }
        }
        /// <summary>
        /// Загрузка всех данных для гильдии
        /// </summary>
        /// <param name="guildCache"></param>
        /// <returns></returns>
        private static async Task RunGuildLoaders(CustomGuildDataCache guildCache)
        {
            guildCache.Settings = await LoadGuildSettings(guildCache.Guild.Id);
            guildCache.Bans = await LoadGuildBans(guildCache.Guild);
            guildCache.Groups = await LoadGuildGroups(guildCache.Guild);
            GroupHandler.ActualizeReactionsOnGroups(guildCache.Guild).GetAwaiter().GetResult();
            File.WriteAllText($"locals/Databases/{guildCache.Guild.Id}/GuildInfo.txt", await CollectGuildInfo(guildCache.Guild));
        }
        /// <summary>
        /// Загрузка всех составов
        /// </summary>
        /// <returns></returns>
        private static async Task<List<Group>> LoadGuildGroups(SocketGuild guild)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<Group> groups = new List<Group>();           
            groups = GroupHandler.LoadAllGroupsFromDB(guild).GetAwaiter().GetResult();             
            stopwatch.Stop();
            await CustomObjects.ConsoleOutput.Debug.WriteDivideMessage($"LoadGroups() finished: took {stopwatch.ElapsedMilliseconds} ms");
            return groups;
        }
        /// <summary>
        /// Загрузка всех банов
        /// </summary>
        /// <returns></returns>
        private static async Task<List<DiscordBan>> LoadGuildBans(SocketGuild guild)
        {
            List<DiscordBan> bans = new List<DiscordBan>();
            bans = await BanHandler.LoadBansFromGuildDB(guild.Id);
            return bans;
        } 
        /// <summary>
        /// Добавка всех обработчиков событий клиента дискорда
        /// </summary>
        /// <param name="_client"></param>
        private static void AddHandlersToClient(DiscordSocketClient _client)
        {           
            _client.Log += GatewayEventHandler.LogHandler;         
            _client.MessageReceived += GatewayEventHandler.MessageReceivedHandler;           
            _client.ReactionAdded += GatewayEventHandler.ReactionAddedHandler;         
            _client.ReactionRemoved += GatewayEventHandler.ReactionRemovedHandler;          
            _client.Ready += SetUpBotData;
            _client.ChannelCreated += GatewayEventHandler.ChannelCreatedHandler;
            _client.ChannelDestroyed += GatewayEventHandler.ChannelDestroyedHandler;
            _client.ChannelUpdated += GatewayEventHandler.ChannelUpdatedHandler;
            _client.Connected += GatewayEventHandler.ConnectedHandler;
            _client.CurrentUserUpdated += GatewayEventHandler.CurrentUserUpdatedHandler;
            _client.Disconnected += GatewayEventHandler.DisconnectedHandler;
            _client.GuildAvailable += GatewayEventHandler.GuildAvailableHandler;
            _client.GuildMembersDownloaded += GatewayEventHandler.GuildMembersDownloadedHandler;
            _client.GuildMemberUpdated += GatewayEventHandler.GuildMemberUpdatedHandler;
            _client.GuildUnavailable += GatewayEventHandler.GuildUnavailableHandler;
            _client.GuildUpdated += GatewayEventHandler.GuildUpdatedHandler;
            _client.JoinedGuild += GatewayEventHandler.JoinedGuildHandler;
            _client.LatencyUpdated += GatewayEventHandler.LatencyUpdatedHandler;
            _client.LeftGuild += GatewayEventHandler.LeftGuildHandler;
            _client.LoggedIn += GatewayEventHandler.LoggedInHandler;
            _client.LoggedOut += GatewayEventHandler.LoggedOutHandler;
            _client.MessageDeleted += GatewayEventHandler.MessageDeletedHandler;
            _client.MessagesBulkDeleted += GatewayEventHandler.MessagesBulkDeletedHandler;
            _client.MessageUpdated += GatewayEventHandler.MessageUpdatedHandler;
            _client.ReactionsCleared += GatewayEventHandler.ReactionsClearedHandler;
            _client.RecipientAdded += GatewayEventHandler.RecipientAddedHandler;
            _client.RecipientRemoved += GatewayEventHandler.RecipientRemovedHandler;
            _client.RoleCreated += GatewayEventHandler.RoleCreatedHandler;
            _client.RoleDeleted += GatewayEventHandler.RoleDeletedHandler;
            _client.RoleUpdated += GatewayEventHandler.RoleUpdatedHandler;
            _client.UserBanned += GatewayEventHandler.UserBannedHandler;
            _client.UserIsTyping += GatewayEventHandler.UserIsTypingHandler;
            _client.UserJoined += GatewayEventHandler.UserJoinedHandler;
            _client.UserLeft += GatewayEventHandler.UserLeftHandler;
            _client.UserUnbanned += GatewayEventHandler.UserUnbannedHandler;
            _client.UserUpdated += GatewayEventHandler.UserUpdatedHandler;
            _client.UserVoiceStateUpdated += GatewayEventHandler.UserVoiceStateUpdatedHandler;
            _client.VoiceServerUpdated += GatewayEventHandler.VoiceServerUpdatedHandler;
        }


        /// <summary>
        /// Получение кэша гильдии
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static bool TryGetGuildDataCache(ulong guildID, out CustomGuildDataCache guildCache)
        {
            guildCache = customGuildDataCaches.Where(x => x.Guild.Id == guildID).FirstOrDefault();
            if (guildCache != null)
                return true;
            else
                return false;
        }
        /// <summary>
        /// Вернуть список настроек конкретного сервера
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        public static bool TryGetDiscordGuildSettings(ulong guildID, out DiscordGuildSettings settings)
        {
            settings = null;
            if (TryGetGuildDataCache(guildID, out CustomGuildDataCache cache))
            {
                settings = cache.Settings;
                if (settings != null)
                    return true;
                else
                    return false;
            }
            else
                return false;
            
        }
        /// <summary>
        /// Вернуть список групп конкретного сервера
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        public static bool TryGetDiscordGuildGroups(ulong guildID, out List<Group> groups)
        {
            groups = null;
            if (TryGetGuildDataCache(guildID, out CustomGuildDataCache cache))
            {
                groups = cache.Groups;
                return true;
            }
            else 
                return false;
        }
        /// <summary>
        /// Вернуть список банов конкретного сервера
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        public static List<DiscordBan> GetDiscordGuildBans(ulong guildID)
        {
            return customGuildDataCaches.Where(x => x.Guild.Id == guildID).FirstOrDefault().Bans;
        }
        /// <summary>
        /// Вернуть список счетчиков мата конкретного сервера
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        public static List<SwearCounter> GetDiscordGuildSwearCounters(ulong guildID)
        {
            return customGuildDataCaches.Where(x => x.Guild.Id == guildID).FirstOrDefault().SwearCounters;
        }
        /// <summary>
        /// Вернуть логи команд/ошибок, произошедших за время работы бота
        /// </summary>
        /// <returns></returns>
        public static List<DataLog> GetDataLogs()
        {
            return dataLogs;
        }


        /// <summary>
        /// Список системных команд
        /// </summary>
        /// <returns></returns>
        public static List<DiscordCommand> SystemCommands()
        {
            return settings.Commands.Where(x => x.CommandMetadata.Category == CommandCategory.System).ToList();
        }
        /// <summary>
        /// Список пользовательских команд
        /// </summary>
        /// <returns></returns>
        public static List<DiscordCommand> UserCommands()
        {
            return settings.Commands.Where(x => x.CommandMetadata.Category == CommandCategory.User).ToList();
        }
        /// <summary>
        /// Список остальных команд
        /// </summary>
        /// <returns></returns>
        public static List<DiscordCommand> OtherCommands()
        {
            return settings.Commands.Where(x => x.CommandMetadata.Category == CommandCategory.Other).ToList();
        }
        /// <summary>
        /// Список матов
        /// </summary>
        /// <returns></returns>
        public static List<string> Swears()
        {
            return settings.swearFilterWords;
        }
        /// <summary>
        /// Список запрещенных команд
        /// </summary>
        /// <returns></returns>
        public static List<string> ForbiddenCommands()
        {
            return settings.forbiddenCommands;
        }
        /// <summary>
        /// Список музыкальных команд
        /// </summary>
        /// <returns></returns>
        public static List<DiscordCommand> MusicCommands()
        {
            return settings.Commands.Where(x => x.CommandMetadata.Category == CommandCategory.Music).ToList();
        }
        /// <summary>
        /// Клиент бота дискорда
        /// </summary>
        /// <returns></returns>
        public static DiscordSocketClient GetClient()
        {
            return discordSocketClient;
        }
        /// <summary>
        /// Поиск команды для запуска
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns></returns>
        public static bool TryGetCommand(string commandName, out DiscordCommand command)
        {
            command = settings.Commands.FirstOrDefault(x => x.CommandMetadata.CommandName == commandName);
            if (command != null)
                return true;
            else
                return false;
        }
        public static HashSet<DiscordCommand> GetAllCommands()
        {
            return settings.Commands;
        }

        /// <summary>
        /// Загрузка настроек гильдии
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        private static async Task<DiscordGuildSettings> LoadGuildSettings(ulong guildID)
        {
            var data = await SQLiteDataManager.GetDataFromDB("locals/GuildSettings.sqlite", $"SELECT * FROM Settings WHERE GuildID = '{guildID}'");
            var settingsJSON = data.Rows[0]["SettingsJSON"].ToString();
            return JsonConvert.DeserializeObject<DiscordGuildSettings>(settingsJSON);
        }
        /// <summary>
        /// Добавление нового кэша гильдии
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task CreateGuildCache(SocketGuild guild)
        {
            await Task.Run(async () =>
            {
                CustomGuildDataCache cache = null;
                if (customGuildDataCaches.Where(x=> x.Guild.Id == guild.Id).Count() == 0)
                {
                    cache = new CustomGuildDataCache()
                    {
                        Guild = guild,
                        Bans = new List<DiscordBan>(),
                        Groups = new List<Group>(),
                        SwearCounters = new List<SwearCounter>(),
                        Settings = new DiscordGuildSettings()
                        {
                            adminsID = new List<ulong>() { 261497385274966026 },
                            APIToken = null,
                            BanRoleID = null,
                            CommandPrefix = '_',
                            NewsChannelID = null,
                            IsSwearFilterEnabled = null
                        },
                        IsAvailable = true
                    };
                    customGuildDataCaches.Add(cache);
                    await PrepareGuildCache(cache);
                    await SQLiteDataManager.PushToDB("locals/GuildSettings.sqlite", $"INSERT OR IGNORE INTO Settings (GuildID, SettingsJSON) VALUES ('{cache.Guild.Id}', '{JsonConvert.SerializeObject(cache.Settings)}')");
                }
                else
                {
                    if (TryGetGuildDataCache(guild.Id, out var guildCache))
                    {
                        guildCache.IsAvailable = true;
                    }
                }                              
            });
        }
        /// <summary>
        /// Удаление кэша гильдии
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task RemoveGuildCache(SocketGuild guild)
        {
            await Task.Run(() =>
            {
                customGuildDataCaches.Where(x => x.Guild.Id == guild.Id).FirstOrDefault().IsAvailable = false;
            });
        }
        /// <summary>
        /// Проверка локальных файлов
        /// </summary>
        /// <returns></returns>
        private static async Task ValidateLocalFiles()
        {
            if (!Directory.Exists("locals"))
            {
                Directory.CreateDirectory("locals");
            }

            if (!File.Exists("locals/GuildSettings.sqlite"))
            {
                await SQLiteDataManager.CreateDataBase("locals/GuildSettings.sqlite");
                await SQLiteDataManager.PushToDB("locals/GuildSettings.sqlite", "CREATE TABLE IF NOT EXISTS Settings(GuildID INTEGER PRIMARY KEY, SettingsJSON TEXT)");
            }
            if (!Directory.Exists("locals/Databases"))
            {
                Directory.CreateDirectory("locals/Databases");
            }
        }
        /// <summary>
        /// Подготовка кэша для гильдии
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        private static async Task PrepareGuildCache(CustomGuildDataCache cache)
        {
            await Task.Run(async () =>
            {
                if (!Directory.Exists($"locals/Databases/{cache.Guild.Id}"))
                {
                    Directory.CreateDirectory($"locals/Databases/{cache.Guild.Id}");
                }
                if (!File.Exists($"locals/Databases/{cache.Guild.Id}/Bans.sqlite"))
                {
                    await SQLiteDataManager.CreateDataBase($"locals/Databases/{cache.Guild.Id}/Bans.sqlite");
                    await SQLiteDataManager.PushToDB($"locals/Databases/{cache.Guild.Id}/Bans.sqlite", "CREATE TABLE IF NOT EXISTS Bans(GUID TEXT PRIMARY KEY, BanJSON TEXT)");
                }
                if (!File.Exists($"locals/Databases/{cache.Guild.Id}/Groups.sqlite"))
                {
                    await SQLiteDataManager.CreateDataBase($"locals/Databases/{cache.Guild.Id}/Groups.sqlite");
                    await SQLiteDataManager.PushToDB($"locals/Databases/{cache.Guild.Id}/Groups.sqlite", "CREATE TABLE IF NOT EXISTS Groups(GUID TEXT PRIMARY KEY, GroupJSON TEXT)");
                }

            });
        }
        /// <summary>
        /// Префикс для команд конкретного сервера
        /// </summary>
        /// <returns></returns>
        public static char GetGuildCommandPrefix(ulong GuildID)
        {
            var prefix = customGuildDataCaches.Where(x => x.Guild.Id == GuildID).FirstOrDefault().Settings.CommandPrefix;
            if (prefix == null)
                return settings.defaultCommandPrefix;
            else
                return prefix.Value;
        }
        /// <summary>
        /// Исходный префикс для команд
        /// </summary>
        /// <returns></returns>
        public static char GetDefaultCommandPrefix()
        {
            return settings.defaultCommandPrefix;
        }
        /// <summary>
        /// Сбор краткой сводки о работе гильдии
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task<string> CollectGuildInfo(SocketGuild guild)
        {
            return await Task.Run(() => {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Server name: {guild.Name}");
                sb.AppendLine($"Server ID: {guild.Id}");
                sb.AppendLine($"People count: {guild.Users.Count}");
                sb.AppendLine($"Channels count: {guild.Channels.Count}");
                sb.AppendLine($"Roles count: {guild.Roles.Count}");
                sb.AppendLine($"AFK Channel: {guild.AFKChannel}");
                sb.AppendLine($"AFK Timeout: {guild.AFKTimeout}");
                sb.AppendLine($"Created at: {guild.CreatedAt}");
                sb.AppendLine($"Default channel: {guild.DefaultChannel.Name}");
                sb.AppendLine($"Emotes count: {guild.Emotes.Count}");
                sb.AppendLine($"Owner name: {guild.Owner.Nickname}");
                sb.AppendLine($"Preferred locale: {guild.PreferredLocale}");
                sb.AppendLine($"Voice region: {guild.VoiceRegionId}");
                return sb.ToString();
            });
        }
        /// <summary>
        /// Сбор данных о методах и регистрация команд в боте
        /// </summary>
        private static void RegisterCommands()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CustomObjects.ConsoleOutput.Debug.Log($"Getting command metadata...", CustomObjects.ConsoleOutput.Debug.InfoType.CommandInfo).GetAwaiter().GetResult();

            var systemMethods = typeof(SystemCommands).GetMethods();
            CustomObjects.ConsoleOutput.Debug.Log($"Found {systemMethods.Length} system commands.", CustomObjects.ConsoleOutput.Debug.InfoType.CommandInfo).GetAwaiter().GetResult();
            var userMethods = typeof(UserCommands).GetMethods();
            CustomObjects.ConsoleOutput.Debug.Log($"Found {userMethods.Length} user commands", CustomObjects.ConsoleOutput.Debug.InfoType.CommandInfo).GetAwaiter().GetResult();
            var otherMethods = typeof(OtherCommands).GetMethods();
            CustomObjects.ConsoleOutput.Debug.Log($"Found {otherMethods.Length} other commands", CustomObjects.ConsoleOutput.Debug.InfoType.CommandInfo).GetAwaiter().GetResult();
            var musicMethods = typeof(MusicCommands).GetMethods();
            CustomObjects.ConsoleOutput.Debug.Log($"Found {musicMethods.Length} music commands", CustomObjects.ConsoleOutput.Debug.InfoType.CommandInfo).GetAwaiter().GetResult();
            var emojiMethods = typeof(PartyReactionHandler).GetMethods();
            CustomObjects.ConsoleOutput.Debug.Log($"Found {emojiMethods.Length} emoji handlers", CustomObjects.ConsoleOutput.Debug.InfoType.CommandInfo).GetAwaiter().GetResult();

            CustomObjects.ConsoleOutput.Debug.Log($"Registering command handlers...", CustomObjects.ConsoleOutput.Debug.InfoType.CommandInfo).GetAwaiter().GetResult();
            foreach (var systemMethod in systemMethods)
            {
                var attribute = systemMethod.GetCustomAttribute<CustomCommandAttribute>();
                if (attribute != null)
                {
                    settings.RegisterCommand(new DiscordCommand() 
                    {  
                        CommandMetadata = attribute, 
                        Method = systemMethod 
                    });
                }
            }
            foreach (var userMethod in userMethods)
            {
                var attribute = userMethod.GetCustomAttribute<CustomCommandAttribute>();
                if (attribute != null)
                {
                    settings.RegisterCommand(new DiscordCommand()
                    {
                        CommandMetadata = attribute,
                        Method = userMethod
                    });
                }
            }
            foreach (var otherMethod in otherMethods)
            {
                var attribute = otherMethod.GetCustomAttribute<CustomCommandAttribute>();
                if (attribute != null)
                {
                    settings.RegisterCommand(new DiscordCommand()
                    {
                        CommandMetadata = attribute,
                        Method = otherMethod
                    });
                }
            }
            foreach (var musicMethod in musicMethods)
            {
                var attribute = musicMethod.GetCustomAttribute<CustomCommandAttribute>();
                if (attribute != null)
                {
                    settings.RegisterCommand(new DiscordCommand()
                    {
                        CommandMetadata = attribute,
                        Method = musicMethod
                    });
                }
            }
            foreach (var emojiMethod in emojiMethods)
            {
                var attribute = emojiMethod.GetCustomAttribute<CustomCommandAttribute>();
                if (attribute != null)
                {
                    settings.RegisterCommand(new DiscordCommand()
                    {
                        CommandMetadata = attribute,
                        Method = emojiMethod
                    });
                }
            }

            stopwatch.Stop();
            CustomObjects.ConsoleOutput.Debug.Log($"Finished registering commands. [Took {stopwatch.Elapsed} ms to register]", CustomObjects.ConsoleOutput.Debug.InfoType.CommandInfo).GetAwaiter().GetResult();
        }
    }
}
