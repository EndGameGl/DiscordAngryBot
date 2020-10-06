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
using Debug = DiscordAngryBot.CustomObjects.ConsoleOutput.Debug;

namespace DiscordAngryBot
{
    /// <summary>
    /// Bot core for all processes
    /// </summary>
    public class BotCore
    {
        /// <summary>
        /// Logs for latest actions
        /// </summary>
        private static List<DataLog> dataLogs = new List<DataLog>();

        /// <summary>
        /// Guild caches
        /// </summary>
        private static List<ExtendedDiscordGuildData> customGuildDataCaches = new List<ExtendedDiscordGuildData>();

        /// <summary>
        /// API server
        /// </summary>
        private static APIServer apiServer;

        /// <summary>
        /// Discord socket client
        /// </summary>
        private static DiscordSocketClient discordSocketClient;

        /// <summary>
        /// Global bot settings
        /// </summary>
        private static BotSettings settings = new BotSettings();

        /// <summary>
        /// Random class object instance
        /// </summary>
        public static Random Random = new Random(Guid.NewGuid().GetHashCode());

        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args">keys</param>
        static void Main(string[] args) 
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;

            apiServer = new APIServer(
                serverAddress: "http://192.168.1.56:20001", 
                mediaTypeHeaderValue: new MediaTypeHeaderValue("text/html"));

            RegisterCommands();
            MainAsync().GetAwaiter().GetResult(); 
        }
       
        /// <summary>
        /// Main async method
        /// </summary>
        /// <returns></returns>
        private async static Task MainAsync()
        {          
            await apiServer.RunAPIServer();
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
        /// Sets up guild caches
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
        /// Loads all data for current guild
        /// </summary>
        /// <param name="guildData">Guild cache</param>
        /// <returns></returns>
        private static async Task RunGuildLoaders(ExtendedDiscordGuildData guildData)
        {
            guildData.UseSettings(await LoadGuildSettings(guildData.Guild.Id));
            guildData.UseExistingListOfBans(await LoadGuildBans(guildData.Guild));
            guildData.UseExistingListOfGroups(await LoadGuildGroups(guildData.Guild));
            GroupHandler.ActualizeReactionsOnGroups(guildData.Guild);
            File.WriteAllText(
                path: $"locals/Databases/{guildData.Guild.Id}/GuildInfo.txt", 
                contents: await CollectGuildInfo(guildData.Guild));
        }

        /// <summary>
        /// Loads all guild groups
        /// </summary>
        /// <param name="guild">Guild</param>
        /// <returns></returns>
        private static async Task<List<Group>> LoadGuildGroups(SocketGuild guild)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            List<Group> groups = new List<Group>();           
            groups = GroupHandler.LoadAllGroupsFromDB(guild.Id).GetAwaiter().GetResult();             
            stopwatch.Stop();
            await Debug.WriteDivideMessage($"LoadGroups() finished: took {stopwatch.ElapsedMilliseconds} ms");
            return groups;
        }

        /// <summary>
        /// Loads all guild bans
        /// </summary>
        /// <param name="guild">Guild</param>
        /// <returns></returns>
        private static async Task<List<DiscordBan>> LoadGuildBans(SocketGuild guild)
        {
            List<DiscordBan> bans = new List<DiscordBan>();
            bans = await BanHandler.LoadBansFromGuildDB(guild.Id);
            return bans;
        } 

