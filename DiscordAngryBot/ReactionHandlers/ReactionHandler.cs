using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Groups;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordAngryBot.ReactionHandlers
{
    public static class ReactionHandler
    {
        public static class PartyReactionHandler
        {
            /// <summary>
            /// Обработка вступления в группу
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            public async static void JoinGroup(Group groupObject, IMessage message, SocketReaction reaction, List<Group> groups)
            {
                if (groupObject.IsParty())
                {
                    var party = (Party)groupObject;
                    party.AddUser((SocketUser)reaction.User);
                    await party.UpdateAtDB();
                    await party.RewriteMessage();
                    if (party.users.Count == 6)
                    {
                        await message.Channel.SendMessageAsync($"Группа персонажа {party.author.Mention} собрана");
                        party.isActive = false;
                        await party.UpdateAtDBIfFull();
                        groups.Remove(party);
                        party.Dispose();
                    }
                }
                else if (groupObject.IsRaid())
                {
                    var raid = (Raid)groupObject;
                    raid.AddUser((SocketUser)reaction.User);
                    await raid.UpdateAtDB();
                    await raid.RewriteMessage();
                    if (raid.users.Count == 12)
                    {
                        await message.Channel.SendMessageAsync($"Группа персонажа {raid.author.Mention} собрана");
                        raid.isActive = false;
                        await raid.UpdateAtDBIfFull();
                        groups.Remove(raid);
                        raid.Dispose();
                    }
                }
            }

            /// <summary>
            /// Обработка выхода из группы
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            public async static void LeaveGroup(Group groupObject, IMessage message, SocketReaction reaction, List<Group> groups)
            {
                if (groupObject.IsParty())
                {
                    var party = (Party)groupObject;
                    bool isInParty = groups.Where(x => x.users.Contains((SocketUser)reaction.User.Value)).Count() > 0 == true;
                    if (isInParty)
                    {
                        party.RemoveUser((SocketUser)reaction.User);
                        await party.UpdateAtDB();
                        await party.RewriteMessage();
                        return;
                    }
                }
                else if (groupObject.IsRaid())
                {
                    var raid = (Raid)groupObject;
                    bool isInRaid = groups.Where(x => x.users.Contains((SocketUser)reaction.User.Value)).Count() > 0 == true;
                    if (isInRaid)
                    {
                        raid.RemoveUser((SocketUser)reaction.User);
                        await raid.UpdateAtDB();
                        await raid.RewriteMessage();
                        return;
                    }
                }
            }

            /// <summary>
            /// Уничтожение объекта группы
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            public async static void TerminateGroup(Group groupObject, IMessage message, SocketReaction reaction, List<Group> groups)
            {
                if (groupObject.IsParty())
                {
                    var party = groups.Where(x => x.targetMessage.Id == reaction.MessageId).SingleOrDefault();

                    if (party != null && party.author.Id == reaction.UserId)
                    {
                        await party.targetMessage.DeleteAsync();
                        await message.Channel.SendMessageAsync($"Сбор группы {party.author.Mention} отменен");
                        await party.RemoveFromDB();
                        groups.Remove(party);
                    }
                }
                else if (groupObject.IsRaid())
                {
                    var raid = groups.Where(x => x.targetMessage.Id == reaction.MessageId).SingleOrDefault();
                    if (raid != null && raid.author.Id == reaction.UserId)
                    {
                        await raid.targetMessage.DeleteAsync();
                        await message.Channel.SendMessageAsync($"Сбор рейда {raid.author.Mention} отменен");
                        await raid.RemoveFromDB();
                        groups.Remove(raid);
                    }
                }
            }
        }
    }
}
