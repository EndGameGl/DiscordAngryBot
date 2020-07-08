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

namespace DiscordAngryBot
{
    /// <summary>
    /// Ядро бота, хранящее данные
    /// </summary>
    public class BotCore
    {
        /// <summary>
        /// Список гильдий и связанных с ними данных
        /// </summary>
        private static List<CustomGuildDataCache> customGuildDataCaches = new List<CustomGuildDataCache>();
        /// <summary>
        /// Сервер API бота
        /// </summary>
        private static APIServer apiServer = new APIServer();
        /// <summary>
        /// Клиент дискорда
        /// </summary>
        private static DiscordSocketClient discordSocketClient;
        /// <summary>
        /// Объект, содержащий настройки бота дискорда
        /// </summary>
        private static BotSettings settings = new BotSettings();

        /// <summary>
        /// Функция запуска бота
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) 
        {
            apiServer.ConfigServer("http://192.168.1.9:20001", new MediaTypeHeaderValue("text/html"), true);           
            MainAsync().GetAwaiter().GetResult(); 
        }
       
        /// <summary>
        /// Установка обработчиков и запуск серва
        /// </summary>
        /// <returns></returns>
        private async static Task MainAsync()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;
            await apiServer.RunAPIServer();
            await ValidateLocalFiles();
            discordSocketClient = new DiscordSocketClient(
                new DiscordSocketConfig 
                { 
                    LogLevel = LogSeverity.Info,
                    MessageCacheSize = 10                     
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
            await ConsoleWriter.WriteDivideMessage($"LoadGroups() finished: took {stopwatch.ElapsedMilliseconds} ms");
            return groups;
        }
        /// <summary>
        /// Загрузка всех банов
        /// </summary>
        /// <returns></returns>
        private static async Task<List<DiscordBan>> LoadGuildBans(SocketGuild guild)
        {
            List<DiscordBan> bans = new List<DiscordBan>();
            bans = await BanHandler.LoadBansFromGuildDB(guild);
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
        public static CustomGuildDataCache GetGuildDataCache(ulong guildID)
        {
            return customGuildDataCaches.Where(x => x.Guild.Id == guildID).FirstOrDefault();
        }
        /// <summary>
        /// Вернуть список настроек конкретного сервера
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        public static DiscordGuildSettings GetDiscordGuildSettings(ulong guildID)
        {
            return customGuildDataCaches.Where(x => x.Guild.Id == guildID).FirstOrDefault().Settings;
        }
        /// <summary>
        /// Вернуть список групп конкретного сервера
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        public static List<Group> GetDiscordGuildGroups(ulong guildID)
        {
            return customGuildDataCaches.Where(x => x.Guild.Id == guildID).FirstOrDefault().Groups;
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
        /// Список системных команд
        /// </summary>
        /// <returns></returns>
        public static List<string> SystemCommands()
        {
            return settings.systemCommands;
        }
        /// <summary>
        /// Список пользовательских команд
        /// </summary>
        /// <returns></returns>
        public static List<string> UserCommands()
        {
            return settings.userCommands;
        }
        /// <summary>
        /// Список остальных команд
        /// </summary>
        /// <returns></returns>
        public static List<string> OtherCommands()
        {
            return settings.otherCommands;
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
        public static List<string> MusicCommands()
        {
            return settings.musicCommands;
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
                    GetGuildDataCache(guild.Id).IsAvailable = true;
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
    }
}
