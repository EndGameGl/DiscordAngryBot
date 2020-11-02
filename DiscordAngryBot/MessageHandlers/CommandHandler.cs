using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.Bans;
using DiscordAngryBot.CustomObjects.Groups;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordAngryBot.CustomObjects.SQLIteHandler;
using Newtonsoft.Json;
using DiscordAngryBot.CustomObjects.DiscordCommands;

namespace DiscordAngryBot.MessageHandlers
{
    /// <summary>
    /// Class for handling commands
    /// </summary>
    public static class CommandHandler
    {
        /// <summary>
        /// Class for handling system commands
        /// </summary>
        public static class SystemCommands
        {
            /// <summary>
            /// Add new admin to bot
            /// </summary>
            /// <param name="message">Command message</param>
            /// <returns></returns>
            [CustomCommand("ADMIN", CommandCategory.System, CommandType.StringCommand, "Добавление нового админа в бота", CommandScope.Admin, CommandExecutionScope.Server)]
            public async static Task AddAdmin(SocketMessage message)
            {
                await Task.Run(async () =>
                {
                    ulong guildID = ((SocketGuildChannel)message.Channel).Guild.Id;
                    if (BotCore.TryGetDiscordGuildSettings(guildID, out var settings))
                    {
                        var adminID = message.MentionedUsers.FirstOrDefault()?.Id;
                        if (adminID != null)
                        {
                            if (!settings.IsAdmin(adminID.Value))
                            {
                                settings.AdminsID.Add(adminID.Value);
                                string callbackMessage = string.Format(CommandResources.AdminAddedLine, message.MentionedUsers.FirstOrDefault().Mention);
                                await message.Channel.SendMessageAsync(callbackMessage);
                                string updateQuery = string.Format(SQLiteResources.UpdateSettingsByGuildID, JsonConvert.SerializeObject(settings), guildID);
                                await SQLiteExtensions.PushToDB(SQLiteResources.GuildSettingsDBPath, updateQuery);
                            }
                            else
                            {
                                await message.Channel.SendMessageAsync(CommandResources.AlreadyAdminLine);
                            }
                        }
                    }
                });
            }

            /// <summary>
            /// Remove admin from bot
            /// </summary>
            /// <param name="message">Command message</param>
            /// <returns></returns>
            [CustomCommand("DEADMIN", CommandCategory.System, CommandType.StringCommand, "Удаление админа из бота", CommandScope.Admin, CommandExecutionScope.Server)]
            public async static Task RemoveAdmin(SocketMessage message)
            {
                await Task.Run(async () =>
                {
                    ulong guildID = ((SocketGuildChannel)message.Channel).Guild.Id;
                    if (BotCore.TryGetDiscordGuildSettings(guildID, out var settings))
                    {
                        var adminID = message.MentionedUsers.FirstOrDefault()?.Id;
                        if (adminID != null)
                        {
                            if (settings.IsAdmin(adminID.Value))
                            {
                                settings.AdminsID.Remove(adminID.Value);
                                string callbackMessage = string.Format(CommandResources.AdminRemovedLine, message.MentionedUsers.FirstOrDefault().Mention);
                                await message.Channel.SendMessageAsync(callbackMessage);
                                string updateQuery = string.Format(SQLiteResources.UpdateSettingsByGuildID, JsonConvert.SerializeObject(settings), guildID);
                                await SQLiteExtensions.PushToDB(SQLiteResources.GuildSettingsDBPath, updateQuery);
                            }
                            else
                            {
                                await message.Channel.SendMessageAsync(CommandResources.NotAnAdminLine);
                            }
                        }
                    }
                });
            }

            /// <summary>
            /// Change command prefix
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="args">Command arguments</param>
            /// <returns></returns>
            [CustomCommand("SETPREFIX", CommandCategory.System, CommandType.StringCommand, "Установка префикса для команд", CommandScope.Admin, CommandExecutionScope.Server)]
            public async static Task SetPrefix(SocketMessage message, string[] args)
            {
                await Task.Run(
                    async () =>
                    {
                        if (args.Length == 1 && args[0]?.Length == 1)
                        {
                            ulong guildID = ((SocketGuildChannel)message.Channel).Guild.Id;
                            if (BotCore.TryGetDiscordGuildSettings(guildID, out var settings))
                            {
                                settings.ChangeCommandPrefix(args[0][0]);
                                string callbackMessage = string.Format(CommandResources.CommandPrefixChangedLine, settings.CommandPrefix);
                                await message.Channel.SendMessageAsync(callbackMessage);
                                string updateQuery = string.Format(SQLiteResources.UpdateSettingsByGuildID, JsonConvert.SerializeObject(settings), guildID);
                                await SQLiteExtensions.PushToDB(SQLiteResources.GuildSettingsDBPath, updateQuery);
                            }
                        }
                    });
            }

