using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Bans;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.Groups;
using DiscordAngryBot.APIHandlers;
using DiscordAngryBot.EventHandlers;
using DiscordAngryBot.GUI;
using DiscordAngryBot.MessageHandlers;
using DiscordAngryBot.ReactionHandlers;
using DiscordAngryBot.WebhookEventHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http.Headers;
using DiscordAngryBot.CustomObjects.Filters;

namespace DiscordAngryBot
{
    public class Program
    {
        static APIServer apiServer = new APIServer();
        public static List<SwearCounter> swearCounters = new List<SwearCounter>();
        //public EventHandlers.EventHandler eventHandler = new EventHandlers.EventHandler();
        static void Main(string[] args) 
        {
            apiServer.ConfigServer("http://192.168.1.9:20001", new MediaTypeHeaderValue("text/html"), true);           
            MainAsync().GetAwaiter().GetResult(); 
        }

        /// <summary>
        /// Клиент дискорда
        /// </summary>
        public static DiscordSocketClient _client;

        //private static Thread formThread = null;

        /// <summary>
        /// Объект сервера
        /// </summary>
        public static DiscordServerObject serverObject;

        /// <summary>
        /// Объект, содержащий настройки дискорда
        /// </summary>
        public static BotSettings settings = new BotSettings();

        /// <summary>
        /// Объект, содержщаий системные данные
        /// </summary>
        public static DataHandler systemData = new DataHandler();

        /// <summary>
        /// Установка обработчиков и запуск серва
        /// </summary>
        /// <returns></returns>
        public async static Task MainAsync()
        {
            Console.Clear();
            Console.BackgroundColor = ConsoleColor.White;

            await apiServer.RunAPIServer();

            await ConsoleWriter.WriteDivideMessage($"MainAsync() Task started");
            await ConsoleWriter.Write($"Starting bot...", ConsoleWriter.InfoType.Notice);
            _client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Info, MessageCacheSize = 10});

            AddHandlersToClient(_client);

            await ConsoleWriter.Write($"Loading token...", ConsoleWriter.InfoType.Notice);
            string token = File.ReadAllText(@"Token\Token.txt");
            await ConsoleWriter.Write($"Logging bot...", ConsoleWriter.InfoType.Notice);
            await _client.LoginAsync(TokenType.Bot, token);
            await ConsoleWriter.Write($"Starting bot...", ConsoleWriter.InfoType.Notice);
            await _client.StartAsync();
            await ConsoleWriter.Write($"Setting timeout to infinite", ConsoleWriter.InfoType.Notice);
            await ConsoleWriter.WriteDivideMessage($"MainAsync() Task finished");
            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// Метод для сбора общей информации о сервере
        /// </summary>
        /// <returns></returns>
        public static async Task CollectServerInfo()
        {
            await ConsoleWriter.WriteDivideMessage($"CollectServerInfo() started");
            await ConsoleWriter.Write($"Loading main bot data...", ConsoleWriter.InfoType.Notice);
            serverObject = new DiscordServerObject(_client, 636208919114547212);
            await LoadGroups();
            await LoadBans();
            await _client.SetGameAsync($"{serverObject.users.Count()} users...", null, ActivityType.Watching);
            await ConsoleWriter.WriteDivideMessage($"CollectServerInfo finished");
            //formThread = new Thread(() => Application.Run(new MainForm(serverObject)));
            //formThread.Start();
        }

