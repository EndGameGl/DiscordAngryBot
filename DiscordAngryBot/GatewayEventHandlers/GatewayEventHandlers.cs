using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.DiscordCommands;
using DiscordAngryBot.CustomObjects.Filters;
using DiscordAngryBot.CustomObjects.Groups;
using DiscordAngryBot.CustomObjects.Logs;
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
    /// Class for handling discord client gateway events
    /// </summary>
    public static class GatewayEventHandler
    {
        /// <summary>
        /// Emoji that is predefined for closing groups
        /// </summary>
        private static readonly Emoji TerminateGroupEmoji = EmojiGetter.GetEmoji(Emojis.CrossMark);
        /// <summary>
        /// Emoji that is predefined for calling groups
        /// </summary>
        private static readonly Emoji CallGroupEmoji = EmojiGetter.GetEmoji(Emojis.ExclamationMark);

        /// <summary>
        /// Log handler
        /// </summary>
        /// <param name="message">Logged message</param>
        /// <returns></returns>
        public static async Task LogHandler(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Error:
                    BotCore.GetDataLogs().Add(new DataLog(message.Exception, "Error"));
                    await Debug.Log($"{message.Message}: {message.Exception}", LogInfoType.Error);
                    await PushNotificator.Notify(message);
                    break;
                case LogSeverity.Warning:
                    break;
                case LogSeverity.Info:
                    await Debug.Log($"{message.Message}", LogInfoType.Info);
                    break;
                case LogSeverity.Critical:
                    BotCore.GetDataLogs().Add(new DataLog(message.Exception, "Error"));
                    await Debug.Log($"{message.Message}: {message.Exception}", LogInfoType.Error);
                    await PushNotificator.Notify(message);
                    break;
            }
        }

        /// <summary>
        /// Received messages handler
        /// </summary>
        /// <param name="message">Received message</param>
        /// <returns></returns>
        public static async Task MessageReceivedHandler(SocketMessage message)
        {
            if (!message.Author.IsBot)
            {
                if (message.Content.Count() > 0)
                {
                    await Debug.Log($"[#{message.Channel}] {message.Author.Username}: {message.Content}", LogInfoType.Chat);
                    char? prefix = null;
                    if (message.Channel is SocketGuildChannel channel)
                    {
                        if (BotCore.TryGetDiscordGuildSettings(channel.Guild.Id, out var settings))
                        {
                            if (settings.IsSwearFilterEnabled == true)
                                await message.RunSwearFilter();
                        }
                        prefix = BotCore.GetGuildCommandPrefix(channel.Guild.Id);
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
                                if (!(message.Channel is SocketDMChannel))
                                    await message.DeleteAsync();
                            }
                        }
                        catch (Exception e)
                        {
                            await Debug.Log($"{e.Message} : [{e.InnerException?.Message}]", LogInfoType.Error);
                        }
                    }
                }
                else
                {
                    await Debug.Log($"[#{message.Channel}] {message.Author.Username}: Пустое сообщение", LogInfoType.Chat);

                    if (message.Channel.Name != "команды-ботам" && BotCore.ForbiddenCommands().Contains(message.Content))
                    {
                        var mashiroMessage = $"{message.Author.Mention}, все команды ботам пишутся в этот канал: <#636226731459608576>";
                        await message.Channel.SendMessageAsync(mashiroMessage);
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
        /// Reaction added handler
        /// </summary>
        /// <param name="cache">Message cache</param>
        /// <param name="messageChannel">Message channel</param>
        /// <param name="reaction">Reaction added</param>
        /// <returns></returns>
        public static async Task ReactionAddedHandler(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            await Debug.Log($"{reaction.User} reacted message {cache.Id} with {reaction.Emote.Name}", LogInfoType.Info);
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
        /// Reaction removed handler
        /// </summary>
        /// <param name="cache">Message cache</param>
        /// <param name="messageChannel">Message channel</param>
        /// <param name="reaction">Reaction removed</param>
        /// <returns></returns>
        public static async Task ReactionRemovedHandler(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            await Debug.Log($"{reaction.User} removed reaction {reaction.Emote.Name} from {cache.Id}", LogInfoType.Info);           
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
        /// Channel created handler
        /// </summary>
        /// <param name="channel">Created channel</param>
        /// <returns></returns>
        public static async Task ChannelCreatedHandler(SocketChannel channel)
        {
            await Debug.Log($"Channel {((IChannel)channel).Name} created", LogInfoType.Notice);
        }

        /// <summary>
        /// Channel destroyed handler
        /// </summary>
        /// <param name="channel">Destroyed channel</param>
        /// <returns></returns>
        public static async Task ChannelDestroyedHandler(SocketChannel channel)
        {
            await Debug.Log($"Channel {((IChannel)channel).Name} destroyed", LogInfoType.Notice);
        }

        /// <summary>
        /// Channel updated channel
        /// </summary>
        /// <param name="oldChannel">Channel before update</param>
        /// <param name="updatedChannel">Channel after update</param>
        /// <returns></returns>
        public static async Task ChannelUpdatedHandler(SocketChannel oldChannel, SocketChannel updatedChannel)
        {
            await Debug.Log($"Channel {((IChannel)oldChannel).Name} updated into {((IChannel)updatedChannel).Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// Connected to server handler
        /// </summary>
        /// <returns></returns>
        public static async Task ConnectedHandler()
        {
            await Debug.Log($"Connected to servers", LogInfoType.Notice);
        }

        /// <summary>
        /// Current user updated handler
        /// </summary>
        /// <param name="oldUser">User before update</param>
        /// <param name="updatedUser">User after update</param>
        /// <returns></returns>
        public static async Task CurrentUserUpdatedHandler(SocketSelfUser oldUser, SocketSelfUser updatedUser)
        {
            await Debug.Log($"{oldUser.Username} updated to {updatedUser.Username}", LogInfoType.Notice);
        }

        /// <summary>
        /// Disconnected form server handler
        /// </summary>
        /// <param name="exception">Caught exception</param>
        /// <returns></returns>
        public static async Task DisconnectedHandler(Exception exception)
        {
            await Debug.Log($"{exception.Message}", LogInfoType.Error);
        }

        /// <summary>
        /// Guild available handler
        /// </summary>
        /// <param name="guild">Available guild</param>
        /// <returns></returns>
        public static async Task GuildAvailableHandler(SocketGuild guild)
        {
            await BotCore.CreateGuildCache(guild);
            await Debug.Log($"{guild.Name} is available", LogInfoType.Notice);
        }

        /// <summary>
        /// Guild members downloaded handler
        /// </summary>
        /// <param name="guild">Guild that members are from</param>
        /// <returns></returns>
        public static async Task GuildMembersDownloadedHandler(SocketGuild guild)
        {
            await Debug.Log($"{guild.Name} members downloaded, {guild.MemberCount} members", LogInfoType.Notice);
        }

        /// <summary>
        /// Guild member updated handler
        /// </summary>
        /// <param name="oldUser">User before update</param>
        /// <param name="updatedUser">User after update</param>
        /// <returns></returns>
        public static async Task GuildMemberUpdatedHandler(SocketGuildUser oldUser, SocketGuildUser updatedUser)
        {
        }

        /// <summary>
        /// Guild unavailable handler
        /// </summary>
        /// <param name="guild">Guild that is unavailable</param>
        /// <returns></returns>
        public static async Task GuildUnavailableHandler(SocketGuild guild)
        {
            await BotCore.RemoveGuildCache(guild);
            await Debug.Log($"{guild.Name} is unavailable", LogInfoType.Notice);
        }

        /// <summary>
        /// Guild updated handler
        /// </summary>
        /// <param name="oldGuild">Guild before update</param>
        /// <param name="updatedGuild">Guild after update</param>
        /// <returns></returns>
        public static async Task GuildUpdatedHandler(SocketGuild oldGuild, SocketGuild updatedGuild)
        {
            await Debug.Log($"{updatedGuild.Name} guild was updated", LogInfoType.Notice);
        }

        /// <summary>
        /// Bot joined guild handler
        /// </summary>
        /// <param name="guild">Guild that was joined</param>
        /// <returns></returns>
        public static async Task JoinedGuildHandler(SocketGuild guild)
        {
            await BotCore.CreateGuildCache(guild);
            await Debug.Log($"Joined \"{guild.Name}\" guild", LogInfoType.Notice);
        }

        /// <summary>
        /// Latency updated handler
        /// </summary>
        /// <param name="oldLatency">Latency before update</param>
        /// <param name="newLatency">Latency after update</param>
        /// <returns></returns>
        public static async Task LatencyUpdatedHandler(int oldLatency, int newLatency) { }

        /// <summary>
        /// Bot left guild handler
        /// </summary>
        /// <param name="guild">Guild that was left</param>
        /// <returns></returns>
        public static async Task LeftGuildHandler(SocketGuild guild)
        {
            await Debug.Log($"Left \"{guild.Name}\" guild", LogInfoType.Notice);
        }

        /// <summary>
        /// Logged in handler
        /// </summary>
        /// <returns></returns>
        public static async Task LoggedInHandler()
        {
            await Debug.Log($"Logged in", LogInfoType.Notice);
        }

        /// <summary>
        /// Logged out handler
        /// </summary>
        /// <returns></returns>
        public static async Task LoggedOutHandler()
        {
            await Debug.Log($"Logged out", LogInfoType.Notice);
        }

        /// <summary>
        /// Message deleted handler
        /// </summary>
        /// <param name="cache">Message cache</param>
        /// <param name="channel">Message channel</param>
        /// <returns></returns>
        public static async Task MessageDeletedHandler(Cacheable<IMessage, ulong> cache, ISocketMessageChannel channel)
        {
            if (cache.Value != null)
            {
                await Debug.Log($"{cache.Value.Author.Username}'s message {cache.Id} was deleted in {channel.Name}", LogInfoType.Notice);
            }
            else
            {
                await Debug.Log($"Message was deleted in {channel.Name}, no cache for message was found", LogInfoType.Notice);
            }
        }

        /// <summary>
        /// Message bulk deleted handler
        /// </summary>
        /// <param name="cacheList">Message cache list</param>
        /// <param name="channel">Messaged channel</param>
        /// <returns></returns>
        public static async Task MessagesBulkDeletedHandler(IReadOnlyCollection<Cacheable<IMessage, ulong>> cacheList, ISocketMessageChannel channel)
        {
            await Debug.Log($"Deleting a bulk of messages in {channel.Name}", LogInfoType.Notice);

            foreach (Cacheable<IMessage, ulong> cache in cacheList)
            {
                if (cache.Value != null)
                {
                    await Debug.Log($"{cache.Value.Author.Username}'s message {cache.Id} was deleted in {channel.Name}", LogInfoType.Notice);
                }
                else
                {
                    await Debug.Log($"Message was deleted in {channel.Name}, no cache for message was found", LogInfoType.Notice);
                }
            }
        }

        /// <summary>
        /// Message updated handler
        /// </summary>
        /// <param name="cachedMessage">Cached old message</param>
        /// <param name="message">Updated message</param>
        /// <param name="channel">Message channel</param>
        /// <returns></returns>
        public static async Task MessageUpdatedHandler(Cacheable<IMessage, ulong> cachedMessage, SocketMessage message, ISocketMessageChannel channel)
        {
            if (!message.Author.IsBot)
            {
                if (message.Content != "" || message.Content != null)
                {
                    await Debug.Log($"[#{channel.Name}]: {message.Id} was updated to: {message.Content}", LogInfoType.Notice);
                    await message.RunSwearFilter();
                }
                else
                {
                    await Debug.Log($"[#{channel.Name}]: {message.Id} was updated but text is empty", LogInfoType.Notice);
                }
            }
        }

        /// <summary>
        /// Reactions cleared handler
        /// </summary>
        /// <param name="userMessageCache">Message cache</param>
        /// <param name="channel">Message channel</param>
        /// <returns></returns>
        public static async Task ReactionsClearedHandler(Cacheable<IUserMessage, ulong> userMessageCache, ISocketMessageChannel channel)
        {
            await Debug.Log($"Cleared all reactions from message {userMessageCache.Id} in channel {channel.Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// Recipient added handler
        /// </summary>
        /// <param name="user">User that was added</param>
        /// <returns></returns>
        public static async Task RecipientAddedHandler(SocketGroupUser user)
        {
            await Debug.Log($"{user.Username} was added to {user.Channel.Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// Recipient removed handler
        /// </summary>
        /// <param name="user">User that was removed</param>
        /// <returns></returns>
        public static async Task RecipientRemovedHandler(SocketGroupUser user)
        {
            await Debug.Log($"{user.Username} was removed from {user.Channel.Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// Role created handler
        /// </summary>
        /// <param name="role">Role that was created</param>
        /// <returns></returns>
        public static async Task RoleCreatedHandler(SocketRole role)
        {
            await Debug.Log($"{role.Name} was created in {role.Guild.Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// Role removed handler
        /// </summary>
        /// <param name="role">Role that was removed</param>
        /// <returns></returns>
        public static async Task RoleDeletedHandler(SocketRole role)
        {
            await Debug.Log($"{role.Name} was deleted from {role.Guild.Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// Role updated handler
        /// </summary>
        /// <param name="oldRole">Role before update</param>
        /// <param name="updatedRole">Role after update</param>
        /// <returns></returns>
        public static async Task RoleUpdatedHandler(SocketRole oldRole, SocketRole updatedRole)
        {
            await Debug.Log($"{updatedRole.Name} was updated at {updatedRole.Guild.Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// User banned handler
        /// </summary>
        /// <param name="user">User that was banned</param>
        /// <param name="guild">Guild from which user was banned</param>
        /// <returns></returns>
        public static async Task UserBannedHandler(SocketUser user, SocketGuild guild)
        {
            await Debug.Log($"{user.Username} was banned from {guild.Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// User is typing handler
        /// </summary>
        /// <param name="user">User that is typing</param>
        /// <param name="channel">Channel where user was typing</param>
        /// <returns></returns>
        public static async Task UserIsTypingHandler(SocketUser user, ISocketMessageChannel channel) { }

        /// <summary>
        /// User joined guild handler
        /// </summary>
        /// <param name="user">User that joined handler</param>
        /// <returns></returns>
        public static async Task UserJoinedHandler(SocketGuildUser user)
        {
            await Debug.Log($"{user.Username} joined {user.Guild.Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// User left guild handler
        /// </summary>
        /// <param name="user">User that left guild</param>
        /// <returns></returns>
        public static async Task UserLeftHandler(SocketGuildUser user)
        {
            await Debug.Log($"{user.Username} left {user.Guild.Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// User unbanned handler
        /// </summary>
        /// <param name="user">User that was unbanned</param>
        /// <param name="guild">Guild where user was unbanned</param>
        /// <returns></returns>
        public static async Task UserUnbannedHandler(SocketUser user, SocketGuild guild)
        {
            await Debug.Log($"{user.Username} was unbanned at {guild.Name}", LogInfoType.Notice);
        }

        /// <summary>
        /// User updated handler
        /// </summary>
        /// <param name="oldUser">User before update</param>
        /// <param name="updatedUser">User after update</param>
        /// <returns></returns>
        public static async Task UserUpdatedHandler(SocketUser oldUser, SocketUser updatedUser) { }

        /// <summary>
        /// User voice state upodated handler
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="oldState">Old voice state</param>
        /// <param name="newState">New voice state</param>
        /// <returns></returns>
        public static async Task UserVoiceStateUpdatedHandler(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            if (oldState.VoiceChannel == null && newState.VoiceChannel != null)
            {
                await Debug.Log($"{user.Username} has joined {newState.VoiceChannel.Name}", LogInfoType.Notice);
            }
            else if (oldState.VoiceChannel != null && newState.VoiceChannel != null && (oldState.VoiceChannel.Id != newState.VoiceChannel.Id))
            {
                await Debug.Log($"{user.Username} had switched voice channel to {newState.VoiceChannel.Name} from {oldState.VoiceChannel.Name}", LogInfoType.Notice);
            }
            else if (oldState.VoiceChannel != null && newState.VoiceChannel == null)
            {
                await Debug.Log($"{user.Username} had left voice channel {oldState.VoiceChannel.Name}", LogInfoType.Notice);
            }
        }

        /// <summary>
        /// Voice server updated handler
        /// </summary>
        /// <param name="voiceServer">Voice server</param>
        /// <returns></returns>
        public static async Task VoiceServerUpdatedHandler(SocketVoiceServer voiceServer)
        {
            await Debug.Log($"Joined voice server", LogInfoType.Notice);
        }
    }
}
