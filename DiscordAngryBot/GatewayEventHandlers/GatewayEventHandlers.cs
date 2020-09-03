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
        private static Emoji TerminateGroupEmoji = EmojiGetter.GetEmoji(Emojis.CrossMark);
        private static Emoji CallGroupEmoji = EmojiGetter.GetEmoji(Emojis.ExclamationMark);
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
                    await Debug.Log($"{message.Message}: {message.Exception}", Debug.InfoType.Error);
                    await PushNotificator.Notify(message);
                    break;
                case LogSeverity.Warning:
                    //await ConsoleWriter.Write($"{message.Message}: {message.Exception}", ConsoleWriter.InfoType.Error);
                    //await PushNotificator.Notify(message);
                    break;
                case LogSeverity.Info:
                    await Debug.Log($"{message.Message}", Debug.InfoType.Info);
                    break;
                case LogSeverity.Critical:
                    BotCore.GetDataLogs().Add(new CustomObjects.Logs.DataLog() { Exception = message.Exception, LogType = "Error" });
                    await Debug.Log($"{message.Message}: {message.Exception}", Debug.InfoType.Error);
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
                    await Debug.Log($"[#{message.Channel}] {message.Author.Username}: {message.Content}", Debug.InfoType.Chat);
                    if (message.Channel is SocketGuildChannel)
                    {
                        if (BotCore.TryGetDiscordGuildSettings(((SocketGuildChannel)message.Channel).Guild.Id, out var settings))
                        {
                            if (settings.IsSwearFilterEnabled == true)
                                await message.RunSwearFilter();
                        }
                    }
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
                            string commandName = commandParser.GetCommand();
                            if (BotCore.TryGetCommand(commandName, out DiscordCommand command))
                            {
                                if (command.CommandMetadata.Type == CommandType.StringCommand)
                                {
                                    if (command.CommandMetadata.CommandExecutionScope == CommandExecutionScope.All)
                                    {
                                        command.Run(new DiscordCommandParameterSet(message).AddParameter(commandParser.GetCommandArgs()));
                                    }
                                    else if (message.Channel is SocketGuildChannel && command.CommandMetadata.CommandExecutionScope == CommandExecutionScope.Server)
                                    {
                                        switch (command.CommandMetadata.Scope)
                                        {
                                            case CommandScope.Admin:
                                                if (BotCore.TryGetDiscordGuildSettings(((SocketGuildChannel)message.Channel).Guild.Id, out var settings))
                                                {
                                                    if (settings.IsAdmin(message.Author.Id))
                                                        command.Run(new DiscordCommandParameterSet(message).AddParameter(commandParser.GetCommandArgs()));
                                                    else
                                                        await message.Channel.SendMessageAsync($"Недостаточно прав на команду.");
                                                }

                                                break;
                                            case CommandScope.User:
                                                command.Run(new DiscordCommandParameterSet(message).AddParameter(commandParser.GetCommandArgs()));
                                                break;
                                        }
                                    }
                                    else if (message.Channel is SocketDMChannel && command.CommandMetadata.CommandExecutionScope == CommandExecutionScope.DM)
                                    {
                                        command.Run(new DiscordCommandParameterSet(message).AddParameter(commandParser.GetCommandArgs()));
                                    }
                                    else
                                        await message.Channel.SendMessageAsync(CommandResources.InvalidLocationScopeLine);
                                }
                                await message.DeleteAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            await Debug.Log($"{e.Message} : [{e.InnerException?.Message}]", Debug.InfoType.Error);
                        }
                    }                   
                }
                else
                {
                    await Debug.Log($"[#{message.Channel}] {message.Author.Username}: Пустое сообщение", Debug.InfoType.Chat);
                }
                // Проверка сообщения на запрещенные команды 
                if (message.Channel.Name != "команды-ботам" && BotCore.ForbiddenCommands().Contains(message.Content))
                {
                    var mashiroMessage = $"{message.Author.Mention}, все команды ботам пишутся в этот канал: <#636226731459608576>";
                    await message.Channel.SendMessageAsync(mashiroMessage);
                    await message.DeleteAsync();
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
            await Debug.Log($"{reaction.User} reacted message {cache.Id} with {reaction.Emote.Name}", Debug.InfoType.Info);
            if (!reaction.User.Value.IsBot)
            {
                await Debug.Log("Reaction is from user. Checking if emoji command...");
                string commandName = string.Empty;
                if (BotCore.TryGetDiscordGuildGroups((messageChannel as SocketGuildChannel).Guild.Id, out List<Group> groups))
                {
                    await Debug.Log($"Found {groups.Count} groups for this guild.");
                    if (groups.TryFindGroup(reaction.MessageId, out Group groupTiedToMessage))
                    {
                        await Debug.Log($"Found group tied to reacted message. Checking reaction corellation.");
                        if (groupTiedToMessage.TryGetUserList((Emoji)reaction.Emote, out UserList userList))
                        {
                            await Debug.Log($"Found user list with corresponding enter reaction.");
                            commandName = "JOINGROUP";
                        }
                        else
                        {
                            if (reaction.Emote.Name == TerminateGroupEmoji.Name)
                            {
                                commandName = "TERMINATEGROUP";
                            }
                            else if (reaction.Emote.Name == CallGroupEmoji.Name)
                            {
                                commandName = "CALLGROUP";
                            }
                        }
                        if (BotCore.TryGetCommand(commandName, out DiscordCommand command))
                        {
                            await Debug.Log($"Command {command.CommandMetadata.CommandName} found. Executing command.");
                            command.Run(new DiscordCommandParameterSet(groupTiedToMessage).AddParameter(reaction));
                        }
                    }
                }
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
            await Debug.Log($"{reaction.User} removed reaction {reaction.Emote.Name} from {cache.Id}", Debug.InfoType.Info);           
            if ((reaction.Emote.Name != TerminateGroupEmoji.Name) && (reaction.Emote.Name != CallGroupEmoji.Name))
            {
                if (BotCore.TryGetDiscordGuildGroups((messageChannel as SocketGuildChannel).Guild.Id, out List<Group> groups))
                {
                    if (groups.TryFindGroup(reaction.MessageId, out Group groupTiedToMessage))
                    {
                        if (groupTiedToMessage.TryGetUserList((Emoji)reaction.Emote, out UserList userList))
                        {
                            if (BotCore.TryGetCommand("LEAVEGROUP", out DiscordCommand command))
                            {
                                command.Run(new DiscordCommandParameterSet(groupTiedToMessage).AddParameter(reaction));
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Обработчик создания канала
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task ChannelCreatedHandler(SocketChannel channel)
        {
            await Debug.Log($"Channel {((IChannel)channel).Name} created", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик удаления канала
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task ChannelDestroyedHandler(SocketChannel channel)
        {
            await Debug.Log($"Channel {((IChannel)channel).Name} destroyed", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик обновления канала
        /// </summary>
        /// <param name="oldChannel"></param>
        /// <param name="updatedChannel"></param>
        /// <returns></returns>
        public static async Task ChannelUpdatedHandler(SocketChannel oldChannel, SocketChannel updatedChannel)
        {
            await Debug.Log($"Channel {((IChannel)oldChannel).Name} updated into {((IChannel)updatedChannel).Name}", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик присоединения к серверу
        /// </summary>
        /// <returns></returns>
        public static async Task ConnectedHandler()
        {
            await Debug.Log($"Connected to servers", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик обновления нынешнего пользователя
        /// </summary>
        /// <param name="oldUser"></param>
        /// <param name="updatedUser"></param>
        /// <returns></returns>
        public static async Task CurrentUserUpdatedHandler(SocketSelfUser oldUser, SocketSelfUser updatedUser)
        {
            await Debug.Log($"{oldUser.Username} updated to {updatedUser.Username}", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик отключения от сервера
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static async Task DisconnectedHandler(Exception exception)
        {
            //Program.GetFormThread().Abort();
            await Debug.Log($"{exception.Message}", Debug.InfoType.Error);
        }
        /// <summary>
        /// Обработчик доступности гильдии дискорда
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task GuildAvailableHandler(SocketGuild guild)
        {
            await BotCore.CreateGuildCache(guild);
            await Debug.Log($"{guild.Name} is available", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик скачанных юзеров дискорда
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task GuildMembersDownloadedHandler(SocketGuild guild)
        {
            await Debug.Log($"{guild.Name} members downloaded, {guild.MemberCount} members", Debug.InfoType.Notice);
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
            await Debug.Log($"{guild.Name} is unavailable", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик обновления состояния гильдии дискорда
        /// </summary>
        /// <param name="oldGuild"></param>
        /// <param name="updatedGuild"></param>
        /// <returns></returns>
        public static async Task GuildUpdatedHandler(SocketGuild oldGuild, SocketGuild updatedGuild)
        {
            await Debug.Log($"{updatedGuild.Name} guild was updated", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик момента присоединения к гильдии бота
        /// </summary>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task JoinedGuildHandler(SocketGuild guild)
        {
            await BotCore.CreateGuildCache(guild);
            await Debug.Log($"Joined \"{guild.Name}\" guild", Debug.InfoType.Notice);
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
            await Debug.Log($"Left \"{guild.Name}\" guild", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик логина в дискорд
        /// </summary>
        /// <returns></returns>
        public static async Task LoggedInHandler()
        {
            await Debug.Log($"Logged in", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик выхода из дискорда
        /// </summary>
        /// <returns></returns>
        public static async Task LoggedOutHandler()
        {
            await Debug.Log($"Logged out", Debug.InfoType.Notice);
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
                await Debug.Log($"{cache.Value.Author.Username}'s message {cache.Id} was deleted in {channel.Name}", Debug.InfoType.Notice);
            }
            else
            {
                await Debug.Log($"Message was deleted in {channel.Name}, no cache for message was found", Debug.InfoType.Notice);
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
            await Debug.Log($"Deleting a bulk of messages in {channel.Name}", Debug.InfoType.Notice);

            foreach (Cacheable<IMessage, ulong> cache in cacheList)
            {
                if (cache.Value != null)
                {
                    await Debug.Log($"{cache.Value.Author.Username}'s message {cache.Id} was deleted in {channel.Name}", Debug.InfoType.Notice);
                }
                else
                {
                    await Debug.Log($"Message was deleted in {channel.Name}, no cache for message was found", Debug.InfoType.Notice);
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
                    await Debug.Log($"[#{channel.Name}]: {message.Id} was updated to: {message.Content}", Debug.InfoType.Notice);
                    await message.RunSwearFilter();
                }
                else
                {
                    await Debug.Log($"[#{channel.Name}]: {message.Id} was updated but text is empty", Debug.InfoType.Notice);
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
            await Debug.Log($"Cleared all reactions from message {userMessageCache.Id} in channel {channel.Name}", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик вступления пользователя в канал (я так и не понял, когда это происходит...)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task RecipientAddedHandler(SocketGroupUser user)
        {
            await Debug.Log($"{user.Username} was added to {user.Channel.Name}", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик выхода пользователя из канала (аналогично с выше)
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task RecipientRemovedHandler(SocketGroupUser user)
        {
            await Debug.Log($"{user.Username} was removed from {user.Channel.Name}", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик создания роли
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task RoleCreatedHandler(SocketRole role)
        {
            await Debug.Log($"{role.Name} was created in {role.Guild.Name}", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик удаления роли
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static async Task RoleDeletedHandler(SocketRole role)
        {
            await Debug.Log($"{role.Name} was deleted from {role.Guild.Name}", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик обновления роли на сервере
        /// </summary>
        /// <param name="oldRole"></param>
        /// <param name="updatedRole"></param>
        /// <returns></returns>
        public static async Task RoleUpdatedHandler(SocketRole oldRole, SocketRole updatedRole)
        {
            await Debug.Log($"{updatedRole.Name} was updated at {updatedRole.Guild.Name}", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик бана пользователя на сервере дискорда
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task UserBannedHandler(SocketUser user, SocketGuild guild)
        {
            await Debug.Log($"{user.Username} was banned from {guild.Name}", Debug.InfoType.Notice);
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
            await Debug.Log($"{user.Username} joined {user.Guild.Name}", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик выхода юзера с сервера
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task UserLeftHandler(SocketGuildUser user)
        {
            await Debug.Log($"{user.Username} left {user.Guild.Name}", Debug.InfoType.Notice);
        }
        /// <summary>
        /// Обработчик разбана юзера на сервере
        /// </summary>
        /// <param name="user"></param>
        /// <param name="guild"></param>
        /// <returns></returns>
        public static async Task UserUnbannedHandler(SocketUser user, SocketGuild guild)
        {
            await Debug.Log($"{user.Username} was unbanned at {guild.Name}", Debug.InfoType.Notice);
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
                await Debug.Log($"{user.Username} has joined {newState.VoiceChannel.Name}", Debug.InfoType.Notice);
            }
            else if (oldState.VoiceChannel != null && newState.VoiceChannel != null && (oldState.VoiceChannel.Id != newState.VoiceChannel.Id))
            {
                await Debug.Log($"{user.Username} had switched voice channel to {newState.VoiceChannel.Name} from {oldState.VoiceChannel.Name}", Debug.InfoType.Notice);
            }
            else if (oldState.VoiceChannel != null && newState.VoiceChannel == null)
            {
                await Debug.Log($"{user.Username} had left voice channel {oldState.VoiceChannel.Name}", Debug.InfoType.Notice);
            }
        }
        /// <summary>
        /// Обработчик смены голосового сервера дискорда
        /// </summary>
        /// <param name="voiceServer"></param>
        /// <returns></returns>
        public static async Task VoiceServerUpdatedHandler(SocketVoiceServer voiceServer)
        {
            await Debug.Log($"Joined voice server", Debug.InfoType.Notice);
        }
    }
}