            /// <summary>
            /// Set news channel
            /// </summary>
            /// <param name="message">Command message</param>
            /// <returns></returns>
            [CustomCommand("NEWS", CommandCategory.System, CommandType.StringCommand, "Установка новостного канала", CommandScope.Admin, CommandExecutionScope.Server)]
            public async static Task SetNews(SocketMessage message)
            {
                await Task.Run(async () =>
                {
                    ulong guildID = ((SocketGuildChannel)message.Channel).Guild.Id;
                    if (BotCore.TryGetDiscordGuildSettings(guildID, out var settings))
                    {
                        settings.ChangeNewsChannelID(message.Channel.Id);
                        await message.Channel.SendMessageAsync(CommandResources.NewsChannelSetLine);
                        string updateQuery = string.Format(SQLiteResources.UpdateSettingsByGuildID, JsonConvert.SerializeObject(settings), guildID);
                        await SQLiteExtensions.PushToDB(SQLiteResources.GuildSettingsDBPath, updateQuery);
                    }
                });
            }

            /// <summary>
            /// Set ban role
            /// </summary>
            /// <param name="message">Command message</param>
            /// <returns></returns>
            [CustomCommand("BANROLE", CommandCategory.System, CommandType.StringCommand, "Установка роли для бана", CommandScope.Admin, CommandExecutionScope.Server)]
            public async static Task SetBanRole(SocketMessage message)
            {
                await Task.Run(async () =>
                {
                    ulong guildID = ((SocketGuildChannel)message.Channel).Guild.Id;
                    if (BotCore.TryGetDiscordGuildSettings(guildID, out var settings))
                    {
                        var banRole = message.MentionedRoles.FirstOrDefault();
                        if (banRole != null)
                        {
                            settings.ChangeBanRoleID(banRole.Id);
                            var callbackMessage = string.Format(CommandResources.BanRoleSetLine, banRole.Name);
                            await message.Channel.SendMessageAsync(callbackMessage);
                            var updateQuery = string.Format(SQLiteResources.UpdateSettingsByGuildID, JsonConvert.SerializeObject(settings), guildID);
                            await SQLiteExtensions.PushToDB(SQLiteResources.GuildSettingsDBPath, updateQuery);
                        }
                    }
                });
            }

            /// <summary>
            /// Ban user
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="args">Command arguments</param>
            /// <returns></returns>
            [CustomCommand("BAN", CommandCategory.System, CommandType.StringCommand, "Бан юзера", CommandScope.Admin, CommandExecutionScope.Server)]
            public async static Task BanUser(SocketMessage message, string[] args)
            {
                var channel = (SocketTextChannel)message.Channel;
                if (BotCore.TryGetDiscordGuildSettings(channel.Guild.Id, out var settings))
                {
                    var user = channel.GetUser(message.MentionedUsers.FirstOrDefault().Id);
                    var role = channel.Guild.GetRole(settings.BanRoleID.Value);
                    if (args?.Length == 2)
                    {
                        if (args[1] != null && args[1]?.Length > 0)
                        {
                            if (int.TryParse(args[1], out int time))
                            {
                                time *= CommandSettings.Default.MinuteToMilliseconds;
                                await user.Ban(time, role, channel, false);
                            }
                        }
                    }
                    else
                    {
                        await user.Ban(null, role, channel, false);
                    }
                }
            }

