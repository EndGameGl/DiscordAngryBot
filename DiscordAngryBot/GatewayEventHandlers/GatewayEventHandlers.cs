using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.DiscordCommands;
using DiscordAngryBot.CustomObjects.Filters;
using DiscordAngryBot.CustomObjects.Groups;
using DiscordAngryBot.CustomObjects.Notifications;
using DiscordAngryBot.CustomObjects.Parsers;
using DiscordAngryBot.MessageHandlers;
using DiscordAngryBot.ReactionHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordAngryBot.GatewayEventHandlers
{
    /// <summary>
    /// Класс-обработчик событий клиента дискорда
    /// </summary>
    public static class GatewayEventHandler
    {
        /// <summary>
        /// Обработчик логгирования
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task LogHandler(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Error:
                    BotCore.GetDataLogs().Add(new CustomObjects.Logs.DataLog() { Exception = message.Exception, LogType = "Error" });
                    await ConsoleWriter.Write($"{message.Message}: {message.Exception}", ConsoleWriter.InfoType.Error);
                    await PushNotificator.Notify(message);
                    break;
                case LogSeverity.Warning:
                    //await ConsoleWriter.Write($"{message.Message}: {message.Exception}", ConsoleWriter.InfoType.Error);
                    //await PushNotificator.Notify(message);
                    break;
                case LogSeverity.Info:
                    await ConsoleWriter.Write($"{message.Message}", ConsoleWriter.InfoType.Info);
                    break;
                case LogSeverity.Critical:
                    BotCore.GetDataLogs().Add(new CustomObjects.Logs.DataLog() { Exception = message.Exception, LogType = "Error" });
                    await ConsoleWriter.Write($"{message.Message}: {message.Exception}", ConsoleWriter.InfoType.Error);
                    await PushNotificator.Notify(message);
                    break;
            }
        }
        /// <summary>
        /// Обработчик приходящих сообщений
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task MessageReceivedHandler(SocketMessage message)
        {
            // Проверка, от бота ли сообщение
            if (!message.Author.IsBot)
            {
                if (message.Content.Count() > 0)
                {
                    await ConsoleWriter.Write($"[#{message.Channel}] {message.Author.Username}: {message.Content}", ConsoleWriter.InfoType.Chat);
                    if (message.Channel is SocketGuildChannel)
                    {                       
                        if (BotCore.GetDiscordGuildSettings(((SocketGuildChannel)message.Channel).Guild.Id).IsSwearFilterEnabled == true)
                            await message.RunSwearFilter();
                    }
                }
                else
                {
                    await ConsoleWriter.Write($"[#{message.Channel}] {message.Author.Username}: Пустое сообщение", ConsoleWriter.InfoType.Chat);
                }
                // Проверка сообщения на запрещенные команды 
                if (message.Channel.Name != "команды-ботам" && BotCore.ForbiddenCommands().Contains(message.Content))
                {
                    var mashiroMessage = $"{message.Author.Mention}, все команды ботам пишутся в этот канал: <#636226731459608576>";
                    await message.Channel.SendMessageAsync(mashiroMessage);
                    await message.DeleteAsync();
                }
                if (message.Content.Count() > 0)
                {
                    char? prefix = null;
                    if (message.Channel is SocketGuildChannel)
                    {
                        prefix = BotCore.GetGuildCommandPrefix(((SocketGuildChannel)message.Channel).Guild.Id);
                    }
                    else if (message.Channel is SocketDMChannel)
                    {
                        prefix = BotCore.GetDefaultCommandPrefix();
                    }
                    if (prefix != null && message.Content[0] == prefix)
                    {
                        try
                        {
                            CommandParser commandParser = new CommandParser(message, prefix);
                            string command = commandParser.GetCommand();

                            if (BotCore.SystemCommands().Contains(command))
                            {
                                if (message.Channel is SocketGuildChannel)
                                {
                                    if (BotCore.GetDiscordGuildSettings(((SocketGuildChannel)message.Channel).Guild.Id).adminsID.Contains(message.Author.Id))
                                    {
                                        // Перебор команд
                                        switch (command)
                                        {
                                            case "BAN":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.BanUser(message, commandParser.GetCommandArgs()),
                                                    CommandType.Ban,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                            case "UNBAN":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.UnbanUser(message, commandParser.GetCommandArgs()),
                                                    CommandType.Unban,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                            case "CLEAR":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.ClearMessages(message, commandParser.GetCommandArgs()),
                                                    CommandType.Clear,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                            case "SETPREFIX":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.SetPrefix(message, commandParser.GetCommandArgs()),
                                                    CommandType.SetPrefix,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                            case "NEWS":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.SetNews(message),
                                                    CommandType.News,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                            case "BANROLE":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.SetBanRole(message),
                                                    CommandType.BanRole,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                            case "EMBEDTEST":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.EmbedTesting(message, commandParser.GetCommandArgs()),
                                                    CommandType.TestingPlaceholder,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                            case "ADMIN":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.AddAdmin(message),
                                                    CommandType.Admin,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                            case "DEADMIN":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.RemoveAdmin(message),
                                                    CommandType.Deadmin,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                            case "FILTERENABLE":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.EnableSwearFilter(message),
                                                    CommandType.EnableSwear,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                            case "FILTERDISABLE":
                                                new DiscordCommand(
                                                    CommandHandler.SystemCommands.DisableSwearFilter(message),
                                                    CommandType.DisableSwear,
                                                    message.Author)
                                                    .RunCommand();
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        await message.Channel.SendMessageAsync($"Недостаточно прав на команду.");
                                    }
                                }
                            }
                            else if (BotCore.UserCommands().Contains(command))
                            {
                                    switch (command.ToUpper())
                                    {
                                        case "PARTY":
                                            new DiscordCommand(
                                                CommandHandler.UserCommands.CreateParty(message, commandParser.GetCommandArgs()),
                                                CommandType.Party,
                                                    message.Author)
                                                .RunCommand();
                                            break;
                                        case "RAID":
                                            new DiscordCommand(
                                                CommandHandler.UserCommands.CreateRaid(message, commandParser.GetCommandArgs()),
                                                CommandType.Raid,
                                                    message.Author)
                                                .RunCommand();
                                            break;
                                        case "LIST":
                                            new DiscordCommand(
                                                CommandHandler.UserCommands.ListGroups(message),
                                                CommandType.List,
                                                    message.Author)
                                                .RunCommand();
                                            break;
                                        case "GVGEV":
                                            new DiscordCommand(
                                                CommandHandler.UserCommands.CreateGuildFight(message, commandParser.GetCommandArgs(), GuildFightType.EV),
                                                CommandType.GvgEV,
                                                    message.Author)
                                                .RunCommand();
                                            break;
                                        case "GVGPR":
                                            new DiscordCommand(
                                                CommandHandler.UserCommands.CreateGuildFight(message, commandParser.GetCommandArgs(), GuildFightType.PR),
                                                CommandType.GvgPR,
                                                    message.Author)
                                                .RunCommand();
                                            break;
                                        case "SELFBAN":
                                            new DiscordCommand(
                                                CommandHandler.UserCommands.SelfBan(message, commandParser.GetCommandArgs()),
                                                CommandType.Selfban,
                                                    message.Author)
                                                .RunCommand();
                                            break;
                                        case "HELP":
                                            new DiscordCommand(
                                                CommandHandler.UserCommands.HelpUser(message, commandParser.GetCommandArgs()),
                                                CommandType.Help,
                                                    message.Author)
                                                .RunCommand();
                                            break;
                                    case "ROLL":
                                        new DiscordCommand(
                                            CommandHandler.UserCommands.Roll(message, commandParser.GetCommandArgs()),
                                            CommandType.Roll,
                                                    message.Author)
                                            .RunCommand();
                                        break;
                                }
                                
                            }
                            else if (BotCore.MusicCommands().Contains(command))
                            {
                                switch (command)
                                {
                                    case "SUMMON":
                                        await CommandHandler.MusicCommands.JoinChannel(message);
                                        break;
                                }
                            }
                            else if (BotCore.OtherCommands().Contains(command))
                            {
                                switch (command)
                                {
                                    case "КУСЬ":
                                        new DiscordCommand(
                                            CommandHandler.OtherCommands.Bite(message, commandParser.GetCommandArgs()),
                                            CommandType.Bite,
                                            message.Author)
                                            .RunCommand();
                                        break;
                                    case "БАН":
                                        new DiscordCommand(
                                            CommandHandler.OtherCommands.Ban(message),
                                            CommandType.NotBan,
                                            message.Author)
                                            .RunCommand();
                                        break;
                                }
                            }
                            else
                            {
                                await message.DeleteAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            await ConsoleWriter.Write($"{e.Message} : [{e.InnerException?.Message}]", ConsoleWriter.InfoType.Error);
                        }
                    }
                }
            }
            else if (message.Author.IsBot && message.Author.Username.Contains("Mashiro") && message.Channel.Name != "команды-ботам" && message.Channel.Name != "флудильня")
            {
                await message.DeleteAsync();
            }
        }
        /// <summary>
        /// Обработчик добавления реакций к сообщениям
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="messageChannel"></param>
        /// <param name="reaction"></param>
        /// <returns></returns>
        public static async Task ReactionAddedHandler(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            await ConsoleWriter.Write($"{reaction.User} reacted message {cache.Id} with {reaction.Emote.Name}", ConsoleWriter.InfoType.Info);
            if (reaction.ValidateReaction(new Emoji("\u2705"))) // white_check_mark
            {
                new DiscordCommand(
                    ReactionHandler.PartyReactionHandler.JoinGroup(reaction),
                    CommandType.JoinGroup,
                    (SocketUser)reaction.User)
                    .RunCommand();
            }
            if (reaction.ValidateReaction(new Emoji("\u274C"))) // cross_mark
            {
                new DiscordCommand(
                    ReactionHandler.PartyReactionHandler.TerminateGroup(reaction),
                    CommandType.TerminateGroup,
                    (SocketUser)reaction.User)
                    .RunCommand();
            }
            if (reaction.ValidateReaction(new Emoji("\u2757"))) // exclamation
            {
                new DiscordCommand(
                    ReactionHandler.PartyReactionHandler.GroupCallout(reaction),
                    CommandType.CallGroup,
                    (SocketUser)reaction.User)
                    .RunCommand();
            }
            if (reaction.ValidateReaction(new Emoji("🐾")) || reaction.ValidateReaction(new Emoji("🐷")) || reaction.ValidateReaction(new Emoji("❓")))
            {
                new DiscordCommand(
                    ReactionHandler.PartyReactionHandler.JoinGuildFight(reaction),
                    CommandType.JoinGroup,
                    (SocketUser)reaction.User)
                    .RunCommand();
            }
            if (reaction.ValidateReaction(new Emoji("❎")) || reaction.ValidateReaction(new Emoji("☑️")) || reaction.ValidateReaction(new Emoji("🇽")))
            {
                new DiscordCommand(
                    ReactionHandler.PartyReactionHandler.JoinGuildFight(reaction),
                    CommandType.JoinGroup,
                    (SocketUser)reaction.User)
                    .RunCommand();
            }
        }
        /// <summary>
        /// Обработчик удаления реакциий из сообщений
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="messageChannel"></param>
        /// <param name="reaction"></param>
        /// <returns></returns>
        public static async Task ReactionRemovedHandler(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            await ConsoleWriter.Write($"{reaction.User} removed reaction {reaction.Emote.Name} from {cache.Id}", ConsoleWriter.InfoType.Info);
            if (reaction.ValidateReaction(new Emoji("\u2705"))) // white_check_mark
            {
                new DiscordCommand(
                    ReactionHandler.PartyReactionHandler.LeaveGroup(reaction),
                    CommandType.LeaveGroup,
                    (SocketUser)reaction.User)
                    .RunCommand();
            }
            if (reaction.ValidateReaction(new Emoji("🐾")) || reaction.ValidateReaction(new Emoji("🐷")) || reaction.ValidateReaction(new Emoji("❓")))
            {
                new DiscordCommand(
                    ReactionHandler.PartyReactionHandler.LeaveGuildFight(reaction),
                    CommandType.LeaveGroup,
                    (SocketUser)reaction.User)
                    .RunCommand();
            }
            if (reaction.ValidateReaction(new Emoji("❎")) || reaction.ValidateReaction(new Emoji("☑️")) || reaction.ValidateReaction(new Emoji("🇽")))
            {
                new DiscordCommand(
                    ReactionHandler.PartyReactionHandler.LeaveGuildFight(reaction),
                    CommandType.LeaveGroup,
                    (SocketUser)reaction.User)
                    .RunCommand();
            }
        }
        /// <summary>
        /// Обработчик создания канала
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task ChannelCreatedHandler(SocketChannel channel)
        {
            await ConsoleWriter.Write($"Channel {((IChannel)channel).Name} created", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик удаления канала
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task ChannelDestroyedHandler(SocketChannel channel)
        {
            await ConsoleWriter.Write($"Channel {((IChannel)channel).Name} destroyed", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик обновления канала
        /// </summary>
        /// <param name="oldChannel"></param>
        /// <param name="updatedChannel"></param>
        /// <returns></returns>
        public static async Task ChannelUpdatedHandler(SocketChannel oldChannel, SocketChannel updatedChannel)
        {
            await ConsoleWriter.Write($"Channel {((IChannel)oldChannel).Name} updated into {((IChannel)updatedChannel).Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик присоединения к серверу
        /// </summary>
        /// <returns></returns>
        public static async Task ConnectedHandler()
        {
            await ConsoleWriter.Write($"Connected to servers", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик обновления нынешнего пользователя
        /// </summary>
        /// <param name="oldUser"></param>
        /// <param name="updatedUser"></param>
        /// <returns></returns>
        public static async Task CurrentUserUpdatedHandler(SocketSelfUser oldUser, SocketSelfUser updatedUser)
        {
            await ConsoleWriter.Write($"{oldUser.Username} updated to {updatedUser.Username}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик отключения от сервера
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task DisconnectedHandler(Exception exception)
        {
            //Program.GetFormThread().Abort();
            await ConsoleWriter.Write($"{exception.Message}", ConsoleWriter.InfoType.Error);
        }
        /// <summary>
        /// Обработчик доступности гильдии дискорда
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task GuildAvailableHandler(SocketGuild guild)
        {
            await BotCore.CreateGuildCache(guild);
            await ConsoleWriter.Write($"{guild.Name} is available", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик скачанных юзеров дискорда
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task GuildMembersDownloadedHandler(SocketGuild guild)
        {
            await ConsoleWriter.Write($"{guild.Name} members downloaded, {guild.MemberCount} members", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик обновления состояния пользователя гильдии
        /// </summary>
        /// <param name="oldUser"></param>
        /// <param name="updatedUser"></param>
        /// <returns></returns>
        public static async Task GuildMemberUpdatedHandler(SocketGuildUser oldUser, SocketGuildUser updatedUser)
        {
            //await ConsoleWriter.Write($"{updatedUser.Username} user entity was updated", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик недоступности гильдии дискорда
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task GuildUnavailableHandler(SocketGuild guild)
        {
            await BotCore.RemoveGuildCache(guild);
            await ConsoleWriter.Write($"{guild.Name} is unavailable", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик обновления состояния гильдии дискорда
        /// </summary>
        /// <param name="oldGuild"></param>
        /// <param name="updatedGuild"></param>
        /// <returns></returns>
        public static async Task GuildUpdatedHandler(SocketGuild oldGuild, SocketGuild updatedGuild)
        {
            await ConsoleWriter.Write($"{updatedGuild.Name} guild was updated", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик момента присоединения к гильдии бота
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task JoinedGuildHandler(SocketGuild guild)
        {
            await BotCore.CreateGuildCache(guild);
            await ConsoleWriter.Write($"Joined \"{guild.Name}\" guild", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик смены задержки обновления
        /// </summary>
        /// <param name="oldLatency"></param>
        /// <param name="newLatency"></param>
        /// <returns></returns>
        public static async Task LatencyUpdatedHandler(int oldLatency, int newLatency)
        {

        }
        /// <summary>
        /// Обработчик выхода из гильдии ботом
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task LeftGuildHandler(SocketGuild guild)
        {
            await ConsoleWriter.Write($"Left \"{guild.Name}\" guild", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик логина в дискорд
        /// </summary>
        /// <returns></returns>
        public static async Task LoggedInHandler()
        {
            await ConsoleWriter.Write($"Logged in", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик выхода из дискорда
        /// </summary>
        /// <returns></returns>
        public static async Task LoggedOutHandler()
        {
            await ConsoleWriter.Write($"Logged out", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик удаления сообщения
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task MessageDeletedHandler(Cacheable<IMessage, ulong> cache, ISocketMessageChannel channel)
        {
            if (cache.Value != null)
            {
                await ConsoleWriter.Write($"{cache.Value.Author.Username}'s message {cache.Id} was deleted in {channel.Name}", ConsoleWriter.InfoType.Notice);
            }
            else
            {
                await ConsoleWriter.Write($"Message was deleted in {channel.Name}, no cache for message was found", ConsoleWriter.InfoType.Notice);
            }
        }
        /// <summary>
        /// Обработчик удаления пачки сообщений
        /// </summary>
        /// <param name="cacheList"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task MessagesBulkDeletedHandler(IReadOnlyCollection<Cacheable<IMessage, ulong>> cacheList, ISocketMessageChannel channel)
        {
            await ConsoleWriter.Write($"Deleting a bulk of messages in {channel.Name}", ConsoleWriter.InfoType.Notice);

            foreach (Cacheable<IMessage, ulong> cache in cacheList)
            {
                if (cache.Value != null)
                {
                    await ConsoleWriter.Write($"{cache.Value.Author.Username}'s message {cache.Id} was deleted in {channel.Name}", ConsoleWriter.InfoType.Notice);
                }
                else
                {
                    await ConsoleWriter.Write($"Message was deleted in {channel.Name}, no cache for message was found", ConsoleWriter.InfoType.Notice);
                }
            }
        }
        /// <summary>
        /// Обработчик обновления сообщения дискорда
        /// </summary>
        /// <param name="cachedMessage"></param>
        /// <param name="message"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task MessageUpdatedHandler(Cacheable<IMessage, ulong> cachedMessage, SocketMessage message, ISocketMessageChannel channel)
        {
            if (!message.Author.IsBot)
            {
                if (message.Content != "" || message.Content != null)
                {
                    await ConsoleWriter.Write($"[#{channel.Name}]: {message.Id} was updated to: {message.Content}", ConsoleWriter.InfoType.Notice);
                    await message.RunSwearFilter();
                }
                else
                {
                    await ConsoleWriter.Write($"[#{channel.Name}]: {message.Id} was updated but text is empty", ConsoleWriter.InfoType.Notice);
                }
            }
        }
        /// <summary>
        /// Обработчик очищения реакций сообщения
        /// </summary>
        /// <param name="userMessageCache"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task ReactionsClearedHandler(Cacheable<IUserMessage, ulong> userMessageCache, ISocketMessageChannel channel)
        {
            await ConsoleWriter.Write($"Cleared all reactions from message {userMessageCache.Id} in channel {channel.Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик вступления пользователя в канал (я так и не понял, когда это происходит...)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task RecipientAddedHandler(SocketGroupUser user)
        {
            await ConsoleWriter.Write($"{user.Username} was added to {user.Channel.Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик выхода пользователя из канала (аналогично с выше)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task RecipientRemovedHandler(SocketGroupUser user)
        {
            await ConsoleWriter.Write($"{user.Username} was removed from {user.Channel.Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик создания роли
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task RoleCreatedHandler(SocketRole role)
        {
            await ConsoleWriter.Write($"{role.Name} was created in {role.Guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик удаления роли
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task RoleDeletedHandler(SocketRole role)
        {
            await ConsoleWriter.Write($"{role.Name} was deleted from {role.Guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик обновления роли на сервере
        /// </summary>
        /// <param name="oldRole"></param>
        /// <param name="updatedRole"></param>
        /// <returns></returns>
        public static async Task RoleUpdatedHandler(SocketRole oldRole, SocketRole updatedRole)
        {
            await ConsoleWriter.Write($"{updatedRole.Name} was updated at {updatedRole.Guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик бана пользователя на сервере дискорда
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task UserBannedHandler(SocketUser user, SocketGuild guild)
        {
            await ConsoleWriter.Write($"{user.Username} was banned from {guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик события "Пользователь печатает..."
        /// </summary>
        /// <param name="user"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task UserIsTypingHandler(SocketUser user, ISocketMessageChannel channel)
        {
            // ffs do nothing please
        }
        /// <summary>
        /// Обработчик вступления юзера на сервер
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task UserJoinedHandler(SocketGuildUser user)
        {
            await ConsoleWriter.Write($"{user.Username} joined {user.Guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик выхода юзера с сервера
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task UserLeftHandler(SocketGuildUser user)
        {
            await ConsoleWriter.Write($"{user.Username} left {user.Guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик разбана юзера на сервере
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task UserUnbannedHandler(SocketUser user, SocketGuild guild)
        {
            await ConsoleWriter.Write($"{user.Username} was unbanned at {guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик обновления пользователя
        /// </summary>
        /// <param name="oldUser"></param>
        /// <param name="updatedUser"></param>
        /// <returns></returns>
        public static async Task UserUpdatedHandler(SocketUser oldUser, SocketUser updatedUser)
        {
            //await ConsoleWriter.Write($"{updatedUser.Username} was updated", ConsoleWriter.InfoType.Info);
        }
        /// <summary>
        /// Обработчик состояния голосового статуса пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        public static async Task UserVoiceStateUpdatedHandler(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            if (oldState.VoiceChannel == null && newState.VoiceChannel != null)
            {
                await ConsoleWriter.Write($"{user.Username} has joined {newState.VoiceChannel.Name}", ConsoleWriter.InfoType.Notice);
            }
            else if (oldState.VoiceChannel != null && newState.VoiceChannel != null && (oldState.VoiceChannel.Id != newState.VoiceChannel.Id))
            {
                await ConsoleWriter.Write($"{user.Username} had switched voice channel to {newState.VoiceChannel.Name} from {oldState.VoiceChannel.Name}", ConsoleWriter.InfoType.Notice);
            }
            else if (oldState.VoiceChannel != null && newState.VoiceChannel == null)
            {
                await ConsoleWriter.Write($"{user.Username} had left voice channel {oldState.VoiceChannel.Name}", ConsoleWriter.InfoType.Notice);
            }
        }
        /// <summary>
        /// Обработчик смены голосового сервера дискорда
        /// </summary>
        /// <param name="voiceServer"></param>
        /// <returns></returns>
        public static async Task VoiceServerUpdatedHandler(SocketVoiceServer voiceServer)
        {
            await ConsoleWriter.Write($"Joined voice server", ConsoleWriter.InfoType.Notice);
        }
    }
}
