using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.SQLIteHandler;
using DiscordAngryBot.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Bans
{
    /// <summary>
    /// Обработчик операций, связанных с банами пользователей
    /// </summary>
    public static class BanHandler
    {
        /// <summary>
        /// Бан пользователя
        /// </summary>
        /// <param name="user">Цель бана</param>
        /// <param name="time">Время бана</param>
        /// <param name="role">Роль, которая попадает под бан</param>
        /// <param name="channel">Канал, в котором забанен пользователь</param>
        /// <returns></returns>
        public static async Task Ban(this SocketGuildUser user, int? time, SocketRole role, SocketTextChannel channel, bool isAuto, bool isSelf = false)
        {
            DiscordBan ban = await BanBuilder.BuildDiscordBan(user, time, channel);
            string banMessageText = string.Empty;
            await ban.BanTarget.RemoveRoleAsync(role);
            if (ban.EndsAt == null)
            {
                banMessageText = string.Format(BanResources.InfiniteBanLine, ban.BanTarget.Mention);               
            }
            else
            {                
                ban.BanTimer = new Timer(BanTimerCallBack, ban, (int)time, Timeout.Infinite);
                if (isAuto == false)
                {
                    if (isSelf == false)
                    {
                        banMessageText = string.Format(BanResources.FiniteBanLine, ban.BanTarget.Mention, time / 60 / 1000);
                    }
                    else 
                    {
                        banMessageText = string.Format(BanResources.FiniteSelfBanLine, ban.BanTarget.Mention, time / 60 / 1000);
                    }
                }
                else 
                {
                    banMessageText = string.Format(BanResources.AutoBanLine, ban.BanTarget.Mention);
                }
            }
            await ban.Channel.SendMessageAsync(banMessageText);
            BotCore.GetDiscordGuildBans(channel.Guild.Id).Add(ban);
            await ban.SaveBanToDB();
        }

        /// <summary>
        /// Разбан пользователя
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public static async Task Unban(this DiscordBan ban)
        {
            if (BotCore.TryGetDiscordGuildSettings(ban.Channel.Guild.Id, out var settings))
            {
                var roleID = settings.BanRoleID;
                await ban.BanTarget.AddRoleAsync(ban.Channel.Guild.GetRole(roleID.Value));
                ban.BanTimer?.Dispose();
                string sqlBanDeleteQuery = string.Format(SQLiteResources.DeleteBanByGUID, ban.GUID);
                string dbPath = string.Format(SQLiteResources.BanDBPath, ban.Channel.Guild.Id);
                await SQLiteDataManager.PushToDB(dbPath, sqlBanDeleteQuery);
                BotCore.GetDiscordGuildBans(ban.Channel.Guild.Id).Remove(ban);
            }
        }

        /// <summary>
        /// Метод, вызываемый при окончании бана
        /// </summary>
        /// <param name="ban"></param>
        public static async void BanTimerCallBack(object ban)
        {
            DiscordBan banObj = (DiscordBan)ban;
            if (BotCore.TryGetDiscordGuildSettings(banObj.Channel.Guild.Id, out var settings))
            {
                var roleID = settings.BanRoleID;
                var role = banObj.Channel.Guild.GetRole(roleID.Value);
                await banObj.BanTarget.AddRoleAsync(role);
                string ubanMessage = string.Format(BanResources.ReturnRoleLine, role.Name, banObj.BanTarget.Username);
                await banObj.Channel.SendMessageAsync(ubanMessage);
                banObj.BanTimer.Change(Timeout.Infinite, Timeout.Infinite);
                banObj.BanTimer.Dispose();
                string sqlBanDeleteQuery = string.Format(SQLiteResources.DeleteBanByGUID, banObj.GUID);
                string dbPath = string.Format(SQLiteResources.BanDBPath, banObj.Channel.Guild.Id);
                await SQLiteDataManager.PushToDB(dbPath, sqlBanDeleteQuery);
            }
        }     

        /// <summary>
        /// Сохранение бана в базе данных
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public static async Task SaveBanToDB(this DiscordBan ban)
        {
            string jsonString = await ban.SerializeToJson();
            string sqlQuery = $"INSERT INTO Bans (GUID, BanJSON) VALUES ('{ban.GUID}', '{jsonString}')"; 
            await SQLiteDataManager.PushToDB($"locals/Databases/{ban.Channel.Guild.Id}/Bans.sqlite", sqlQuery);
        }

        /// <summary>
        /// Сериализация бана в формат JSON
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public static async Task<string> SerializeToJson(this DiscordBan ban)
        {
            using (MemoryStream serializationStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(serializationStream, ban.GetReference());
                var jsonResult = Encoding.UTF8.GetString(serializationStream.ToArray());
                return jsonResult;
            }
        }

        /// <summary>
        /// Десериализация бана из формата JSON
        /// </summary>
        /// <param name="jsonText"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<DiscordBan> DeserializeFromJson(string jsonText)
        {
            using (MemoryStream serializationStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonText)))
            {
                var banDataObject = await JsonSerializer.DeserializeAsync<BanReference>(serializationStream);
                var ban = banDataObject.LoadOrigin();
                return ban;
            }
        }

        /// <summary>
        /// Загрузка банов из базы данных
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<List<DiscordBan>> LoadBansFromGuildDB(ulong guildID)
        {
            string dbPath = string.Format(SQLiteResources.BanDBPath, guildID);
            DataTable data = await SQLiteDataManager.GetDataFromDB(dbPath, SQLiteResources.SelectAllBans);
            List<DiscordBan> bans = new List<DiscordBan>();
            foreach (DataRow row in data.AsEnumerable())
            {
                bans.Add(await BanBuilder.BuildLoadedDiscordBan(row["GUID"].ToString(), row["BanJSON"].ToString()));
            }
            return bans;
        }
    }  
}