            /// <summary>
            /// Clear messages
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="args">Command arguments</param>
            /// <returns></returns>
            [CustomCommand("CLEAR", CommandCategory.System, CommandType.StringCommand, "Очистка сообщений", CommandScope.Admin, CommandExecutionScope.Server)]
            public async static Task ClearMessages(SocketMessage message, string[] args)
            {
                var channel = (SocketGuildChannel)message.Channel;
                if (args?.Length == 1)
                {
                    if (int.TryParse(args[0], out int amount))
                    {
                        try
                        {
                            var messages = message.Channel.GetMessagesAsync(amount + 1, CacheMode.AllowDownload).Flatten();
                            await channel.Guild.GetTextChannel(message.Channel.Id).DeleteMessagesAsync(messages.ToEnumerable());
                        }
                        catch (Exception ex)
                        {
                            await Debug.Log(ex.Message, LogInfoType.Error);
                            await message.Channel.SendMessageAsync(CommandResources.InvalidMessageClearRangeLine);
                        }
                    }
                }
            }

            /// <summary>
            /// Unban user
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="args">Command arguments</param>
            /// <returns></returns>
            [CustomCommand("UNBAN", CommandCategory.System, CommandType.StringCommand, "Разбан юзера", CommandScope.Admin, CommandExecutionScope.Server)]
            public async static Task UnbanUser(SocketMessage message, string[] args)
            {
                if (args?.Length == 1)
                {
                    if (args[0].Length > 0)
                    {
                        try
                        {
                            var channel = (SocketGuildChannel)message.Channel;
                            var user = channel.Guild.GetUser(message.MentionedUsers.FirstOrDefault().Id);
                            var ban = BotCore.GetDiscordGuildBans(channel.Guild.Id).Where(x => x.BanTarget.Id == user.Id).FirstOrDefault();
                            if (ban != null)
                            {
                                await ban.Unban();
                                string callbackMessage = string.Format(CommandResources.UserIsUnbannedLine, user.Username);
                                await message.Channel.SendMessageAsync(callbackMessage);
                            }
                            else
                            {
                                string callbackMessage = string.Format(CommandResources.UserIsNotBannedLine, user.Username);
                                await message.Channel.SendMessageAsync(callbackMessage);
                            }
                        }
                        catch (Exception e)
                        {
                            await Debug.Log($"{e.Message}", LogInfoType.Error);
                        }
                    }
                }
            }

            /// <summary>
            /// Enable swear filter in guild
            /// </summary>
            /// <param name="message">Command message</param>
            /// <returns></returns>
            [CustomCommand("FILTERENABLE", CommandCategory.System, CommandType.StringCommand, "Включение мат-фильтра на сервере", CommandScope.Admin, CommandExecutionScope.Server)]
            public async static Task EnableSwearFilter(SocketMessage message)
            {
                try
                {
                    var channel = (SocketGuildChannel)message.Channel;
                    if (BotCore.TryGetDiscordGuildSettings(channel.Guild.Id, out var settings))
                    {
                        settings.EnableSwearFilter();
                        await message.Channel.SendMessageAsync(CommandResources.SwearFilterEnabledLine);
                    }
                }
                catch (Exception e)
                {
                    await Debug.Log($"{e.Message}", LogInfoType.Error);
                }
            }

            /// <summary>
            /// Disable swear filter in guild
            /// </summary>
            /// <param name="message">Command message</param>
            /// <returns></returns>
            [CustomCommand("FILTERDISABLE", CommandCategory.System, CommandType.StringCommand, "Выключение мат-фильтра на сервере", CommandScope.Admin, CommandExecutionScope.Server)]
            public async static Task DisableSwearFilter(SocketMessage message)
            {
                try
                {
                    var channel = (SocketGuildChannel)message.Channel;
                    if (BotCore.TryGetDiscordGuildSettings(channel.Guild.Id, out var settings))
                    {
                        settings.DisableSwearFilter();
                        await message.Channel.SendMessageAsync(CommandResources.SwearFilterDisabledLine);
                    }
                }
                catch (Exception e)
                {
                    await Debug.Log($"{e.Message}", LogInfoType.Error);
                }
            }

        }

