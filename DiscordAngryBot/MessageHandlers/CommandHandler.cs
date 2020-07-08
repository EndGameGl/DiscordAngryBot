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

namespace DiscordAngryBot.MessageHandlers
{
    /// <summary>
    /// Класс для обработки команд
    /// </summary>
    public static class CommandHandler
    {
        /// <summary>
        /// Класс для обработки администраторских команд
        /// </summary>
        public static class SystemCommands
        {
            /// <summary>
            /// Добавление нового админа в бота
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public async static Task AddAdmin(SocketMessage message)
            {
                await Task.Run(async () => 
                {
                    ulong guildID = ((SocketGuildChannel)message.Channel).Guild.Id;
                    var settings = BotCore.GetDiscordGuildSettings(guildID);
                    var adminID = message.MentionedUsers.FirstOrDefault()?.Id;
                    if (adminID != null)
                    {
                        if (!settings.adminsID.Contains(adminID.Value))
                        {
                            settings.adminsID.Add(adminID.Value);
                            await message.Channel.SendMessageAsync($"Добавлен новый администратор бота на сервере: '{BotCore.GetGuildDataCache(guildID).Guild.GetUser(adminID.Value)}'");
                            await SQLiteDataManager.PushToDB("locals/GuildSettings.sqlite", $"UPDATE Settings SET SettingsJSON = '{JsonConvert.SerializeObject(settings)}' WHERE GuildID = '{guildID}'");
                        }
                        else 
                        {
                            await message.Channel.SendMessageAsync("Данный пользователь уже является администратором");
                        }
                    }
                });
            }
            /// <summary>
            /// Удаление админа из бота
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public async static Task RemoveAdmin(SocketMessage message)
            {
                await Task.Run(async () =>
                {
                    ulong guildID = ((SocketGuildChannel)message.Channel).Guild.Id;
                    var settings = BotCore.GetDiscordGuildSettings(guildID);
                    var adminID = message.MentionedUsers.FirstOrDefault()?.Id;
                    if (adminID != null)
                    {
                        if (settings.adminsID.Contains(adminID.Value))
                        {
                            settings.adminsID.Remove(adminID.Value);
                            await message.Channel.SendMessageAsync($"Забраны администраторские права у: '{BotCore.GetGuildDataCache(guildID).Guild.GetUser(adminID.Value)}'");
                            await SQLiteDataManager.PushToDB("locals/GuildSettings.sqlite", $"UPDATE Settings SET SettingsJSON = '{JsonConvert.SerializeObject(settings)}' WHERE GuildID = '{guildID}'");
                        }
                        else
                        {
                            await message.Channel.SendMessageAsync("Данный пользователь не является администратором");
                        }
                    }
                });
            }
            /// <summary>
            /// Установка префикса для команд
            /// </summary>
            /// <param name="message"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public async static Task SetPrefix(SocketMessage message, string[] args)
            {
                await Task.Run(
                    async () => 
                    {
                        if (args.Length == 1 && args[0]?.Length == 1) 
                        {
                            ulong guildID = ((SocketGuildChannel)message.Channel).Guild.Id;
                            var settings = BotCore.GetDiscordGuildSettings(guildID);
                            settings.CommandPrefix = args[0][0];
                            await message.Channel.SendMessageAsync($"Префикс команд сменен на '{settings.CommandPrefix}'");
                            await SQLiteDataManager.PushToDB("locals/GuildSettings.sqlite", $"UPDATE Settings SET SettingsJSON = '{JsonConvert.SerializeObject(settings)}' WHERE GuildID = '{guildID}'");
                        }
                    });
            }
            /// <summary>
            /// Установка новостного канала
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public async static Task SetNews(SocketMessage message)
            {
                await Task.Run(async () =>
                {
                    ulong guildID = ((SocketGuildChannel)message.Channel).Guild.Id;
                    var settings = BotCore.GetDiscordGuildSettings(guildID);
                    settings.NewsChannelID = message.Channel.Id;
                    await message.Channel.SendMessageAsync($"Этот канал установлен как новостной.");
                    await SQLiteDataManager.PushToDB("locals/GuildSettings.sqlite", $"UPDATE Settings SET SettingsJSON = '{JsonConvert.SerializeObject(settings)}' WHERE GuildID = '{guildID}'");
                });
            }
            /// <summary>
            /// Установка роли для бана
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public async static Task SetBanRole(SocketMessage message)
            {
                await Task.Run(async () =>
                {
                    ulong guildID = ((SocketGuildChannel)message.Channel).Guild.Id;
                    var settings = BotCore.GetDiscordGuildSettings(guildID);
                    settings.BanRoleID = message.MentionedRoles.FirstOrDefault()?.Id;
                    await message.Channel.SendMessageAsync($"Роль для бана на этом сервере: '{((SocketGuildChannel)message.Channel).Guild.GetRole(settings.BanRoleID.Value).Name}'");
                    await SQLiteDataManager.PushToDB("locals/GuildSettings.sqlite", $"UPDATE Settings SET SettingsJSON = '{JsonConvert.SerializeObject(settings)}' WHERE GuildID = '{guildID}'");
                });
            }
            /// <summary>
            /// Бан юзера
            /// </summary>
            /// <param name="message"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public async static Task BanUser(SocketMessage message, string[] args)
            {
                if (message.Channel is SocketGuildChannel)
                {
                    var channel = (SocketTextChannel)message.Channel;
                    var user = channel.GetUser(message.MentionedUsers.FirstOrDefault().Id);
                    var role = channel.Guild.GetRole(BotCore.GetDiscordGuildSettings(channel.Guild.Id).BanRoleID.Value);
                    if (args?.Length == 2)
                    {
                        if (args[1] != null && args[1]?.Length > 0)
                        {
                            if (int.TryParse(args[1], out int time))
                            {
                                time = time * 60 * 1000;
                                await user.Ban(time, role, channel, false);
                            }
                        }
                    }
                    else
                    {
                        await user.Ban(null, role, channel, false);
                    }
                }
                else
                {
                    await message.Channel.SendMessageAsync("Данная команда предназначена для использования на сервере.");
                }
            }
            /// <summary>
            /// Очистка сообщений
            /// </summary>
            /// <param name="message"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public async static Task ClearMessages(SocketMessage message, string[] args)
            {
                if (message.Channel is SocketGuildChannel)
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
                                await ConsoleWriter.Write(ex.Message, ConsoleWriter.InfoType.Error);
                                await message.Channel.SendMessageAsync($"Не получилось удалить столько сообщений, возможно, вы попытались удалить сообщения старше двух недель.");
                                await message.DeleteAsync();
                            }
                        }
                    }
                }
                else
                {
                    await message.Channel.SendMessageAsync("Данная команда предназначена для использования на сервере.");
                }
            }
            /// <summary>
            /// Разбан юзера
            /// </summary>
            /// <param name="message"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public async static Task UnbanUser(SocketMessage message, string[] args)
            {
                if (message.Channel is SocketGuildChannel)
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
                                    await message.Channel.SendMessageAsync($"Пользователь {user.Username} был разбанен.");
                                }
                                else
                                {
                                    await message.Channel.SendMessageAsync($"Пользователь {user.Username} не был забанен.");
                                }
                            }
                            catch (Exception e)
                            {
                                await ConsoleWriter.Write($"{e.Message}", ConsoleWriter.InfoType.Error);
                            }
                        }
                    }
                    await message.DeleteAsync();
                }
                else
                {
                    await message.Channel.SendMessageAsync("Данная команда предназначена для использования на сервере.");
                }
            }
            /// <summary>
            /// Тестовая штука
            /// </summary>
            /// <param name="message"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public async static Task EmbedTesting(SocketMessage message, string[] args)
            {
                if (message.Channel is SocketGuildChannel)
                {                  
                    Party party = await GroupBuilder.BuildParty(message, args);
                    var embedbuilder = new EmbedBuilder();
                    var embed =
                        embedbuilder
                        .WithTitle($"{string.Join(" ", args)}")
                        .WithTimestamp(party.CreatedAt)
                        .WithColor(Color.Blue)
                        .WithAuthor($"Лидер: {party.Author.Username}")
                        .AddField("Поле 1", "Тестовое значение 1")
                        .AddField("Поле 2", "Тестовое значение 2")
                        .AddField("Поле 3", "Тестовое значение 3")
                        .AddField("Поле 4", "Тестовое значение 4")
                        .AddField("Поле 5", "Тестовое значение 5")
                        .AddField("Поле 6", "Тестовое значение 6")
                        .Build();
                    party.TargetMessage = await party.Channel.SendMessageAsync(null, false, embed);
                    var messageEmbed = party.TargetMessage.Embeds.FirstOrDefault();
                }
                
            }
            /// <summary>
            /// Включение мат-фильтра на сервере
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public async static Task EnableSwearFilter(SocketMessage message)
            {
                if (message.Channel is SocketGuildChannel)
                {
                    try
                    {
                        var channel = (SocketGuildChannel)message.Channel;
                        BotCore.GetDiscordGuildSettings(channel.Guild.Id).IsSwearFilterEnabled = true;
                        await message.Channel.SendMessageAsync($"Матфильтр на этом сервере включен");
                    }
                    catch (Exception e)
                    {
                        await ConsoleWriter.Write($"{e.Message}", ConsoleWriter.InfoType.Error);
                    }
                }
                else
                {
                    await message.Channel.SendMessageAsync("Данная команда предназначена для использования на сервере.");
                }
            }
            /// <summary>
            /// Выключение мат-фильтра на сервере
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public async static Task DisableSwearFilter(SocketMessage message)
            {
                if (message.Channel is SocketGuildChannel)
                {
                    try
                    {
                        var channel = (SocketGuildChannel)message.Channel;
                        BotCore.GetDiscordGuildSettings(channel.Guild.Id).IsSwearFilterEnabled = false;
                        await message.Channel.SendMessageAsync($"Матфильтр на этом сервере выключен");
                    }
                    catch (Exception e)
                    {
                        await ConsoleWriter.Write($"{e.Message}", ConsoleWriter.InfoType.Error);
                    }
                }
                else
                {
                    await message.Channel.SendMessageAsync("Данная команда предназначена для использования на сервере.");
                }
            }

        }

        /// <summary>
        /// Класс для обработки пользовательских команд
        /// </summary>
        public static class UserCommands
        {
            /// <summary>
            /// Операция создания группы
            /// </summary>
            /// <param name="groups">Список всех групп</param>
            /// <param name="message">Сообщение, инициировавшее создание</param>
            /// <param name="destination">Описание группы</param>
            /// <returns></returns>
            public static async Task CreateParty(SocketMessage message, string[] destination)
            {
                if (message.Channel is SocketGuildChannel)
                {
                    Party party = await GroupBuilder.BuildParty(message, destination);
                    BotCore.GetDiscordGuildGroups(party.Channel.Guild.Id).Add(party);
                    await party.SendMessage();
                    await party.SaveToDB();
                }
                else
                {
                    await message.Channel.SendMessageAsync("Данная команда предназначена для использования на сервере.");
                }
            }
            /// <summary>
            /// Операция создания рейда
            /// </summary>
            /// <param name="groups">Список всех групп</param>
            /// <param name="message">Сообщение, инициировавшее создание</param>
            /// <param name="destination">Описание рейда</param>
            /// <returns></returns>
            public static async Task CreateRaid(SocketMessage message, string[] destination)
            {
                if (message.Channel is SocketGuildChannel)
                {
                    Raid raid = await GroupBuilder.BuildRaid(message, destination);
                    BotCore.GetDiscordGuildGroups(raid.Channel.Guild.Id).Add(raid);
                    await raid.SendMessage();
                    await raid.SaveToDB();
                }
                else
                {
                    await message.Channel.SendMessageAsync("Данная команда предназначена для использования на сервере.");
                }
            }
            /// <summary>
            /// Операция создания битвы БШ
            /// </summary>
            /// <param name="groups"></param>
            /// <param name="message"></param>
            /// <param name="destination"></param>
            /// <returns></returns>
            public static async Task CreateGuildFight(SocketMessage message, string[] destination, GuildFightType type)
            {
                if (message.Channel is SocketGuildChannel)
                {
                    GuildFight guildFight = await GroupBuilder.BuildGuildFight(message, destination, type);
                    BotCore.GetDiscordGuildGroups(guildFight.Channel.Guild.Id).Add(guildFight);
                    await guildFight.SendMessage();
                    await guildFight.SaveToDB();
                    await guildFight.RewriteMessage();
                }
                else
                {
                    await message.Channel.SendMessageAsync("Данная команда предназначена для использования на сервере.");
                }
            }
            /// <summary>
            /// Операция вывода списка групп
            /// </summary>
            /// <param name="message">Сообщение, инициировавшее вызов списка</param>
            /// <param name="groups">Список всех групп</param>
            /// <returns></returns>
            public static async Task ListGroups(SocketMessage message)
            {
                StringBuilder text = new StringBuilder();
                var groups = BotCore.GetDiscordGuildGroups(((SocketGuildChannel)message.Channel).Guild.Id);
                if (groups.Count() == 0)
                {
                    text.AppendLine("В данный момент нет никаких групп или рейдов.");
                }
                else
                {
                    if (groups.Count() > 0)
                    {
                        text.AppendLine("Список собираемых в данный момент составов:");
                        if (groups.Count(x => x is Party) > 0)
                        {
                            text.AppendLine("Группы:");
                            groups.ForEach(x =>
                                {
                                    if (x is Party)
                                    {
                                        text.AppendLine($"    {x.Author.Username}: {x.Destination} ({x.Users.Count} участников)");
                                    }
                                });
                        }
                        if (groups.Count(x => x is Raid) > 0)
                        {
                            text.AppendLine("Группы:");
                            groups.ForEach(x =>
                                {
                                    if (x is Raid)
                                    {
                                        text.AppendLine($"    {x.Author.Username}: {x.Destination} ({x.Users.Count} участников)");
                                    }
                                });
                        }
                        if (groups.Count(x => x is GuildFight) > 0)
                        {
                            text.AppendLine("Битвы БШ:");
                            groups.ForEach(x =>
                                {
                                    if (x is GuildFight)
                                    {
                                        text.AppendLine($"    {x.Author.Username}: {x.Destination} ({x.Users.Count + ((GuildFight)x).unwillingUsers.Count + ((GuildFight)x).unsureUsers.Count + ((GuildFight)x).noGearUsers.Count} участников)");
                                    }
                                });
                        }
                    }                    
                }
                await message.Author.SendMessageAsync(text.ToString());
                await message.DeleteAsync();
            }
            /// <summary>
            /// Вывод для пользователя всех доступных комманд бота
            /// </summary>
            /// <param name="user"></param>
            /// <returns></returns>
            public static async Task HelpUser(SocketMessage message, string[] args)
            {
                var guildID = ((SocketGuildUser)message.Author).Guild.Id;
                StringBuilder stringBuilder = new StringBuilder();
                if (args.Length == 0)
                {                   
                    stringBuilder.AppendLine("Информация о командах, используемых хомяком.");
                    if (BotCore.GetDiscordGuildSettings(guildID).adminsID.Contains(message.Author.Id))
                    {
                        stringBuilder.AppendLine("> Системные команды:");
                        foreach (var com in BotCore.SystemCommands())
                        {
                            stringBuilder.AppendLine($" - {com.ToLowerInvariant()}");
                        }
                    }
                    stringBuilder.AppendLine("> Пользовательские команды:");
                    foreach (var com in BotCore.UserCommands())
                    {
                        stringBuilder.AppendLine($" - {com.ToLowerInvariant()}");
                    }
                    stringBuilder.AppendLine("> Другие команды:");
                    foreach (var com in BotCore.OtherCommands())
                    {
                        stringBuilder.AppendLine($" - {com.ToLowerInvariant()}");
                    }
                }
                else
                {
                    stringBuilder.AppendLine($"Информация о команде {args[0]}");
                    switch (args[0].ToUpperInvariant())
                    {
                        case "BAN":
                            stringBuilder.AppendLine($"Применение:\n> {BotCore.GetGuildCommandPrefix(guildID)}ban [цель бана] [время в минутах]");
                            break;
                        case "CLEAR":
                            stringBuilder.AppendLine($"Применение:\n> {BotCore.GetGuildCommandPrefix(guildID)}clear [количество сообщений]");
                            break;
                        case "PARTY":
                            stringBuilder.AppendLine($"Применение:\n> {BotCore.GetGuildCommandPrefix(guildID)}party [цель сбора группы]");
                            break;
                        case "RAID":
                            stringBuilder.AppendLine($"Применение:\n> {BotCore.GetGuildCommandPrefix(guildID)}raid [цель сбора рейда]");
                            break;
                        case "LIST":
                            stringBuilder.AppendLine($"Применение:\n> {BotCore.GetGuildCommandPrefix(guildID)}list");
                            break;
                        case "GVG":
                            stringBuilder.AppendLine($"Применение:\n> {BotCore.GetGuildCommandPrefix(guildID)}gvg [время сбора битв бш]");
                            break;
                        case "HELP":
                            stringBuilder.AppendLine($"Применение:\n> {BotCore.GetGuildCommandPrefix(guildID)}help (команда)");
                            break;
                        case "КУСЬ":
                            stringBuilder.AppendLine($"Применение:\n> {BotCore.GetGuildCommandPrefix(guildID)}кусь (цель)");
                            break;
                        case "БАН":
                            stringBuilder.AppendLine($"Применение:\n> {BotCore.GetGuildCommandPrefix(guildID)}бан");
                            break;
                        default:
                            stringBuilder.AppendLine($"Такой команды нет.");
                            break;
                    }

                }
                await message.Author.SendMessageAsync(stringBuilder.ToString());
                await message.DeleteAsync();
            }
            /// <summary>
            /// Команда для бана пользователя самим себя
            /// </summary>
            /// <param name="message"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public static async Task SelfBan(SocketMessage message, string[] args)
            {
                if (message.Channel is SocketGuildChannel)
                {
                    var targetServer = ((SocketGuildChannel)message.Channel).Guild;
                    var targetUser = targetServer.Users.Where(x => x.Id == message.Author.Id).Single();
                    if (BotCore.GetDiscordGuildBans(targetServer.Id).Where(x => x.BanTarget.Id == targetUser.Id).Count() == 0)
                    {
                        if (args[0] != null && args[0] != "")
                            await targetUser?.Ban(Convert.ToInt32(args[0]) * 60 * 1000, targetServer.GetRole(BotCore.GetDiscordGuildSettings(targetServer.Id).BanRoleID.Value), (SocketTextChannel)message.Channel, false, true);
                        else
                            await targetUser?.Ban(21600000, targetServer.GetRole(BotCore.GetDiscordGuildSettings(targetServer.Id).BanRoleID.Value), (SocketTextChannel)message.Channel, false, true);
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync("Нельзя выдать себе бан будучи в бане.");
                    }
                    await message.DeleteAsync();
                }
                else
                {
                    await message.Channel.SendMessageAsync("Данная команда предназначена для использования на сервере.");
                }
            }
        }

        /// <summary>
        /// Класс для обработки музыкальных комманд
        /// </summary>
        public static class MusicCommands
        {
            [Command(RunMode = RunMode.Async)]
            public static async Task JoinChannel(SocketMessage message)
            {
                if (message.Channel is SocketGuildChannel)
                {
                    var voiceChannel = ((SocketGuildChannel)message.Channel).Guild.VoiceChannels.Where(x => x.Users.Contains(message.Author)).FirstOrDefault();
                    await voiceChannel.ConnectAsync();
                }
            }
        }

        /// <summary>
        /// Класс для обработки команд для развлечения
        /// </summary>
        public static class OtherCommands
        {
            /// <summary>
            /// Кусь
            /// </summary>
            /// <param name="message"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public static async Task Bite(SocketMessage message, string[] args)
            {
                await message.DeleteAsync();

                int shoudBite = -1;
                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                shoudBite = rnd.Next(0, 2);

                if (args.Length != 0)
                {
                    if (shoudBite == 1)
                    {
                        List<string> choices = new List<string>() { "ляху", "кокоро", "жеппу", "ножку", "щечку", "хвост" };
                        await message.Channel.SendMessageAsync($"Кусаю {args[0]} за {choices.OrderBy(x => Guid.NewGuid()).FirstOrDefault()}");
                    }
                    else if (shoudBite == 0)
                    {
                        await message.Channel.SendMessageAsync($"Не собираюсь я такое кусать D:");
                    }
                }
                else
                {
                    if (shoudBite == 1)
                    {
                        List<string> moreChoices = new List<string>() { "Кого кусать то? ( ._.)", "КУСЬ" };
                        await message.Channel.SendMessageAsync($"{moreChoices.OrderBy(x => Guid.NewGuid()).FirstOrDefault()}");
                    }
                    else
                        await message.Channel.SendMessageAsync($"Сейчас мне лень кусаться");
                }

            }

            /// <summary>
            /// Бан
            /// </summary>
            /// <param name="message"></param>
            /// <returns></returns>
            public static async Task Ban(SocketMessage message)
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync($"Не, ну это бан");
            }
        }
    }
}
