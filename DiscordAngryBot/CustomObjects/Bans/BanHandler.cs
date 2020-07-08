using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.SQLIteHandler;
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

            if (ban.EndsAt == null)
            {
                await ban.BanTarget.RemoveRoleAsync(role);
                await ban.Channel.SendMessageAsync($"Выдан бан пользователю {ban.BanTarget.Mention}");
            }
            else
            {
                await ban.BanTarget.RemoveRoleAsync(role);

                ban.BanTimer = new Timer(BanTimerCallBack, ban, (int)time, Timeout.Infinite);

                if (isAuto == false)
                {
                    if (isSelf == false)
                    {
                        await ban.Channel.SendMessageAsync($"Выдан бан пользователю {ban.BanTarget.Mention} на {time / 60 / 1000} минут");
                    }
                    else 
                    {
                        await ban.Channel.SendMessageAsync($"Пользователь {ban.BanTarget.Mention} выпросил у хомяка себе бан на {time / 60 / 1000} минут");
                    }
                }
                else 
                {
                    await ban.Channel.SendMessageAsync($"Выдан автоматический бан за мат пользователю {ban.BanTarget.Mention}, если вы считаете его ошибочным, напишите администраторам");
                }
            }
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
            var roleID = BotCore.GetGuildDataCache(ban.Channel.Guild.Id).Settings.BanRoleID;
            await ban.BanTarget.AddRoleAsync(ban.Channel.Guild.GetRole(roleID.Value));
            ban.BanTimer?.Dispose();
            string sqlQuery = $"DELETE FROM Bans WHERE GUID = '{ban.GUID}'";
            await SQLiteDataManager.PushToDB($"locals/Databases/{ban.Channel.Guild.Id}/Bans.sqlite", sqlQuery);
            BotCore.GetDiscordGuildBans(ban.Channel.Guild.Id).Remove(ban);
        }

        /// <summary>
        /// Метод, вызываемый при окончании бана
        /// </summary>
        /// <param name="ban"></param>
        public static async void BanTimerCallBack(object ban)
        {
            DiscordBan objBan = (DiscordBan)ban;
            var roleID = BotCore.GetGuildDataCache(objBan.Channel.Guild.Id).Settings.BanRoleID;
            var role = objBan.Channel.Guild.GetRole(roleID.Value);
            await objBan.BanTarget.AddRoleAsync(role);
            await objBan.Channel.SendMessageAsync($"Возвращена роль {role.Name} пользователю {objBan.BanTarget.Username}");
            objBan.BanTimer.Change(Timeout.Infinite, Timeout.Infinite);
            objBan.BanTimer.Dispose();
            string sqlQuery = $"DELETE FROM Bans WHERE GUID = '{objBan.GUID}'";
            await SQLiteDataManager.PushToDB($"locals/Databases/{objBan.Channel.Guild.Id}/Bans.sqlite", sqlQuery);
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
                await JsonSerializer.SerializeAsync<BanJSONobject>(serializationStream, new BanJSONobject(ban));
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
        public static async Task<DiscordBan> DeserializeFromJson(string jsonText, SocketGuild guild)
        {
            using (MemoryStream serializationStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonText)))
            {
                var banDataObject = await JsonSerializer.DeserializeAsync<BanJSONobject>(serializationStream);
                var ban = await banDataObject.ConvertToDiscordBan(guild);
                return ban;
            }
        }

        /// <summary>
        /// Загрузка банов из базы данных
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<List<DiscordBan>> LoadBansFromGuildDB(SocketGuild guild)
        {
            string query = "SELECT * FROM Bans";
            DataTable data = await SQLiteDataManager.GetDataFromDB($"locals/Databases/{guild.Id}/Bans.sqlite", query);
            List<DiscordBan> bans = new List<DiscordBan>();
            foreach (DataRow row in data.AsEnumerable())
            {
                bans.Add(await BanBuilder.BuildLoadedDiscordBan(row["GUID"].ToString(), row["BanJSON"].ToString(), guild));
            }
            return bans;
        }
    }
    
}
