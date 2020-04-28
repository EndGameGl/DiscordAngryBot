using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
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
            public async static Task JoinGroup(Group groupObject, IMessage message, SocketReaction reaction, List<Group> groups)
            {
                if (groupObject.isActive)
                {
                    if (groupObject.IsParty())
                    {
                        var party = (Party)groupObject;
                        if (party.users.Count < party.userLimit)
                        {
                            party.AddUser((SocketUser)reaction.User);
                            await party.UpdateAtDB();
                            await party.RewriteMessage();
                        }
                        if (party.users.Count == 6)
                        {
                            //party.isActive = false;
                            await party.author.SendMessageAsync($"Ваш группа [{party.destination}] была собрана");
                            //await party.UpdateAtDBIfFull();
                        }
                    }
                    else if (groupObject.IsRaid())
                    {
                        var raid = (Raid)groupObject;
                        if (raid.users.Count < raid.userLimit)
                        {
                            raid.AddUser((SocketUser)reaction.User);
                            await raid.UpdateAtDB();
                            await raid.RewriteMessage();
                        }
                        if (raid.users.Count == 12)
                        {
                            //raid.isActive = false;
                            await raid.author.SendMessageAsync($"Ваш рейд [{raid.destination}] был собран");
                            //await raid.UpdateAtDBIfFull();
                        }
                    }
                    else if (groupObject.IsGuildFight())
                    {
                        var guildFight = (GuildFight)groupObject;
                        guildFight.AddUser((SocketUser)reaction.User);
                        await guildFight.UpdateAtDB();
                        await guildFight.RewriteMessage();
                    }
                }
            }

            /// <summary>
            /// Обработка выхода из группы
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            public async static Task LeaveGroup(Group groupObject, IMessage message, SocketReaction reaction, List<Group> groups)
            {
                if (groupObject.isActive)
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
                    else if (groupObject.IsGuildFight())
                    {
                        var guildFight = (GuildFight)groupObject;
                        bool isInFight = groups.Where(x => x.users.Contains((SocketUser)reaction.User.Value)).Count() > 0 == true;
                        if (isInFight)
                        {
                            guildFight.RemoveUser((SocketUser)reaction.User);
                            await guildFight.UpdateAtDB();
                            await guildFight.RewriteMessage();
                            return;
                        }
                    }

                }
            }

            /// <summary>
            /// Уничтожение объекта группы
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            public async static Task TerminateGroup(Group groupObject, IMessage message, SocketReaction reaction, List<Group> groups)
            {
                if (groupObject.IsParty())
                {
                    var party = groups.Where(x => x.targetMessage.Id == reaction.MessageId).SingleOrDefault();

                    if (party != null && party.author.Id == reaction.UserId)
                    {
                        //await party.targetMessage.DeleteAsync();
                        //await message.Channel.SendMessageAsync($"Сбор группы {party.author.Mention} ({party.destination}) закончен");
                        await party.RewriteMessageOnCancel();
                        await party.RemoveFromDB();
                        groups.Remove(party);
                    }
                }
                else if (groupObject.IsRaid())
                {
                    var raid = groups.Where(x => x.targetMessage.Id == reaction.MessageId).SingleOrDefault();
                    if (raid != null && raid.author.Id == reaction.UserId)
                    {
                        //await raid.targetMessage.DeleteAsync();
                        //await message.Channel.SendMessageAsync($"Сбор рейда {raid.author.Mention} ({raid.destination}) закончен");
                        await raid.RewriteMessageOnCancel();
                        await raid.RemoveFromDB();
                        groups.Remove(raid);
                    }
                }
                else if (groupObject.IsGuildFight())
                {
                    var guildFight = groups.Where(x => x.targetMessage.Id == reaction.MessageId).SingleOrDefault();
                    if (guildFight != null && guildFight.author.Id == reaction.UserId)
                    {
                        //await guildFight.targetMessage.DeleteAsync();
                        //await message.Channel.SendMessageAsync($"Сбор на битвы БШ {guildFight.destination} закончен");
                        await guildFight.RewriteMessageOnCancel();
                        await guildFight.RemoveFromDB();
                        groups.Remove(guildFight);
                    }
                }
            }

            /// <summary>
            /// Объявление сбора группы
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="reaction"></param>
            /// <returns></returns>
            public async static Task GroupCallout(Group groupObject, SocketReaction reaction)
            {
                if (reaction.UserId == groupObject.author.Id)
                {
                    if (!groupObject.IsGuildFight())
                    {
                        StringBuilder calloutText = new StringBuilder();
                        calloutText.AppendLine($"{groupObject.author.Mention} объявляет сбор группы");
                        foreach (var user in groupObject.users)
                        {
                            calloutText.AppendLine($"{user.Mention}");
                        }
                        await groupObject.targetMessage.Channel.SendMessageAsync(calloutText.ToString());
                    }
                    else 
                    {
                        StringBuilder calloutText = new StringBuilder();
                        calloutText.AppendLine($"__**Сбор на битвы БШ!**__\nСостав:");
                        int memberCount = 0;
                        // Пробег по первому списку юзеров
                        foreach (var user in groupObject.users)
                        {                           
                            if (memberCount < 6)
                            {
                                calloutText.AppendLine($"> {user.Mention}");
                                memberCount++;
                            }
                            else if (memberCount == 6)
                                goto A;
                        }
 
                        // Пробег по второму списку
                        foreach (var user in ((GuildFight)groupObject).noGearUsers)
                        {                            
                            if (memberCount < 6)
                            {
                                calloutText.AppendLine($"> {user.Mention}");
                                memberCount++;
                            }
                            if (memberCount == 6)
                                goto A;
                        }

                        foreach (var user in ((GuildFight)groupObject).unwillingUsers)
                        {
                            if (memberCount < 6)
                            {
                                calloutText.AppendLine($"> {user.Mention}");
                                memberCount++;
                            }
                            if (memberCount == 6)
                                goto A;
                        }

                        foreach (var user in ((GuildFight)groupObject).unsureUsers)
                        {
                            if (memberCount < 6)
                            {
                                calloutText.AppendLine($"> {user.Mention}");
                                memberCount++;
                            }
                            if (memberCount == 6)
                                goto A;
                        }

                        A: await groupObject.targetMessage.Channel.SendMessageAsync(calloutText.ToString());
                    }
                }           
            }

            /// <summary>
            /// Обработка вступления на битву БШ
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            /// <param name="groups"></param>
            /// <returns></returns>
            public async static Task JoinGuildFight(Group groupObject, IMessage message, SocketReaction reaction, List<Group> groups)
            {
                if (groupObject.isActive)
                {
                    if (groupObject.IsGuildFight())
                    {
                        switch (reaction.Emote.Name)
                        {
                            case "🐾":
                                if (!((GuildFight)groupObject).users.Contains((SocketUser)reaction.User) &&
                                    !((GuildFight)groupObject).unwillingUsers.Contains((SocketUser)reaction.User) &&
                                    !((GuildFight)groupObject).unsureUsers.Contains((SocketUser)reaction.User))
                                {
                                    ((GuildFight)groupObject).noGearUsers.Add((SocketUser)reaction.User);
                                }
                                break;
                            case "🐷":
                                if (!((GuildFight)groupObject).users.Contains((SocketUser)reaction.User) &&
                                    !((GuildFight)groupObject).noGearUsers.Contains((SocketUser)reaction.User) &&
                                    !((GuildFight)groupObject).unsureUsers.Contains((SocketUser)reaction.User))
                                {
                                    ((GuildFight)groupObject).unwillingUsers.Add((SocketUser)reaction.User); 
                                }
                                break;
                            case "❓":
                                if (!((GuildFight)groupObject).users.Contains((SocketUser)reaction.User) &&
                                    !((GuildFight)groupObject).unwillingUsers.Contains((SocketUser)reaction.User) &&
                                    !((GuildFight)groupObject).noGearUsers.Contains((SocketUser)reaction.User))
                                {
                                    ((GuildFight)groupObject).unsureUsers.Add((SocketUser)reaction.User);
                                }
                                break;
                        }
                        await groupObject.UpdateAtDB();
                        await groupObject.RewriteMessage();
                    }
                }
            }

            /// <summary>
            /// Обработка выхода из битвы БШ
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            /// <param name="groups"></param>
            /// <returns></returns>
            public async static Task LeaveGuildFight(Group groupObject, IMessage message, SocketReaction reaction, List<Group> groups)
            {
                if (groupObject.isActive)
                {
                    if (groupObject.IsGuildFight())
                    {
                        switch (reaction.Emote.Name)
                        {
                            case "🐾":
                                ((GuildFight)groupObject).noGearUsers.Remove((SocketUser)reaction.User);
                                break;
                            case "🐷":
                                ((GuildFight)groupObject).unwillingUsers.Remove((SocketUser)reaction.User);
                                break;
                            case "❓":
                                ((GuildFight)groupObject).unsureUsers.Remove((SocketUser)reaction.User);
                                break;
                        }
                        await groupObject.UpdateAtDB();
                        await groupObject.RewriteMessage();
                    }
                }
            }
        }
    }
}