        /// <summary>
        /// Проверка наличия роли у юзера
        /// </summary>
        /// <param name="user">Юзер</param>
        /// <param name="Id">ID роли</param>
        /// <returns></returns>
        public bool HasRole(SocketGuildUser user, ulong Id)
        {
            if (user.Roles.Where(x => x.Id == Id).Count() == 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Вывод текста в консоль
        /// </summary>
        /// <param name="obj"></param>
        public static void Write(object obj)
        {
            Console.WriteLine(obj.ToString());
        }

        /// <summary>
        /// Загрузка всех составов
        /// </summary>
        /// <returns></returns>
        public static async Task LoadGroups()
        {
            await ConsoleWriter.WriteDivideMessage($"LoadGroups() started");
            await ConsoleWriter.Write($"Loading groups...", ConsoleWriter.InfoType.Notice);
            systemData.groups = new List<Group>();
            await ConsoleWriter.Write($"Created empty list", ConsoleWriter.InfoType.Notice);
            systemData.groups = await GroupHandler.LoadAllGroupsFromDB(_client);
            await ConsoleWriter.WriteDivideLine();
            await GroupHandler.ActualizeReactionsOnGroups(systemData.groups, _client);         
            int raidCount = 0;
            int partyCount = 0;
            int fightCount = 0;
            foreach (var group in systemData.groups)
            {
                if (group.IsParty())
                {
                    partyCount++;
                }
                else if (group.IsRaid())
                {
                    raidCount++;
                } 
                else if (group.IsGuildFight())
                {
                    fightCount++;
                }
            }
            await ConsoleWriter.Write($"LOADED PARTIES: {partyCount}", ConsoleWriter.InfoType.Notice);
            await ConsoleWriter.Write($"LOADED RAIDS: {raidCount}", ConsoleWriter.InfoType.Notice);
            await ConsoleWriter.Write($"LOADED GUILD FIGHTS: {fightCount}", ConsoleWriter.InfoType.Notice);
            await ConsoleWriter.WriteDivideMessage($"LoadGroups() finished");
        }

        /// <summary>
        /// Загрузка всех банов
        /// </summary>
        /// <returns></returns>
        public static async Task LoadBans()
        {
            await ConsoleWriter.WriteDivideMessage($"LoadBans() started");
            await ConsoleWriter.Write($"Loading bans...", ConsoleWriter.InfoType.Notice);
            await ConsoleWriter.Write($"Creating new list", ConsoleWriter.InfoType.Notice);
            systemData.bans = new List<DiscordBan>();
            systemData.bans = await BanHandler.LoadAllBansFromDB(_client);
            await ConsoleWriter.Write($"LOADED BANS: {systemData.bans.Count}", ConsoleWriter.InfoType.Notice);
            await ConsoleWriter.WriteDivideMessage($"LoadBans() finished");
        } 

        /// <summary>
        /// Метод для возврата настроек бота
        /// </summary>
        /// <returns></returns>
        public static BotSettings FetchSettings()
        {
            return settings;
        }

        /// <summary>
        /// Метод для возврата основных данных
        /// </summary>
        /// <returns></returns>
        public static DataHandler FetchData()
        {
            return systemData;
        }

        /// <summary>
        /// Метод для возврата объекта сервера
        /// </summary>
        /// <returns></returns>
        public static DiscordServerObject FetchServerObject()
        {
            return serverObject;
        }

        //public static Thread GetFormThread()
        //{
        //    return formThread;
        //}

        /// <summary>
        /// Добавка всех обработчиков событий клиента дискорда
        /// </summary>
        /// <param name="_client"></param>
        public static void AddHandlersToClient(DiscordSocketClient _client)
        {           
            _client.Log += WebhookEventHandler.LogHandler;         
            _client.MessageReceived += WebhookEventHandler.MessageReceivedHandler;           
            _client.ReactionAdded += WebhookEventHandler.ReactionAddedHandler;         
            _client.ReactionRemoved += WebhookEventHandler.ReactionRemovedHandler;          
            _client.Ready += CollectServerInfo;
            _client.ChannelCreated += WebhookEventHandler.ChannelCreatedHandler;
            _client.ChannelDestroyed += WebhookEventHandler.ChannelDestroyedHandler;
            _client.ChannelUpdated += WebhookEventHandler.ChannelUpdatedHandler;
            _client.Connected += WebhookEventHandler.ConnectedHandler;
            _client.CurrentUserUpdated += WebhookEventHandler.CurrentUserUpdatedHandler;
            _client.Disconnected += WebhookEventHandler.DisconnectedHandler;
            _client.GuildAvailable += WebhookEventHandler.GuildAvailableHandler;
            _client.GuildMembersDownloaded += WebhookEventHandler.GuildMembersDownloadedHandler;
            _client.GuildMemberUpdated += WebhookEventHandler.GuildMemberUpdatedHandler;
            _client.GuildUnavailable += WebhookEventHandler.GuildUnavailableHandler;
            _client.GuildUpdated += WebhookEventHandler.GuildUpdatedHandler;
            _client.JoinedGuild += WebhookEventHandler.JoinedGuildHandler;
            _client.LatencyUpdated += WebhookEventHandler.LatencyUpdatedHandler;
            _client.LeftGuild += WebhookEventHandler.LeftGuildHandler;
            _client.LoggedIn += WebhookEventHandler.LoggedInHandler;
            _client.LoggedOut += WebhookEventHandler.LoggedOutHandler;
            _client.MessageDeleted += WebhookEventHandler.MessageDeletedHandler;
            _client.MessagesBulkDeleted += WebhookEventHandler.MessagesBulkDeletedHandler;
            _client.MessageUpdated += WebhookEventHandler.MessageUpdatedHandler;
            _client.ReactionsCleared += WebhookEventHandler.ReactionsClearedHandler;
            _client.RecipientAdded += WebhookEventHandler.RecipientAddedHandler;
            _client.RecipientRemoved += WebhookEventHandler.RecipientRemovedHandler;
            _client.RoleCreated += WebhookEventHandler.RoleCreatedHandler;
            _client.RoleDeleted += WebhookEventHandler.RoleDeletedHandler;
            _client.RoleUpdated += WebhookEventHandler.RoleUpdatedHandler;
            _client.UserBanned += WebhookEventHandler.UserBannedHandler;
            _client.UserIsTyping += WebhookEventHandler.UserIsTypingHandler;
            _client.UserJoined += WebhookEventHandler.UserJoinedHandler;
            _client.UserLeft += WebhookEventHandler.UserLeftHandler;
            _client.UserUnbanned += WebhookEventHandler.UserUnbannedHandler;
            _client.UserUpdated += WebhookEventHandler.UserUpdatedHandler;
            _client.UserVoiceStateUpdated += WebhookEventHandler.UserVoiceStateUpdatedHandler;
            _client.VoiceServerUpdated += WebhookEventHandler.VoiceServerUpdatedHandler;
        }

    }
}
