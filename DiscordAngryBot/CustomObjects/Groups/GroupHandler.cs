﻿using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Data;
using DiscordAngryBot.CustomObjects.SQLIteHandler;
using System.Threading;
using System.Diagnostics;
using DiscordAngryBot.ReactionHandlers;
using DiscordAngryBot.Models;
using Debug = DiscordAngryBot.CustomObjects.ConsoleOutput.Debug;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Extension class for handling groups
    /// </summary>
    public static class GroupHandler
    {
        /// <summary>
        /// Form message that will represent this group
        /// </summary>
        /// <param name="group">Group object</param>
        /// <returns></returns>
        private static string FormMessage(Group group)
        {
            Debug.Log($"Forming message...").GetAwaiter().GetResult();
            StringBuilder messageBuilder = new StringBuilder();
            Debug.Log($"Checking group type: {group.Type}").GetAwaiter().GetResult();
            switch (group.Type)
            {
                case GroupType.Simple:
                    messageBuilder.Append(
                        $"Собирается группа пользователем {group.Author.Mention}: {group.Destination}\n" +
                        $"__**Осталось {group.UserLists[0].UserLimit - group.UserLists[0].Users.Count()} мест**__\n");
                    break;
                case GroupType.GuildFight:
                    messageBuilder.Append($"Собирается группа на битвы БШ пользователем {group.Author.Mention}: {group.Destination}\n");
                    break;
                case GroupType.Poll:
                    messageBuilder.Append($"Пользователь {group.Author.Mention} предлагает проголосовать:\n");
                    break;
            }
            Debug.Log($"Iterating user lists...").GetAwaiter().GetResult();
            foreach (var list in group.UserLists)
            {
                Debug.Log($"Writing list: {list.ListName}").GetAwaiter().GetResult();
                messageBuilder.Append($"{list.ListName} ({list.ListEmoji.Name}):\n");
                Debug.Log($"Iteraring users in list...").GetAwaiter().GetResult();
                foreach (var user in list.Users)
                {
                    Debug.Log($"Writing user: {user.Mention}").GetAwaiter().GetResult();
                    messageBuilder.Append($"> {user.Mention}\n");
                }
            }
            Debug.Log($"Finished forming message.").GetAwaiter().GetResult();
            return messageBuilder.ToString();
        }

        /// <summary>
        /// Send initial group message and reactions
        /// </summary>
        /// <param name="group">Group object</param>
        /// <returns></returns>
        public static async Task SendMessage(this Group group)
        {
            group.TargetMessage = await group.Channel.SendMessageAsync(FormMessage(group));
            var emojis = group.UserLists.Select(x => x.ListEmoji).ToList();
            emojis.Add(EmojiGetter.GetEmoji(Emojis.ExclamationMark));
            emojis.Add(EmojiGetter.GetEmoji(Emojis.CrossMark));
            await group.TargetMessage.AddReactionsAsync(emojis.ToArray());
        }

        /// <summary>
        /// Edit group message to actual group data
        /// </summary>
        /// <param name="group">Группа</param>
        /// <returns></returns>
        public static async Task RewriteMessage(this Group group)
        {
            await Debug.Log($"Preparing to modify message: {group.TargetMessage.Id}");
            await group.TargetMessage.ModifyAsync(m => { m.Content = FormMessage(group); });
            await Debug.Log($"Message modifying finished.");
        }

        /// <summary>
        /// Redact message to represent closed group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static async Task RewriteMessageOnCancel(this Group group)
        {
            switch (group.Type)
            {
                case GroupType.Simple:
                    await group.TargetMessage.ModifyAsync(m => 
                    {
                        m.Content = $"Сбор группы {group.Author.Mention} ({group.Destination}) завершен."; 
                    });
                    break;
                case GroupType.GuildFight:
                    await group.TargetMessage.ModifyAsync(m => 
                    { 
                        m.Content = $"Сбор битв БШ ({group.Destination}) завершен."; 
                    });
                    break;
                case GroupType.Poll:
                    break;
            }
            await group.TargetMessage.RemoveAllReactionsAsync();
        }

        /// <summary>
        /// Serialize group data to JSON
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public static async Task<string> SerializeToJson(this Group group)
        {
            using (MemoryStream serializationStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(serializationStream, group.GetReference());
                var jsonResult = Encoding.UTF8.GetString(serializationStream.ToArray());
                return jsonResult;
            }
        }

        /// <summary>
        /// Deserialize JSON reference to Group object
        /// </summary>
        /// <param name="jsonText">JSON data</param>
        /// <returns></returns>
        public static async Task<Group> DeserializeFromJson(string jsonText)
        {
            try
            {
                using (MemoryStream serializationStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonText)))
                {
                    var groupDataObject = await JsonSerializer.DeserializeAsync<GroupReference>(serializationStream);
                    var group = groupDataObject.LoadOrigin();
                    return group;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Save Group object to DB
        /// </summary>
        /// <param name="group">Group object</param>
        /// <returns></returns>
        public static async Task SaveToDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            string query = $"INSERT INTO Groups (GUID, GroupJSON) VALUES('{group.GUID}', '{jsonString}')";
            await SQLiteExtensions.PushToDB($"locals/Databases/{group.Channel.Guild.Id}/Groups.sqlite", query);
        }

        /// <summary>
        /// Update group entry at DB
        /// </summary>
        /// <param name="group">Group object</param>
        /// <returns></returns>
        public static async Task UpdateAtDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            string query = $"UPDATE Groups SET GroupJSON = '{jsonString}' WHERE GUID = '{group.GUID}'";
            await SQLiteExtensions.PushToDB($"locals/Databases/{group.Channel.Guild.Id}/Groups.sqlite", query);
        }

        /// <summary>
        /// Remove group entry from DB
        /// </summary>
        /// <param name="group">Group object</param>
        /// <returns></returns>
        public static async Task RemoveFromDB(this Group group)
        {
            var jsonString = await group.SerializeToJson();
            string query = $"DELETE FROM Groups WHERE GUID = '{group.GUID}'";
            await SQLiteExtensions.PushToDB($"locals/Databases/{group.Channel.Guild.Id}/Groups.sqlite", query);
        }

        /// <summary>
        /// Load all group data from DB
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <returns></returns>
        public static async Task<List<Group>> LoadAllGroupsFromDB(ulong guildID)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string query = "SELECT * FROM Groups";
            DataTable data = await SQLiteExtensions.GetDataFromDB($"locals/Databases/{guildID}/Groups.sqlite", query);
            List<Group> groups = new List<Group>();
            List<Thread> threads = new List<Thread>();
            foreach (DataRow row in data.AsEnumerable())
            {
                Thread loadThread = new Thread(() =>
                {
                    try
                    {
                        var nextGroup = GroupBuilder.BuildLoadedGroup(row["GUID"].ToString(), row["GroupJSON"].ToString()).GetAwaiter().GetResult();
                        groups.Add(nextGroup);
                    }
                    catch (GroupInvalidTargetException groupInvalidTargetException)
                    {
                        Debug.Log("Group is missing target message", LogInfoType.Error).GetAwaiter().GetResult();
                        SQLiteExtensions.PushToDB($"locals/Databases/{guildID}/Groups.sqlite", $"DELETE FROM Groups WHERE GUID = '{groupInvalidTargetException.GroupGUID}'").GetAwaiter().GetResult();
                    }
                    catch (GroupInvalidAuthorException groupInvalidAuthorException)
                    {
                        Debug.Log("Group is missing author", LogInfoType.Error).GetAwaiter().GetResult();
                        SQLiteExtensions.PushToDB($"locals/Databases/{guildID}/Groups.sqlite", $"DELETE FROM Groups WHERE GUID = '{groupInvalidAuthorException.GroupGUID}'").GetAwaiter().GetResult();
                        if (BotCore.TryGetExtendedDiscordGuildBotData(guildID, out var extendedDiscordGuildData))
                        {
                            extendedDiscordGuildData.Guild.GetTextChannel(groupInvalidAuthorException.ChannelID).DeleteMessageAsync(groupInvalidAuthorException.GroupMessageID).GetAwaiter().GetResult();
                        }
                    }
                    catch (GroupInvalidChannelException groupInvalidChannelException)
                    {
                        Debug.Log("Group is missing channel", LogInfoType.Error).GetAwaiter().GetResult();
                        SQLiteExtensions.PushToDB($"locals/Databases/{guildID}/Groups.sqlite", $"DELETE FROM Groups WHERE GUID = '{groupInvalidChannelException.GroupGUID}'").GetAwaiter().GetResult();
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Error: {e.Message}", LogInfoType.Error).GetAwaiter().GetResult();
                    }
                });
                loadThread.Start();
                threads.Add(loadThread);
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            sw.Stop();
            await Debug.Log($"Group loading took {sw.ElapsedMilliseconds} ms.", LogInfoType.Notice);
            return groups;
        }

        /// <summary>
        /// Actualize reactions to actual state
        /// </summary>
        /// <param name="guild">Discord guild</param>
        /// <returns></returns>
        public static void ActualizeReactionsOnGroups(SocketGuild guild)
        {
            if (BotCore.TryGetDiscordGuildGroups(guild.Id, out List<Group> groups))
            {
                foreach (var group in groups)
                {
                    if (group.TargetMessage != null)
                    {
                        try
                        {
                            bool rewriteRequired = false;
                            foreach (var userList in group.UserLists)
                            {
                                if (group.TargetMessage.Reactions.TryGetValue(userList.ListEmoji, out var metaData))
                                {
                                    var amountOfReactionsInMessage = metaData.ReactionCount;
                                    var reactedUsers = group.TargetMessage.GetReactionUsersAsync(userList.ListEmoji, amountOfReactionsInMessage).Flatten();
                                    foreach (var user in reactedUsers.ToEnumerable())
                                    {
                                        if (!user.IsBot)
                                        {
                                            bool shouldAttemptJoin = false;
                                            if (userList.UserLimit.HasValue)
                                            {
                                                if (userList.UserLimit.Value <= userList.Users.Count)
                                                    shouldAttemptJoin = false;
                                                else
                                                    shouldAttemptJoin = true;
                                            }
                                            else
                                                shouldAttemptJoin = true;

                                            if (shouldAttemptJoin)
                                            {
                                                if (!group.IsUserInGroup(user.Id))
                                                {
                                                    if (userList.TryJoin(guild.GetUser(user.Id)))
                                                    {
                                                        rewriteRequired = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (rewriteRequired)
                            {
                                group.UpdateAtDB().GetAwaiter().GetResult();
                                group.RewriteMessage().GetAwaiter().GetResult();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.Log(ex.Message, LogInfoType.Error).GetAwaiter().GetResult();
                        }
                    }
                    else
                    {
                        Debug.Log("Found broken party, deleting entry...", LogInfoType.Error).GetAwaiter().GetResult();
                        SQLiteExtensions.PushToDB($"locals/Databases/{group.Channel.Guild.Id}/Groups.sqlite", $"DELETE FROM Groups WHERE GUID = '{group.GUID}'").GetAwaiter().GetResult();
                    }
                }
            }
        }

        /// <summary>
        /// Try to get user list from group by emoji
        /// </summary>
        /// <param name="group">Group object</param>
        /// <param name="emoji">List emoji</param>
        /// <param name="userList">Found user list, if any</param>
        /// <returns></returns>
        public static bool TryGetUserList(this Group group, Emoji emoji, out UserList userList)
        {
            userList = group.UserLists.FirstOrDefault(x => x.ListEmoji.Name == emoji.Name);           
            if (userList != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Try to find group in list
        /// </summary>
        /// <param name="groups">Groups list</param>
        /// <param name="groupMessageID">Group target message ID</param>
        /// <param name="group">Found group, if any</param>
        /// <returns></returns>
        public static bool TryFindGroup(this List<Group> groups, ulong groupMessageID, out Group group)
        {
            group = groups.FirstOrDefault(x => x.TargetMessage.Id == groupMessageID);
            if (group != null)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks whether user with given ID is present is this group
        /// </summary>
        /// <param name="group">Target group</param>
        /// <param name="userID">Socket user ID</param>
        /// <returns></returns>
        public static bool IsUserInGroup(this Group group, ulong userID)
        {
            var groupLists = group.UserLists;
            foreach (var list in groupLists)
            {
                if (list.IsInList(userID))
                {
                    return true;
                }
            }
            return false;
        }
    }   
}
