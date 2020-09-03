﻿using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Caches;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.CustomObjects.DiscordCommands;
using DiscordAngryBot.CustomObjects.Groups;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.ReactionHandlers
{
    /// <summary>
    /// Класс для обработки реакций к сообщениям
    /// </summary>
    public static class ReactionHandler
    {
        /// <summary>
        /// Обработка реакций, связанных со сбором групп
        /// </summary>
        public static class PartyReactionHandler
        {
            /// <summary>
            /// Обработка вступления в группу
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            [CustomCommand("JOINGROUP", CommandCategory.User, CommandType.EmojiCommand, "Вступление в группу", CommandScope.User, CommandExecutionScope.Server)]
            public async static Task JoinGroup(Group group, SocketReaction reaction)
            {
                await Debug.Log($"Trying to join group {group.GUID} with reaction: {reaction.Emote.Name}. Getting user list...");
                if (group.TryGetUserList((Emoji)reaction.Emote, out UserList userList))
                {
                    await Debug.Log($"Got user list: {userList.ListName}");
                    if (!userList.TryJoin((SocketGuildUser)reaction.User))
                    {
                        await Debug.Log($"Failed to join the group.");
                        await group.TargetMessage.RemoveReactionAsync(reaction.Emote, reaction.UserId);
                    }
                    else
                    {
                        await Debug.Log($"Successfully joined group. Updating group data.");
                        await group.UpdateAtDB();
                        await Debug.Log($"Rewriting message...");
                        await group.RewriteMessage();
                        await Debug.Log($"Task done.");
                    }                    
                }
            }

            /// <summary>
            /// Обработка выхода из группы
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            [CustomCommand("LEAVEGROUP", CommandCategory.User, CommandType.EmojiCommand, "Выход из группы", CommandScope.User, CommandExecutionScope.Server)]
            public async static Task LeaveGroup(Group group, SocketReaction reaction)
            {
                if (group.TryGetUserList((Emoji)reaction.Emote, out UserList userList))
                {
                    if (!userList.RemoveUserIfExists(reaction.UserId))
                    {
                        await group.TargetMessage.RemoveReactionAsync(reaction.Emote, reaction.UserId);
                    }
                    else
                    {
                        await group.UpdateAtDB();
                        await group.RewriteMessage();
                    }
                }
            }

            /// <summary>
            /// Уничтожение объекта группы
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            [CustomCommand("TERMINATEGROUP", CommandCategory.User, CommandType.EmojiCommand, "Удаление группы", CommandScope.User, CommandExecutionScope.Server)]
            public async static Task TerminateGroup(Group group, SocketReaction reaction)
            {
                if (group != null)
                {
                    if (BotCore.TryGetDiscordGuildSettings(group.Channel.Guild.Id, out DiscordGuildSettings settings))
                    {
                        if (settings.IsAdmin(reaction.UserId) || (group.Author.Id == reaction.UserId))
                        {
                            await group.RewriteMessageOnCancel();
                            await group.RemoveFromDB();
                            if (BotCore.TryGetDiscordGuildGroups(((SocketGuildUser)reaction.User).Guild.Id, out List<Group> groups))
                            {
                                groups.Remove(group);
                            }
                        }
                    }
                }               
            }

            /// <summary>
            /// Объявление сбора группы
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="reaction"></param>
            /// <returns></returns>
            [CustomCommand("CALLGROUP", CommandCategory.User, CommandType.EmojiCommand, "Сбор", CommandScope.User, CommandExecutionScope.Server)]
            public async static Task GroupCallout(Group group, SocketReaction reaction)
            {
                StringBuilder stringBuilder = new StringBuilder();
                if (group.Type != GroupType.Poll)
                {
                    if (reaction.User.IsSpecified)
                    {
                        stringBuilder.AppendLine($"{reaction.User.Value.Mention} объявляет сбор группы: {group.Destination}");
                    }
                    else
                    {
                        stringBuilder.AppendLine($"{group.Author.Mention} объявляет сбор группы: {group.Destination}");
                    }
                }
                else
                {
                    stringBuilder.AppendLine($"Голосование завершено: {group.Destination}");
                }
                foreach (var list in group.UserLists)
                {
                    stringBuilder.AppendLine($"{list.ListName}:");
                    foreach (var user in list.Users)
                    {
                        stringBuilder.AppendLine($"> {user.Mention}");
                    }
                }
                await group.Channel.SendMessageAsync(stringBuilder.ToString());
            }
        }
    }
}