        /// <summary>
        /// Add all handlers to client
        /// </summary>
        /// <param name="client">Discord Socket Client</param>
        private static void AddHandlersToClient(DiscordSocketClient client)
        {           
            client.Log += GatewayEventHandler.LogHandler;         
            client.MessageReceived += GatewayEventHandler.MessageReceivedHandler;           
            client.ReactionAdded += GatewayEventHandler.ReactionAddedHandler;         
            client.ReactionRemoved += GatewayEventHandler.ReactionRemovedHandler;          
            client.Ready += SetUpBotData;
            client.ChannelCreated += GatewayEventHandler.ChannelCreatedHandler;
            client.ChannelDestroyed += GatewayEventHandler.ChannelDestroyedHandler;
            client.ChannelUpdated += GatewayEventHandler.ChannelUpdatedHandler;
            client.Connected += GatewayEventHandler.ConnectedHandler;
            client.CurrentUserUpdated += GatewayEventHandler.CurrentUserUpdatedHandler;
            client.Disconnected += GatewayEventHandler.DisconnectedHandler;
            client.GuildAvailable += GatewayEventHandler.GuildAvailableHandler;
            client.GuildMembersDownloaded += GatewayEventHandler.GuildMembersDownloadedHandler;
            client.GuildMemberUpdated += GatewayEventHandler.GuildMemberUpdatedHandler;
            client.GuildUnavailable += GatewayEventHandler.GuildUnavailableHandler;
            client.GuildUpdated += GatewayEventHandler.GuildUpdatedHandler;
            client.JoinedGuild += GatewayEventHandler.JoinedGuildHandler;
            client.LatencyUpdated += GatewayEventHandler.LatencyUpdatedHandler;
            client.LeftGuild += GatewayEventHandler.LeftGuildHandler;
            client.LoggedIn += GatewayEventHandler.LoggedInHandler;
            client.LoggedOut += GatewayEventHandler.LoggedOutHandler;
            client.MessageDeleted += GatewayEventHandler.MessageDeletedHandler;
            client.MessagesBulkDeleted += GatewayEventHandler.MessagesBulkDeletedHandler;
            client.MessageUpdated += GatewayEventHandler.MessageUpdatedHandler;
            client.ReactionsCleared += GatewayEventHandler.ReactionsClearedHandler;
            client.RecipientAdded += GatewayEventHandler.RecipientAddedHandler;
            client.RecipientRemoved += GatewayEventHandler.RecipientRemovedHandler;
            client.RoleCreated += GatewayEventHandler.RoleCreatedHandler;
            client.RoleDeleted += GatewayEventHandler.RoleDeletedHandler;
            client.RoleUpdated += GatewayEventHandler.RoleUpdatedHandler;
            client.UserBanned += GatewayEventHandler.UserBannedHandler;
            client.UserIsTyping += GatewayEventHandler.UserIsTypingHandler;
            client.UserJoined += GatewayEventHandler.UserJoinedHandler;
            client.UserLeft += GatewayEventHandler.UserLeftHandler;
            client.UserUnbanned += GatewayEventHandler.UserUnbannedHandler;
            client.UserUpdated += GatewayEventHandler.UserUpdatedHandler;
            client.UserVoiceStateUpdated += GatewayEventHandler.UserVoiceStateUpdatedHandler;
            client.VoiceServerUpdated += GatewayEventHandler.VoiceServerUpdatedHandler;
        }


