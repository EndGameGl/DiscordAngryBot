using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.Groups;
using DiscordAngryBot.MessageHandlers;
using DiscordAngryBot.ReactionHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.WebhookEventHandlers
{
    public static class WebhookEventHandler
    {
        public static async Task LogHandler(LogMessage message)
        {
            switch (message.Severity)
            {
                case LogSeverity.Error:
                    await ConsoleWriter.Write($"{message.Message}: {message.Exception}", ConsoleWriter.InfoType.Error);
                    break;
                case LogSeverity.Warning:
                    await ConsoleWriter.Write($"{message.Message}: {message.Exception}", ConsoleWriter.InfoType.Error);
                    break;
                case LogSeverity.Info:
                    await ConsoleWriter.Write($"{message.Message}", ConsoleWriter.InfoType.Info);
                    break;
            }
        }
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
                                    case "GVG":
                                        await CommandHandler.UserCommands.CreateGuildFight(systemData.groups, message, args);
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
        public static async Task ReactionAddedHandler(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
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
        }
        public static async Task ReactionRemovedHandler(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel messageChannel, SocketReaction reaction)
        {
            DataHandler systemData = Program.FetchData();
            if (reaction.ValidateReaction(new Emoji("\u2705"))) // white_check_mark
            {
                var message = await messageChannel.GetMessageAsync(cache.Id);

                Group group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                if (group == null)
                {
                    group = systemData.groups.Where(x => x.targetMessage.Id == message.Id).SingleOrDefault();
                }
                if (group != null)
                {
                    await ReactionHandler.PartyReactionHandler.LeaveGroup(group, message, reaction, systemData.groups);
                }
            }
        }
        public static async Task ChannelCreatedHandler(SocketChannel channel)
        {
            await ConsoleWriter.Write($"Channel {((IChannel)channel).Name} created", ConsoleWriter.InfoType.Notice);
        }
        public static async Task ChannelDestroyedHandler(SocketChannel channel)
        {
            await ConsoleWriter.Write($"Channel {((IChannel)channel).Name} destroyed", ConsoleWriter.InfoType.Notice);
        }
        public static async Task ChannelUpdatedHandler(SocketChannel oldChannel, SocketChannel updatedChannel)
        {
            await ConsoleWriter.Write($"Channel {((IChannel)oldChannel).Name} updated into {((IChannel)updatedChannel).Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task ConnectedHandler()
        {
            await ConsoleWriter.Write($"Connected to servers", ConsoleWriter.InfoType.Notice);
        }
        public static async Task CurrentUserUpdatedHandler(SocketSelfUser oldUser, SocketSelfUser updatedUser)
        {
            await ConsoleWriter.Write($"{oldUser.Username} updated to {updatedUser.Username}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task DisconnectedHandler(Exception exception)
        {
            await ConsoleWriter.Write($"{exception.Message}", ConsoleWriter.InfoType.Error);
        }
        public static async Task GuildAvailableHandler(SocketGuild guild)
        {
            await ConsoleWriter.Write($"{guild.Name} is available", ConsoleWriter.InfoType.Notice);
        }
        public static async Task GuildMembersDownloadedHandler(SocketGuild guild)
        {
            await ConsoleWriter.Write($"{guild.Name} members downloaded, {guild.MemberCount} members", ConsoleWriter.InfoType.Notice);
        }
        public static async Task GuildMemberUpdatedHandler(SocketGuildUser oldUser, SocketGuildUser updatedUser)
        {
            //await ConsoleWriter.Write($"{updatedUser.Username} user entity was updated", ConsoleWriter.InfoType.Notice);
        }
        public static async Task GuildUnavailableHandler(SocketGuild guild)
        {
            await ConsoleWriter.Write($"{guild.Name} is unavailable", ConsoleWriter.InfoType.Notice);
        }
        public static async Task GuildUpdatedHandler(SocketGuild oldGuild, SocketGuild updatedGuild)
        {
            await ConsoleWriter.Write($"{updatedGuild.Name} guild was updated", ConsoleWriter.InfoType.Notice);
        }
        public static async Task JoinedGuildHandler(SocketGuild guild)
        {
            await ConsoleWriter.Write($"Joined \"{guild.Name}\" guild", ConsoleWriter.InfoType.Notice);
        }
        public static async Task LatencyUpdatedHandler(int oldLatency, int newLatency)
        {

        }
        public static async Task LeftGuildHandler(SocketGuild guild)
        {
            await ConsoleWriter.Write($"Left \"{guild.Name}\" guild", ConsoleWriter.InfoType.Notice);
        }
        public static async Task LoggedInHandler()
        {
            await ConsoleWriter.Write($"Logged in", ConsoleWriter.InfoType.Notice);
        }
        public static async Task LoggedOutHandler()
        {
            await ConsoleWriter.Write($"Logged out", ConsoleWriter.InfoType.Notice);
        }
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
        public static async Task MessageUpdatedHandler(Cacheable<IMessage, ulong> cachedMessage, SocketMessage message, ISocketMessageChannel channel)
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
        public static async Task ReactionsClearedHandler(Cacheable<IUserMessage, ulong> userMessageCache, ISocketMessageChannel channel)
        {
            await ConsoleWriter.Write($"Cleared all reactions from message {userMessageCache.Id} in channel {channel.Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task RecipientAddedHandler(SocketGroupUser user) 
        {
            await ConsoleWriter.Write($"{user.Username} was added to {user.Channel.Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task RecipientRemovedHandler(SocketGroupUser user)
        {
            await ConsoleWriter.Write($"{user.Username} was removed from {user.Channel.Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task RoleCreatedHandler(SocketRole role)
        {
            await ConsoleWriter.Write($"{role.Name} was created in {role.Guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task RoleDeletedHandler(SocketRole role)
        {
            await ConsoleWriter.Write($"{role.Name} was deleted from {role.Guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task RoleUpdatedHandler(SocketRole oldRole, SocketRole updatedRole)
        {
            await ConsoleWriter.Write($"{updatedRole.Name} was updated at {updatedRole.Guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task UserBannedHandler(SocketUser user, SocketGuild guild)
        {
            await ConsoleWriter.Write($"{user.Username} was banned from {guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task UserIsTypingHandler(SocketUser user, ISocketMessageChannel channel)
        {
            // ffs do nothing please
        }
        public static async Task UserJoinedHandler(SocketGuildUser user)
        {
            await ConsoleWriter.Write($"{user.Username} joined {user.Guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task UserLeftHandler(SocketGuildUser user)
        {
            await ConsoleWriter.Write($"{user.Username} left {user.Guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task UserUnbannedHandler(SocketUser user, SocketGuild guild)
        {
            await ConsoleWriter.Write($"{user.Username} was unbanned at {guild.Name}", ConsoleWriter.InfoType.Notice);
        }
        public static async Task UserUpdatedHandler(SocketUser oldUser, SocketUser updatedUser)
        {
            //await ConsoleWriter.Write($"{updatedUser.Username} was updated", ConsoleWriter.InfoType.Info);
        }
        public static async Task UserVoiceStateUpdatedHandler(SocketUser user, SocketVoiceState oldState, SocketVoiceState newState)
        {
            if (newState.VoiceChannel != null)
            {
                await ConsoleWriter.Write($"{user.Username} voice state was updated: {newState.VoiceChannel.Name}", ConsoleWriter.InfoType.Info);
            }
            else
            {
                await ConsoleWriter.Write($"{user.Username} had left voice channel {oldState.VoiceChannel.Name}", ConsoleWriter.InfoType.Info);
            }
        }
        public static async Task VoiceServerUpdatedHandler(SocketVoiceServer voiceServer)
        {
            await ConsoleWriter.Write($"Joined voice server", ConsoleWriter.InfoType.Info);
        }
    }
}