        /// <summary>
        /// Class for handling user commands
        /// </summary>
        public static class UserCommands
        {
            /// <summary>
            /// Create group with a party template
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="destination">Group goal</param>
            /// <returns></returns>
            [CustomCommand("PARTY", CommandCategory.User, CommandType.StringCommand, "Операция создания группы", CommandScope.User, CommandExecutionScope.Server)]
            public static async Task CreatePartyTemplate(SocketMessage message, string[] destination)
            {
                await Debug.Log($"Starting party creation: [message is in {((message.Channel is SocketGuildChannel) ? "guild channel" : "not a guild channel")}] [destination: {((destination != null) ? string.Join("", destination) : "empty")}].");
                await Debug.Log("Building group with given parameters.");
                Group group = GroupBuilder.BuildPartyTemplate(message, destination);
                await Debug.Log($"Group {group.GUID} was built for {group.UserLists.Count} userlists at {group.CreatedAt}.");
                if (BotCore.TryGetDiscordGuildGroups(group.Channel.Guild.Id, out List<Group> groups))
                {
                    await Debug.Log($"Got guild groups list: {groups.Count} groups. Adding new group to the list.");
                    groups.Add(group);
                    await Debug.Log($"Group added: {groups.Count} groups in the list. Sending group message...");
                    await group.SendMessage();
                    await Debug.Log($"Message sent. Saving group to database.");
                    await group.SaveToDB();
                    await Debug.Log($"Group saved. Task finished.");
                }
            }

            /// <summary>
            /// Create group with raid template
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="destination">Group goal</param>
            /// <returns></returns>
            [CustomCommand("RAID", CommandCategory.User, CommandType.StringCommand, "Операция создания рейда", CommandScope.User, CommandExecutionScope.Server)]
            public static async Task CreateRaidTemplate(SocketMessage message, string[] destination)
            {
                Group group = GroupBuilder.BuildRaidTemplate(message, destination);
                if (BotCore.TryGetDiscordGuildGroups(group.Channel.Guild.Id, out List<Group> groups))
                {
                    groups.Add(group);
                    await group.SendMessage();
                    await group.SaveToDB();
                }
            }

            /// <summary>
            /// Legacy, pointless but can't remove
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="destination">Group goal</param>
            /// <returns></returns>
            [CustomCommand("GVGEV", CommandCategory.User, CommandType.StringCommand, "Операция создания битвы БШ", CommandScope.User, CommandExecutionScope.Server)]
            public static async Task CreateGuildFightEV(SocketMessage message, string[] destination)
            {
                GuildFightType type = GuildFightType.EV;
                GuildFight group = await GroupBuilder.BuildGuildFight(message, destination, type);
                if (BotCore.TryGetDiscordGuildGroups(group.Channel.Guild.Id, out List<Group> groups))
                {
                    groups.Add(group);
                    await group.SendMessage();
                    await group.SaveToDB();
                }
            }

            /// <summary>
            /// Legacy, but can't remove
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="destination">Group goal</param>
            /// <returns></returns>
            [CustomCommand("GVGPR", CommandCategory.User, CommandType.StringCommand, "Операция создания битвы БШ", CommandScope.User, CommandExecutionScope.Server)]
            public static async Task CreateGuildFightPR(SocketMessage message, string[] destination)
            {
                GuildFightType type = GuildFightType.PR;
                GuildFight group = await GroupBuilder.BuildGuildFight(message, destination, type);
                if (BotCore.TryGetDiscordGuildGroups(group.Channel.Guild.Id, out List<Group> groups))
                {
                    groups.Add(group);
                    await group.SendMessage();
                    await group.SaveToDB();
                }
            }

            /// <summary>
            /// List all groups present on server
            /// </summary>
            /// <param name="message">Command message</param>
            /// <returns></returns>
            [CustomCommand("LIST", CommandCategory.User, CommandType.StringCommand, "Операция вывода списка групп", CommandScope.User, CommandExecutionScope.Server)]
            public static async Task ListGroups(SocketMessage message)
            {
                StringBuilder text = new StringBuilder();
                if (BotCore.TryGetDiscordGuildGroups(((SocketGuildChannel)message.Channel).Guild.Id, out List<Group> groups))
                {
                    if (groups.Count() == 0)
                    {
                        text.AppendLine("В данный момент нет никаких групп или рейдов.");
                    }
                    else
                    {
                        text.AppendLine("Собираемые в данный момент группы:");
                        foreach (var group in groups)
                        {
                            text.AppendLine($">{group.Author.Mention}: {group.Destination} ({group.UserLists.Sum(x => x.Users.Count())} пользователей)");
                        }
                    }
                    await message.Author.SendMessageAsync(text.ToString());
                }
            }

