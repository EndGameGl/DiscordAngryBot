using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Groups;
using DiscordAngryBot.MessageHandlers;
using DiscordAngryBot.ReactionHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot
{
    public class Program
    {
        public bool isGroupStateLoaded = false;
        static void Main(string[] args) 
            => new Program().MainAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Клиент дискорда
        /// </summary>
        public static DiscordSocketClient _client;

        /// <summary>
        /// Объект сервера
        /// </summary>
        public static DiscordServerObject serverObject;

        /// <summary>
        /// Объект, содержащий настройки дискорда
        /// </summary>
        private BotSettings settings = new BotSettings();

        /// <summary>
        /// Объект, содержщаий системные данные
        /// </summary>
        public static DataHandler systemData = new DataHandler();

        /// <summary>
        /// Пустой конструктор программы
        /// </summary>
        private Program() { }

        /// <summary>
        /// Установка обработчиков и запуск серва
        /// </summary>
        /// <returns></returns>
        public async Task MainAsync()
        {

            _client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Info });
            _client.Log += Log;
            _client.MessageReceived += MessageReceived;
            _client.ReactionAdded += ReactionAdded;
            _client.ReactionRemoved += ReactionRemoved;
            _client.Ready += CollectServerInfo;
            _client.Disconnected += ManageDisconnect;

            //_client.UserJoined
            //_client.UserLeft
            string token = File.ReadAllText(@"F:\Programming Stuff\DToken\Token.txt");
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            
            await Task.Delay(Timeout.Infinite);
        }

        /// <summary>
        /// Логгирование ошибок и сообщений сервера
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task Log(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Обработка новых сообщений
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task MessageReceived(SocketMessage message)
        {
            // Проверка, от бота ли сообщение
            if (!message.Author.IsBot)
            {
                if (message.Content.Count() > 0)
                {
                    Console.WriteLine($"[{message.CreatedAt}]-[#{message.Channel}] {message.Author}: {message.Content}");
                }
                else 
                {
                    Console.WriteLine($"[{message.CreatedAt}]-[#{message.Channel}] {message.Author}: Message is empty");
                }

                // Проверка сообщения на запрещенные команды 
                if (message.Channel.Name != "команды-ботам" && settings.forbiddenCommands.Contains(message.Content))
                {
                    var mashiroMessage = $"<@!{message.Author.Id}>, все команды ботам пишутся в этот канал: <#636226731459608576>";                  
                    await message.Channel.SendMessageAsync(mashiroMessage);
                    await message.DeleteAsync();
                }

                // Проверка, не пустое ли сообщение
                if (message.Content.Count() > 0) 
                {
                    // Проверка наличия команды в сообщении
                    if (message.Content[0] == settings.commandPrefix)
                    {
                        var commandParameters = CommandHandler.ProcessCommandMessage(message);
                        var channel = commandParameters.Item1;
                        var command = commandParameters.Item2;
                        var args = commandParameters.Item3;
                        // Проверка наличия такой команды в списке команд бота
                        if (settings.systemCommands.Contains(command.ToUpper()))
                        {
                            if (settings.admins.Contains(message.Author.Id))
                            {
                                // Перебор команд
                                switch (command.ToUpper())
                                {
                                    case "BAN":
                                        await CommandHandler.SystemCommands.TempBanUser(serverObject, systemData.timers, message, commandParameters);
                                        break;                                   
                                }
                            }
                            else
                            {
                                await channel.SendMessageAsync($"Недостаточно прав на команду.");
                            }
                        }
                        if (settings.userCommands.Contains(command.ToUpper()))
                        {
                            if (true)
                            {
                                switch (command.ToUpper())
                                {
                                    case "PARTY":
                                        await CommandHandler.UserCommands.CreateParty(systemData.groups, message, args);
                                        break;
                                    case "RAID":
                                        await CommandHandler.UserCommands.CreateRaid(systemData.groups, message, args);
                                        break;
                                    case "LIST":
                                        await CommandHandler.UserCommands.ListGroups(message, systemData.groups);
                                        break;
                                }
                            }                            
                        }
                        else
                            await message.DeleteAsync();
                    }
                }                                 
            }
            else if (message.Author.IsBot && message.Author.Username.Contains("Mashiro") && message.Channel.Name != "команды-ботам" && message.Channel.Name != "флудильня")
            {
                await message.DeleteAsync();
            }
        }

        /// <summary>
        /// Обработка добавления реакции
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="messageChannel"></param>
        /// <param name="reaction"></param>
        /// <returns></returns>
        public async Task ReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            if (reaction.Emote.Name == "✅" && !reaction.User.Value.IsBot)
            {
                var message = await messageChannel.GetMessageAsync(cache.Id);

                Group group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                if (group != null)
                {
                    ReactionHandler.PartyReactionHandler.JoinGroup(group, message, reaction, systemData.groups);
                }
            }
            else if (reaction.Emote.Name == "\u274C" && !reaction.User.Value.IsBot)
            {
                var message = await messageChannel.GetMessageAsync(cache.Id);
                Group group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                if (group != null)
                {
                    ReactionHandler.PartyReactionHandler.TerminateGroup(group, message, reaction, systemData.groups);
                }
            }
        }

        /// <summary>
        /// Обработка удаления реакции
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="messageChannel"></param>
        /// <param name="reaction"></param>
        /// <returns></returns>
        public async Task ReactionRemoved(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            if (reaction.Emote.Name == "✅" && !reaction.User.Value.IsBot)
            {
                var message = await messageChannel.GetMessageAsync(cache.Id);

                Group group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                if (group == null)
                {
                    group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                }
                if (group != null)
                {
                    ReactionHandler.PartyReactionHandler.LeaveGroup(group, message, reaction, systemData.groups);
                }
            }
        }

        /// <summary>
        /// Метод для сбора общей информации о сервере
        /// </summary>
        /// <returns></returns>
        public async Task CollectServerInfo()
        {            
            serverObject = new DiscordServerObject(_client, 636208919114547212);
            await LoadGroups();
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
        public async Task LoadGroups()
        {
            systemData.groups = await GroupHandler.LoadAllGroupsFromDB(_client);
            int raidCount = 0;
            int partyCount = 0;
            foreach (var group in systemData.groups)
            {
                if (group is Party)
                {
                    partyCount++;
                }
                else if (group is Raid)
                {
                    raidCount++;
                }               
            }
            Write($"Загружено групп: {partyCount}\nЗагружено рейдов: {raidCount}");
        }

        /// <summary>
        /// Обработка отключения сервера
        /// </summary>
        /// <param name="e">Исключение, повлекшее отключение сервера</param>
        /// <returns></returns>
        public async Task ManageDisconnect(Exception e) 
        {
            if (systemData.groups.Count > 0) 
            {
                systemData.groups = new List<Group>();
            }
        }
    }
}