        /// <summary>
        /// Try to get guild cache by ID
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <param name="guildCache">Guild cache, if found any</param>
        /// <returns></returns>
        public static bool TryGetExtendedDiscordGuildBotData(ulong guildID, out ExtendedDiscordGuildData guildCache)
        {
            guildCache = customGuildDataCaches.Where(x => x.Guild.Id == guildID).FirstOrDefault();
            if (guildCache != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Try to get discord guild settings by ID
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <param name="settings">Guild settings, if found any</param>
        /// <returns></returns>
        public static bool TryGetDiscordGuildSettings(ulong guildID, out DiscordGuildBotSettings settings)
        {
            settings = null;
            if (TryGetExtendedDiscordGuildBotData(guildID, out ExtendedDiscordGuildData cache))
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
        /// Try to get guild groups by ID
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <param name="groups">Groups, if found any</param>
        /// <returns></returns>
        public static bool TryGetDiscordGuildGroups(ulong guildID, out List<Group> groups)
        {
            groups = null;
            if (TryGetExtendedDiscordGuildBotData(guildID, out ExtendedDiscordGuildData cache))
            {
                groups = cache.Groups;
                return true;
            }
            else 
                return false;
        }

        /// <summary>
        /// Get guild bans
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <returns></returns>
        public static List<DiscordBan> GetDiscordGuildBans(ulong guildID)
        {
            return customGuildDataCaches.Where(x => x.Guild.Id == guildID).FirstOrDefault().Bans;
        }

        /// <summary>
        /// Get guild swear counters
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <returns></returns>
        public static List<SwearCounter> GetDiscordGuildSwearCounters(ulong guildID)
        {
            return customGuildDataCaches.Where(x => x.Guild.Id == guildID).FirstOrDefault().SwearCounters;
        }

        /// <summary>
        /// Get all datalogs 
        /// </summary>
        /// <returns></returns>
        public static List<DataLog> GetDataLogs()
        {
            return dataLogs;
        }

        /// <summary>
        /// Get list of filtered swears
        /// </summary>
        /// <returns></returns>
        public static List<string> Swears()
        {
            return settings.swearFilterWords;
        }

        /// <summary>
        /// Get list of forbidden shiro commands
        /// </summary>
        /// <returns></returns>
        public static List<string> ForbiddenCommands()
        {
            return settings.forbiddenCommands;
        }

        /// <summary>
        /// Get bot client
        /// </summary>
        /// <returns></returns>
        public static DiscordSocketClient GetClient()
        {
            return discordSocketClient;
        }

        /// <summary>
        /// Try get command for running
        /// </summary>
        /// <param name="commandName">Command name</param>
        /// <param name="command">Command, if found any</param>
        /// <returns></returns>
        public static bool TryGetCommand(string commandName, out DiscordCommand command)
        {
            command = settings.Commands.FirstOrDefault(x => x.CommandMetadata.CommandName == commandName);
            if (command != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Get all commands
        /// </summary>
        /// <returns></returns>
        public static HashSet<DiscordCommand> GetAllCommands()
        {
            return settings.Commands;
        }

        /// <summary>
        /// Load guild settings by ID
        /// </summary>
        /// <param name="guildID"></param>
        /// <returns></returns>
        private static async Task<DiscordGuildBotSettings> LoadGuildSettings(ulong guildID)
        {
            var data = await SQLiteExtensions.GetDataFromDB("locals/GuildSettings.sqlite", $"SELECT * FROM Settings WHERE GuildID = '{guildID}'");
            var settingsJSON = data.Rows[0]["SettingsJSON"].ToString();
            return JsonConvert.DeserializeObject<DiscordGuildBotSettings>(settingsJSON);
        }

        /// <summary>
        /// Create guild cache
        /// </summary>
        /// <param name="guild">Guild</param>
        /// <returns></returns>
        public static async Task CreateGuildCache(SocketGuild guild)
        {
            await Task.Run(async () =>
            {               
                if (!customGuildDataCaches.Any(x => x.Guild.Id == guild.Id))
                {
                    ExtendedDiscordGuildData cache = new ExtendedDiscordGuildData(guild: guild, isAvailable: true);
                    customGuildDataCaches.Add(cache);
                    await PrepareGuildCache(cache);
                    await SQLiteExtensions.PushToDB("locals/GuildSettings.sqlite", $"INSERT OR IGNORE INTO Settings (GuildID, SettingsJSON) VALUES ('{cache.Guild.Id}', '{JsonConvert.SerializeObject(cache.Settings)}')");
                }
                else
                {
                    if (TryGetExtendedDiscordGuildBotData(guild.Id, out var guildCache))
                    {
                        guildCache.SetAvailable();
                    }
                }                              
            });
        }

        /// <summary>
        /// Remove guild cache
        /// </summary>
        /// <param name="guild">Guild</param>
        /// <returns></returns>
        public static async Task RemoveGuildCache(SocketGuild guild)
        {
            await Task.Run(() =>
            {
                customGuildDataCaches.FirstOrDefault(x => x.Guild.Id == guild.Id)?.SetUnavailable();
            });
        }

        /// <summary>
        /// Validate all local files
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
                await SQLiteExtensions.CreateDataBase("locals/GuildSettings.sqlite");
                await SQLiteExtensions.PushToDB("locals/GuildSettings.sqlite", "CREATE TABLE IF NOT EXISTS Settings(GuildID INTEGER PRIMARY KEY, SettingsJSON TEXT)");
            }
            if (!Directory.Exists("locals/Databases"))
            {
                Directory.CreateDirectory("locals/Databases");
            }
        }

        /// <summary>
        /// Prepare all guild files
        /// </summary>
        /// <param name="cache">Guild cache</param>
        /// <returns></returns>
        private static async Task PrepareGuildCache(ExtendedDiscordGuildData cache)
        {
            await Task.Run(async () =>
            {
                if (!Directory.Exists($"locals/Databases/{cache.Guild.Id}"))
                {
                    Directory.CreateDirectory($"locals/Databases/{cache.Guild.Id}");
                }
                if (!File.Exists($"locals/Databases/{cache.Guild.Id}/Bans.sqlite"))
                {
                    await SQLiteExtensions.CreateDataBase($"locals/Databases/{cache.Guild.Id}/Bans.sqlite");
                    await SQLiteExtensions.PushToDB($"locals/Databases/{cache.Guild.Id}/Bans.sqlite", "CREATE TABLE IF NOT EXISTS Bans(GUID TEXT PRIMARY KEY, BanJSON TEXT)");
                }
                if (!File.Exists($"locals/Databases/{cache.Guild.Id}/Groups.sqlite"))
                {
                    await SQLiteExtensions.CreateDataBase($"locals/Databases/{cache.Guild.Id}/Groups.sqlite");
                    await SQLiteExtensions.PushToDB($"locals/Databases/{cache.Guild.Id}/Groups.sqlite", "CREATE TABLE IF NOT EXISTS Groups(GUID TEXT PRIMARY KEY, GroupJSON TEXT)");
                }

            });
        }

        /// <summary>
        /// Get guild command prefix
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
        /// Get default command prefix
        /// </summary>
        /// <returns></returns>
        public static char GetDefaultCommandPrefix()
        {
            return settings.defaultCommandPrefix;
        }

        /// <summary>
        /// Collect guild summary
        /// </summary>
        /// <param name="guild">Guild</param>
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
        /// Register all bot commands
        /// </summary>
        private static void RegisterCommands()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            CustomObjects.ConsoleOutput.Debug.Log($"Getting command metadata...", LogInfoType.CommandInfo).GetAwaiter().GetResult();

            var systemMethods = typeof(SystemCommands).GetMethods();
            CustomObjects.ConsoleOutput.Debug.Log($"Found {systemMethods.Length} system commands.", LogInfoType.CommandInfo).GetAwaiter().GetResult();
            var userMethods = typeof(UserCommands).GetMethods();
            CustomObjects.ConsoleOutput.Debug.Log($"Found {userMethods.Length} user commands", LogInfoType.CommandInfo).GetAwaiter().GetResult();
            var otherMethods = typeof(OtherCommands).GetMethods();
            CustomObjects.ConsoleOutput.Debug.Log($"Found {otherMethods.Length} other commands", LogInfoType.CommandInfo).GetAwaiter().GetResult();
            var musicMethods = typeof(MusicCommands).GetMethods();
            CustomObjects.ConsoleOutput.Debug.Log($"Found {musicMethods.Length} music commands", LogInfoType.CommandInfo).GetAwaiter().GetResult();
            var emojiMethods = typeof(GroupReactionHandler).GetMethods();
            CustomObjects.ConsoleOutput.Debug.Log($"Found {emojiMethods.Length} emoji handlers", LogInfoType.CommandInfo).GetAwaiter().GetResult();

            CustomObjects.ConsoleOutput.Debug.Log($"Registering command handlers...", LogInfoType.CommandInfo).GetAwaiter().GetResult();
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
            CustomObjects.ConsoleOutput.Debug.Log($"Finished registering commands. [Took {stopwatch.Elapsed} ms to register]", LogInfoType.CommandInfo).GetAwaiter().GetResult();
        }
    }
}
