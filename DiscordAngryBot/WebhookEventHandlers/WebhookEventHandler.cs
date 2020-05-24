using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.Filters;
using DiscordAngryBot.CustomObjects.Groups;
using DiscordAngryBot.CustomObjects.Notifications;
using DiscordAngryBot.MessageHandlers;
using DiscordAngryBot.ReactionHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.WebhookEventHandlers
{
    /// <summary>
    /// Класс-обработчик событий клиента дискорда
    /// </summary>
    public static class WebhookEventHandler
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
                    await ConsoleWriter.Write($"{message.Message}: {message.Exception}", ConsoleWriter.InfoType.Error);
                    await PushNotificator.Notify(message);
                    break;
                case LogSeverity.Warning:
                    await ConsoleWriter.Write($"{message.Message}: {message.Exception}", ConsoleWriter.InfoType.Error);
                    await PushNotificator.Notify(message);
                    break;
                case LogSeverity.Info:
                    await ConsoleWriter.Write($"{message.Message}", ConsoleWriter.InfoType.Info);
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
            BotSettings settings = Program.FetchSettings();
            DataHandler systemData = Program.FetchData();
            DiscordServerObject serverObject = Program.FetchServerObject();
            // Проверка, от бота ли сообщение
            if (!message.Author.IsBot)
            {
                if (message.Content.Count() > 0)
                {
                    await ConsoleWriter.Write($"[#{message.Channel}] {message.Author.Username}: {message.Content}", ConsoleWriter.InfoType.Chat);
                    if ((message.Channel.Id != 636222368028688385) && (await message.CheckPhrase() && Program.swearCounters.Where( x => x.author == message.Author).Count() == 0))
                    {
                        Program.swearCounters.Add(await message.CreateSwearCounter());
                        
                    }
                    else if ((message.Channel.Id != 636222368028688385) && (await message.CheckPhrase() && Program.swearCounters.Where(x => x.author == message.Author).Count() == 1))
                    {
                        await Program.swearCounters.Where(x => x.author == message.Author).SingleOrDefault().reasons.AddReason(message);
                    }
                }
                else
                {
                    await ConsoleWriter.Write($"[#{message.Channel}] {message.Author.Username}: Пустое сообщение", ConsoleWriter.InfoType.Chat);
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
                                        systemData.bans.Add(await CommandHandler.SystemCommands.BanUser(serverObject, message, commandParameters));
                                        break;
                                    case "CLEAR":
                                        await CommandHandler.SystemCommands.ClearMessages(message, args);
                                        break;
                                }
                            }
                            else
                            {
                                await channel.SendMessageAsync($"Недостаточно прав на команду.");
                            }
                        }
                        else if (settings.userCommands.Contains(command.ToUpper()))
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
                                case "GVG":
                                    await CommandHandler.UserCommands.CreateGuildFight(systemData.groups, message, args);
                                    break;
                                case "HELP":
                                    await CommandHandler.UserCommands.HelpUser(message.Author, args);
                                    try
                                    {
                                        await message.DeleteAsync();
                                    }
                                    catch (Exception ex)
                                    {
                                        await ConsoleWriter.Write(ex.Message, ConsoleWriter.InfoType.Error);
                                    }
                                    break;
                            }
                        }
                        else if (settings.musicCommands.Contains(command.ToUpper()))
                        {
                            switch (command.ToUpper())
                            {
                                case "SUMMON":
                                    await CommandHandler.MusicCommands.JoinChannel(message);
                                    break;
                            }
                        }
                        else if (settings.otherCommands.Contains(command.ToUpper()))
                        {
                            switch (command.ToUpper())
                            {
                                case "КУСЬ":
                                    await CommandHandler.OtherCommands.Bite(message, args);
                                    break;
                                case "БАН":
                                    await CommandHandler.OtherCommands.Ban(message);
                                    break;
                            }
                        }
                        else
                        {
                            await message.DeleteAsync();
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
            DataHandler systemData = Program.FetchData();
            if (reaction.ValidateReaction(new Emoji("\u2705"))) // white_check_mark
            {
                var message = await messageChannel.GetMessageAsync(cache.Id);
                Group group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                if (group != null)
                {
                    await ReactionHandler.PartyReactionHandler.JoinGroup(group, message, reaction, systemData.groups);
                }
            }
            if (reaction.ValidateReaction(new Emoji("\u274C"))) // cross_mark
            {
                var message = await messageChannel.GetMessageAsync(cache.Id);
                Group group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                if (group != null)
                {
                    await ReactionHandler.PartyReactionHandler.TerminateGroup(group, message, reaction, systemData.groups);
                }
            }
            if (reaction.ValidateReaction(new Emoji("\u2757"))) // exclamation
            {
                var message = await messageChannel.GetMessageAsync(cache.Id);
                Group group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                if (group != null)
                {
                    await ReactionHandler.PartyReactionHandler.GroupCallout(group, reaction);
                }
            }
            if (reaction.ValidateReaction(new Emoji("🐾")) || reaction.ValidateReaction(new Emoji("🐷")) || reaction.ValidateReaction(new Emoji("❓")))
            {
                var message = await messageChannel.GetMessageAsync(cache.Id);
                Group group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                if (group != null)
                {
                    await ReactionHandler.PartyReactionHandler.JoinGuildFight(group, message, reaction, systemData.groups);
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
            await ConsoleWriter.Write($"{reaction.User} removed reaction {reaction.Emote.Name} from {cache.Id}", ConsoleWriter.InfoType.Info);
            DataHandler systemData = Program.FetchData();
            if (reaction.ValidateReaction(new Emoji("\u2705"))) // white_check_mark
            {
                var message = await messageChannel.GetMessageAsync(cache.Id);
                Group group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                if (group != null)
                {
                    await ReactionHandler.PartyReactionHandler.LeaveGroup(group, message, reaction, systemData.groups);
                }
            }
            if (reaction.ValidateReaction(new Emoji("🐾")) || reaction.ValidateReaction(new Emoji("🐷")) || reaction.ValidateReaction(new Emoji("❓")))
            {
                var message = await messageChannel.GetMessageAsync(cache.Id);
                Group group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                if (group != null)
                {
                    await ReactionHandler.PartyReactionHandler.LeaveGuildFight(group, message, reaction, systemData.groups);
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