            /// <summary>
            /// Show all available commands
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="args">Command arguments</param>
            /// <returns></returns>
            [CustomCommand("HELP", CommandCategory.User, CommandType.StringCommand, "Вывод для пользователя всех доступных команд бота", CommandScope.User, CommandExecutionScope.All)]
            public static async Task HelpUser(SocketMessage message, string[] args)
            {
                await Debug.Log("Entered HelpUser method.");
                StringBuilder stringBuilder = new StringBuilder();
                if (args == null || args.Length == 0)
                {
                    await Debug.Log("No args detected...fetching all commands.");
                    var commands = BotCore.GetAllCommands();
                    var stringCommands = commands.Where(x => x.CommandMetadata.Type == CommandType.StringCommand);
                    if (stringCommands.Count() > 0)
                    {
                        var globalCommands = stringCommands.Where(x => x.CommandMetadata.CommandExecutionScope == CommandExecutionScope.All);
                        if (globalCommands.Count() > 0)
                        {
                            stringBuilder.AppendLine($"Команды, применяемые в любом месте:");
                            WriteCommandInfo(stringBuilder, globalCommands);
                        }

                        var serverCommands = stringCommands.Where(x => x.CommandMetadata.CommandExecutionScope == CommandExecutionScope.Server);
                        if (serverCommands.Count() > 0)
                        {
                            stringBuilder.AppendLine($"Команды для применения на сервере:");
                            WriteCommandInfo(stringBuilder, serverCommands);
                        }

                        var dmCommands = stringCommands.Where(x => x.CommandMetadata.CommandExecutionScope == CommandExecutionScope.DM);
                        if (dmCommands.Count() > 0)
                        {
                            stringBuilder.AppendLine($"Команды для применения в личном канале:");
                            WriteCommandInfo(stringBuilder, dmCommands);
                        }
                    }
                }
                else
                {
                    stringBuilder.AppendLine($"Информация о команде {args[0]}:");
                    if (BotCore.TryGetCommand(args[0].ToUpperInvariant(), out DiscordCommand discordCommand))
                    {
                        stringBuilder.AppendLine($"{discordCommand.CommandMetadata.Description}");
                    }
                }
                await message.Author.SendMessageAsync(stringBuilder.ToString());
            }

            /// <summary>
            /// User self ban
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="args">Command arguments</param>
            /// <returns></returns>
            [CustomCommand("SELFBAN", CommandCategory.User, CommandType.StringCommand, "Команда для бана пользователя самим себя", CommandScope.User, CommandExecutionScope.Server)]
            public static async Task SelfBan(SocketMessage message, string[] args)
            {
                var targetServer = ((SocketGuildChannel)message.Channel).Guild;
                if (BotCore.TryGetDiscordGuildSettings(targetServer.Id, out var settings))
                {
                    var targetUser = targetServer.Users.Where(x => x.Id == message.Author.Id).Single();
                    if (BotCore.GetDiscordGuildBans(targetServer.Id).Where(x => x.BanTarget.Id == targetUser.Id).Count() == 0)
                    {
                        if (args[0] != null && args[0] != "")
                            await targetUser?.Ban(Convert.ToInt32(args[0]) * CommandSettings.Default.MinuteToMilliseconds, targetServer.GetRole(settings.BanRoleID.Value), (SocketTextChannel)message.Channel, false, true);
                        else
                            await targetUser?.Ban(21600000, targetServer.GetRole(settings.BanRoleID.Value), (SocketTextChannel)message.Channel, false, true);
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Нельзя выдать себе бан будучи в бане.");
                    }
                }
            }

            /// <summary>
            /// Rolls the dice!
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="args">Command arguments</param>
            /// <returns></returns>
            [CustomCommand("ROLL", CommandCategory.User, CommandType.StringCommand, "Команда для броска костей", CommandScope.User, CommandExecutionScope.All)]
            public static async Task Roll(SocketMessage message, string[] args)
            {
                if (Int32.TryParse(args[0], out int num))
                {
                    await message.Channel.SendMessageAsync($"Пользователь {message.Author.Mention} выкинул {BotCore.Random.Next(0, num)} из {num}");
                }
            }

