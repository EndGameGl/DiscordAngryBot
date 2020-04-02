using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Parties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            public async static void JoinGroup(IGroup groupObject, IMessage message, SocketReaction reaction)
            {
                if (groupObject.IsParty())
                {
                    var party = (Party)groupObject;
                    party.Add((SocketUser)reaction.User.Value);
                    party.RewriteMessage();
                    if (party.users.Count == 6)
                    {
                        await message.Channel.SendMessageAsync($"Группа персонажа {party.author.Mention} собрана");
                        File.Delete(party.localPath);
                        Program.systemData.parties.Remove(party);
                        party.Dispose();
                    }
                }
                else if (groupObject.IsRaid())
                {
                    var raid = (Raid)groupObject;
                    raid.Add((SocketUser)reaction.User.Value);
                    raid.RewriteMessage();
                    if (raid.users.Count == 12)
                    {
                        await message.Channel.SendMessageAsync($"Группа персонажа {raid.author.Mention} собрана");
                        File.Delete(raid.localPath);
                        Program.systemData.raids.Remove(raid);
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
            public async static void LeaveGroup(IGroup groupObject, IMessage message, SocketReaction reaction)
            {
                if (groupObject.IsParty())
                {
                    var party = (Party)groupObject;
                    bool isInParty = Program.systemData.parties.Where(x => x.users.Contains(reaction.User.Value)).Count() > 0 == true;
                    if (isInParty)
                    {
                        party.Remove((SocketUser)reaction.User.Value);
                        party.RewriteMessage();
                        return;
                    }
                }
                else if (groupObject.IsRaid())
                {
                    var raid = (Raid)groupObject;
                    bool isInRaid = Program.systemData.raids.Where(x => x.users.Contains(reaction.User.Value)).Count() > 0 == true;
                    if (isInRaid)
                    {
                        raid.Remove((SocketUser)reaction.User.Value);
                        raid.RewriteMessage();
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
            public async static void TerminateGroup(IGroup groupObject, IMessage message, SocketReaction reaction)
            {
                if (groupObject.IsParty())
                {
                    var party = Program.systemData.parties.Where(x => x.targetMessage.Id == reaction.MessageId).SingleOrDefault();

                    if (party != null && party.author.Id == reaction.UserId)
                    {
                        await party.targetMessage.DeleteAsync();
                        await message.Channel.SendMessageAsync($"Сбор группы {party.author.Mention} отменен");
                        Program.systemData.parties.Remove(party);
                        File.Delete(party.localPath);
                    }
                }
                else if (groupObject.IsRaid())
                {
                    var raid = Program.systemData.raids.Where(x => x.targetMessage.Id == reaction.MessageId).SingleOrDefault();
                    if (raid != null && raid.author.Id == reaction.UserId)
                    {
                        await raid.targetMessage.DeleteAsync();
                        await message.Channel.SendMessageAsync($"Сбор рейда {raid.author.Mention} отменен");
                        Program.systemData.raids.Remove(raid);
                        File.Delete(raid.localPath);
                    }
                }
            }
        }
    }
}
