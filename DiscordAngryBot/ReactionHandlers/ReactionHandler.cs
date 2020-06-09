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
            public async static Task JoinGroup(SocketReaction reaction)
            {
                var group = BotCore.GetDiscordGuildGroups(((SocketGuildUser)reaction.User).Guild.Id).Where(x => x.TargetMessage.Id == reaction.MessageId).FirstOrDefault();
                if (group is Party)
                {
                    var party = group as Party;
                    if (party.Users.Count < party.UserLimit)
                    {
                        party.Users.Add((SocketGuildUser)reaction.User);
                        await party.UpdateAtDB();
                        await party.RewriteMessage();
                    }
                    if (party.Users.Count == 6)
                    {
                        await party.Author.SendMessageAsync($"Ваш группа [{party.Destination}] была собрана");
                    }
                }
                else if (group is Raid)
                {
                    var raid = group as Raid;
                    if (raid.Users.Count < raid.UserLimit)
                    {
                        raid.Users.Add((SocketGuildUser)reaction.User);
                        await raid.UpdateAtDB();
                        await raid.RewriteMessage();
                    }
                }
                else if (group is GuildFight)
                {
                    var guildFight = group as GuildFight;
                    guildFight.Users.Add((SocketGuildUser)reaction.User);
                    await guildFight.UpdateAtDB();
                    await guildFight.RewriteMessage();
                }
            }

            /// <summary>
            /// Обработка выхода из группы
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="message"></param>
            /// <param name="reaction"></param>
            public async static Task LeaveGroup(SocketReaction reaction)
            {
                var group = BotCore.GetDiscordGuildGroups(((SocketGuildUser)reaction.User).Guild.Id).Where(x => x.TargetMessage.Id == reaction.MessageId).FirstOrDefault();
                if (group is Party)
                {
                    var party = group as Party;
                    if (party.Users.Contains((SocketGuildUser)reaction.User))
                    {
                        party.Users.Remove((SocketGuildUser)reaction.User);
                        await party.UpdateAtDB();
                        await party.RewriteMessage();
                        return;
                    }
                }
                else if (group is Raid)
                {
                    var raid = group as Raid;
                    if (raid.Users.Contains((SocketGuildUser)reaction.User))
                    {
                        raid.Users.Remove((SocketGuildUser)reaction.User);
                        await raid.UpdateAtDB();
                        await raid.RewriteMessage();
                        return;
                    }
                }
                else if (group is GuildFight)
                {
                    var guildFight = group as GuildFight;
                    if (guildFight.Users.Contains((SocketGuildUser)reaction.User))
                    {
                        guildFight.Users.Remove((SocketGuildUser)reaction.User);
                        await guildFight.UpdateAtDB();
                        await guildFight.RewriteMessage();
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
            public async static Task TerminateGroup(SocketReaction reaction)
            {
                var group = BotCore.GetDiscordGuildGroups(((SocketGuildUser)reaction.User).Guild.Id).Where(x => x.TargetMessage.Id == reaction.MessageId).FirstOrDefault();
                if (group is Party)
                {
                    var party = group as Party;
                    if (party != null && (party.Author.Id == reaction.UserId || BotCore.GetDiscordGuildSettings(party.Channel.Guild.Id).adminsID.Contains(reaction.UserId)))
                    {
                        await party.RewriteMessageOnCancel();
                        await party.RemoveFromDB();
                        BotCore.GetDiscordGuildGroups(((SocketGuildUser)reaction.User).Guild.Id).Remove(party);
                    }
                }
                else if (group is Raid)
                {
                    var raid = group as Raid;
                    if (raid != null && (raid.Author.Id == reaction.UserId || BotCore.GetDiscordGuildSettings(raid.Channel.Guild.Id).adminsID.Contains(reaction.UserId)))
                    {
                        await raid.RewriteMessageOnCancel();
                        await raid.RemoveFromDB();
                        BotCore.GetDiscordGuildGroups(((SocketGuildUser)reaction.User).Guild.Id).Remove(raid);
                    }
                }
                else if (group is GuildFight)
                {
                    var guildFight = group as GuildFight;
                    if (guildFight != null && (guildFight.Author.Id == reaction.UserId || BotCore.GetDiscordGuildSettings(guildFight.Channel.Guild.Id).adminsID.Contains(reaction.UserId)))
                    {
                        await guildFight.RewriteMessageOnCancel();
                        await guildFight.RemoveFromDB();
                        BotCore.GetDiscordGuildGroups(((SocketGuildUser)reaction.User).Guild.Id).Remove(guildFight);
                    }
                }
            }

            /// <summary>
            /// Объявление сбора группы
            /// </summary>
            /// <param name="groupObject"></param>
            /// <param name="reaction"></param>
            /// <returns></returns>
            public async static Task GroupCallout(SocketReaction reaction)
            {
                var group = BotCore.GetDiscordGuildGroups(((SocketGuildUser)reaction.User).Guild.Id).Where(x => x.TargetMessage.Id == reaction.MessageId).FirstOrDefault();
                if (reaction.UserId == group.Author.Id || BotCore.GetDiscordGuildSettings(group.Channel.Guild.Id).adminsID.Contains(reaction.UserId))
                {
                    if (!(group is GuildFight))
                    {
                        StringBuilder calloutText = new StringBuilder();
                        calloutText.AppendLine($"{reaction.User.Value?.Mention} объявляет сбор группы");
                        foreach (var user in group.Users)
                        {
                            calloutText.AppendLine($"{user.Mention}");
                        }
                        await group.TargetMessage.Channel.SendMessageAsync(calloutText.ToString());
                    }
                    else
                    {
                        StringBuilder calloutText = new StringBuilder();
                        calloutText.AppendLine($"__**Сбор на битвы БШ!**__\nСостав:");
                        int memberCount = 0;
                        // Пробег по первому списку юзеров
                        foreach (var user in group.Users)
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
                        foreach (var user in ((GuildFight)group).noGearUsers)
                        {
                            if (memberCount < 6)
                            {
                                calloutText.AppendLine($"> {user.Mention}");
                                memberCount++;
                            }
                            if (memberCount == 6)
                                goto A;
                        }
                        foreach (var user in ((GuildFight)group).unwillingUsers)
                        {
                            if (memberCount < 6)
                            {
                                calloutText.AppendLine($"> {user.Mention}");
                                memberCount++;
                            }
                            if (memberCount == 6)
                                goto A;
                        }
                        foreach (var user in ((GuildFight)group).unsureUsers)
                        {
                            if (memberCount < 6)
                            {
                                calloutText.AppendLine($"> {user.Mention}");
                                memberCount++;
                            }
                            if (memberCount == 6)
                                goto A;
                        }

                        A: await group.TargetMessage.Channel.SendMessageAsync(calloutText.ToString());
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
            public async static Task JoinGuildFight(SocketReaction reaction)
            {
                var group = BotCore.GetDiscordGuildGroups(((SocketGuildUser)reaction.User).Guild.Id).Where(x => x.TargetMessage.Id == reaction.MessageId).FirstOrDefault();
                if (group is GuildFight)
                {
                    var guildFight = group as GuildFight;
                    var user = (SocketGuildUser)reaction.User;
                    if (guildFight.GuildFightType == GuildFightType.PR)
                    {
                        switch (reaction.Emote.Name)
                        {
                            case "🐾":
                                if (!guildFight.Users.Contains(user)
                                    && !guildFight.unwillingUsers.Contains(user)
                                    && !guildFight.unsureUsers.Contains(user))
                                {
                                    guildFight.noGearUsers.Add(user);
                                }
                                break;
                            case "🐷":
                                if (!guildFight.Users.Contains(user) &&
                                    !guildFight.noGearUsers.Contains(user) &&
                                    !guildFight.unsureUsers.Contains(user))
                                {
                                    guildFight.unwillingUsers.Add(user);
                                }
                                break;
                            case "❓":
                                if (!guildFight.Users.Contains(user) &&
                                    !guildFight.unwillingUsers.Contains(user) &&
                                    !guildFight.noGearUsers.Contains(user))
                                {
                                    guildFight.unsureUsers.Add(user);
                                }
                                break;
                        }
                        await group.UpdateAtDB();
                        await group.RewriteMessage();
                    }
                    else if (guildFight.GuildFightType == GuildFightType.EV)
                    {
                        switch (reaction.Emote.Name)
                        {
                            case "❎":
                                if (!guildFight.Users.Contains(user) &&
                                    !guildFight.unwillingUsers.Contains(user) &&
                                    !guildFight.unsureUsers.Contains(user))
                                {
                                    guildFight.noGearUsers.Add(user);
                                }
                                break;
                            case "☑️":
                                if (!guildFight.Users.Contains(user) &&
                                    !guildFight.noGearUsers.Contains(user) &&
                                    !guildFight.unsureUsers.Contains(user))
                                {
                                    guildFight.unwillingUsers.Add(user);
                                }
                                break;
                            case "🇽":
                                if (!guildFight.Users.Contains(user) &&
                                    !guildFight.unwillingUsers.Contains(user) &&
                                    !guildFight.noGearUsers.Contains(user))
                                {
                                    guildFight.unsureUsers.Add(user);
                                }
                                break;
                        }
                        await group.UpdateAtDB();
                        await group.RewriteMessage();
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
            public async static Task LeaveGuildFight(SocketReaction reaction)
            {
                var group = BotCore.GetDiscordGuildGroups(((SocketGuildUser)reaction.User).Guild.Id).Where(x => x.TargetMessage.Id == reaction.MessageId).FirstOrDefault();
                if (group is GuildFight)
                {
                    var guildFight = group as GuildFight;
                    var user = (SocketGuildUser)reaction.User;

                    if (guildFight.GuildFightType == GuildFightType.PR)
                    {
                        switch (reaction.Emote.Name)
                        {
                            case "🐾":
                                guildFight.noGearUsers.Remove(user);
                                break;
                            case "🐷":
                                guildFight.unwillingUsers.Remove(user);
                                break;
                            case "❓":
                                guildFight.unsureUsers.Remove(user);
                                break;
                        }
                        await guildFight.UpdateAtDB();
                        await guildFight.RewriteMessage();
                    }
                    else if (guildFight.GuildFightType == GuildFightType.EV)
                    {
                        switch (reaction.Emote.Name)
                        {
                            case "❎":
                                guildFight.noGearUsers.Remove(user);
                                break;
                            case "☑️":
                                guildFight.unwillingUsers.Remove(user);
                                break;
                            case "🇽":
                                guildFight.unsureUsers.Remove(user);
                                break;
                        }
                        await guildFight.UpdateAtDB();
                        await guildFight.RewriteMessage();

                    }
                }
            }
        }
    }
}