            /// <summary>
            /// Starts the poll (not ready yet)
            /// </summary>
            /// <param name="message">Command message</param>
            /// <param name="args">Command arguments</param>
            /// <returns></returns>
            [CustomCommand("POLL", CommandCategory.User, CommandType.StringCommand, "Запустить голосование", CommandScope.Admin, CommandExecutionScope.Server)]
            public static async Task StartPoll(SocketMessage message, string[] args)
            {
                var group = await GroupBuilder.BuildPoll(message, args);
                if (BotCore.TryGetDiscordGuildGroups(group.Channel.Guild.Id, out List<Group> groups))
                {
                    groups.Add(group);
                    await group.SendMessage();
                    await group.SaveToDB();
                }
            }

            private static void WriteCommandInfo(StringBuilder builder, IEnumerable<DiscordCommand> discordCommands)
            {
                var userCommands = discordCommands.Where(x => x.CommandMetadata.Scope == CommandScope.User);
                var adminCommands = discordCommands.Where(x => x.CommandMetadata.Scope == CommandScope.Admin);
                if (adminCommands.Count() > 0)
                {
                    builder.AppendLine("    Администраторские команды:");
                    foreach (var command in adminCommands)
                    {
                        builder.AppendLine($"        {command.CommandMetadata.CommandName}: {command.CommandMetadata.Description}");
                    }
                }
                if (userCommands.Count() > 0)
                {
                    builder.AppendLine("    Пользовательские команды:");
                    foreach (var command in userCommands)
                    {
                        builder.AppendLine($"        {command.CommandMetadata.CommandName}: {command.CommandMetadata.Description}");
                    }
                }
            }
        }

        /// <summary>
        /// Class for handling music commands
        /// </summary>
        public static class MusicCommands
        {
            /// <summary>
            /// Not working
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            [Command(RunMode = RunMode.Async)]
            public static async Task JoinChannel(SocketMessage message)
            {
                if (message.Channel is SocketGuildChannel channel)
                {
                    var voiceChannel = channel.Guild.VoiceChannels.Where(x => x.Users.Contains(message.Author)).FirstOrDefault();
                    await voiceChannel.ConnectAsync();
                }
            }
        }

        /// <summary>
        /// Class for handling misc commands
        /// </summary>
        public static class OtherCommands
        {
            /// <summary>
            /// Bites someone...or no one!
            /// </summary>
            /// <param name="message">Command message</param>
            /// <returns></returns>
            [CustomCommand("КУСЬ", CommandCategory.Other, CommandType.StringCommand, "КУСЬ", CommandScope.User, CommandExecutionScope.All)]
            public static async Task Bite(SocketMessage message)
            {
                await Debug.Log("Выбираем, кого укусить...");
                bool shoudBite = false;
                shoudBite = Convert.ToBoolean(BotCore.Random.Next(0, 2));
                await Debug.Log($"{((shoudBite) ? "Кусать будем!" : "Кусать не будем...")}");
                
                if (message.MentionedUsers.Count > 0)
                {
                    await Debug.Log("Аргументы есть, зне");
                    if (shoudBite)
                    {
                        List<string> choices = new List<string>() { "ляху", "кокоро", "жеппу", "ножку", "щечку", "хвост" };
                        await message.Channel.SendMessageAsync($"Кусаю {message.MentionedUsers.FirstOrDefault()?.Mention} за {choices.OrderBy(x => Guid.NewGuid()).FirstOrDefault()}");
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync($"Не собираюсь я такое кусать D:");
                    }
                }
                else
                {
                    if (shoudBite)
                    {
                        List<string> moreChoices = new List<string>() { "Кого кусать то? ( ._.)", "КУСЬ" };
                        await message.Channel.SendMessageAsync($"{moreChoices.OrderBy(x => Guid.NewGuid()).FirstOrDefault()}");
                    }
                    else
                        await message.Channel.SendMessageAsync($"Сейчас мне лень кусаться");
                }

            }

            /// <summary>
            /// Fake ban...huh
            /// </summary>
            /// <param name="message">Command message</param>
            /// <returns></returns>
            [CustomCommand("БАН", CommandCategory.Other, CommandType.StringCommand, "Нет, ну это бан", CommandScope.User, CommandExecutionScope.All)]
            public static async Task Ban(SocketMessage message)
            {
                await message.Channel.SendMessageAsync($"Не, ну это бан");
            }
        }
    }
}
