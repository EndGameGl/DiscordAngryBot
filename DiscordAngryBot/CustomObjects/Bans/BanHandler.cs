using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
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
        public static async Task<DiscordBan> Ban(this SocketGuildUser user, int? time, SocketRole role, ISocketMessageChannel channel)
        {
            DiscordBan ban = await BanBuilder.BuildDiscordBan(user, time, role, channel);

            if (ban.isInfinite)
            {
                await ban.banTarget.RemoveRoleAsync(ban.roleToBan);
                await ban.channel.SendMessageAsync($"Выдан бан пользователю {ban.banTarget.Mention}");
            }
            else
            {
                await ban.banTarget.RemoveRoleAsync(ban.roleToBan);
                ban.timer = new Timer(BanTimerCallBack, ban, (int)time, Timeout.Infinite);
                await ban.channel.SendMessageAsync($"Выдан бан пользователю {ban.banTarget.Mention} на {time / 60 / 1000} минут");
            }
            
            return ban;
        }

        /// <summary>
        /// Метод, вызываемый при окончании бана
        /// </summary>
        /// <param name="ban"></param>
        public static async void BanTimerCallBack(object ban)
        {
            DiscordBan objBan = (DiscordBan)ban;
            await objBan.banTarget.AddRoleAsync(objBan.roleToBan);
            await objBan.channel.SendMessageAsync($"Возвращена роль {objBan.roleToBan.Name} пользователю {objBan.banTarget.Username}");
            objBan.timer.Change(Timeout.Infinite, Timeout.Infinite);
            objBan.timer.Dispose();
            string sqlQuery = $"DELETE FROM Bans WHERE GUID = '{objBan.GUID}'";
            await SQLiteDataManager.PushToDB(Groups.configs.groups_dbPath, sqlQuery);
        }     

        /// <summary>
        /// Сохранение бана в базе данных
        /// </summary>
        /// <param name="ban"></param>
        /// <returns></returns>
        public static async Task SaveBanToDB(this DiscordBan ban)
        {
            string jsonString = await ban.SerializeToJson();
            int isInfinite = 0;
            if (ban.isInfinite)
                isInfinite = 1;
            string sqlQuery = $"INSERT INTO Bans (GUID, JSON, isInfinite) VALUES ('{ban.GUID}', '{jsonString}', {isInfinite})"; 
            await SQLIteHandler.SQLiteDataManager.PushToDB(Groups.configs.groups_dbPath, sqlQuery);
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
        public static async Task<DiscordBan> DeserializeFromJson(string jsonText, DiscordSocketClient client)
        {
            await ConsoleWriter.Write($"Deserializing ban from JSON", ConsoleWriter.InfoType.Notice);
            using (MemoryStream serializationStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonText)))
            {
                var banDataObject = await JsonSerializer.DeserializeAsync<BanJSONobject>(serializationStream);
                var ban = await banDataObject.ConvertToDiscordBan(client.GetGuild(636208919114547212));
                return ban;
            }
        }

        /// <summary>
        /// Загрузка банов из базы данных
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<List<DiscordBan>> LoadAllBansFromDB(DiscordSocketClient client)
        {
            await ConsoleWriter.Write($"Loading bans from database", ConsoleWriter.InfoType.Notice);
            string query = "SELECT * FROM Bans WHERE isInfinite = 0";
            DataTable data = await SQLiteDataManager.GetDataFromDB(Groups.configs.groups_dbPath, query);
            List<DiscordBan> bans = new List<DiscordBan>();
            await ConsoleWriter.Write($"Filling list with bans", ConsoleWriter.InfoType.Notice);
            foreach (DataRow row in data.AsEnumerable())
            {
                bans.Add(await BanBuilder.BuildLoadedDiscordBan(client, row["GUID"].ToString(), row["JSON"].ToString()));
            }
            return bans;
        }
    }

    /// <summary>
    /// Класс-конструктор банов
    /// </summary>
    public static class BanBuilder
    {
        /// <summary>
        /// Конструктор бана дискорда
        /// </summary>
        /// <param name="target"></param>
        /// <param name="time"></param>
        /// <param name="role"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static async Task<DiscordBan> BuildDiscordBan(SocketGuildUser target, int? time, SocketRole role, ISocketMessageChannel channel)
        {
            DateTime createdAt = DateTime.Now;
            DateTime? endsAt = null;
            if (time != null)
            {
                endsAt = createdAt.AddMilliseconds((double)time);
            }
            DiscordBan ban = new DiscordBan()
            {
                channel = channel,
                banTarget = target,
                GUID = Guid.NewGuid().ToString(),
                isInfinite = time == null,
                length = time,
                roleToBan = role,
                timer = null,
                createdAt = createdAt,
                endsAt = endsAt
            };
            return ban;
        }

        /// <summary>
        /// Конструктор бана дискорда на основе данных из базы
        /// </summary>
        /// <param name="client"></param>
        /// <param name="GUID"></param>
        /// <param name="JSON"></param>
        /// <returns></returns>
        public static async Task<DiscordBan> BuildLoadedDiscordBan(DiscordSocketClient client, string GUID, string JSON)
        {
            await ConsoleWriter.Write($"Building ban {GUID}", ConsoleWriter.InfoType.Notice);
            DiscordBan ban = await BanHandler.DeserializeFromJson(JSON, client);
            ban.GUID = GUID;
            DateTime currentTime = DateTime.Now;

            if (ban.endsAt.Value > currentTime) 
            {
                TimeSpan timeLeft = ban.endsAt.Value - currentTime;
                ban.timer = new Timer(BanHandler.BanTimerCallBack, ban, (int)timeLeft.TotalMilliseconds, Timeout.Infinite); 
            }
            else
            {
                ban.timer = new Timer(BanHandler.BanTimerCallBack, ban, 0, Timeout.Infinite);
            }
            return ban;
        }
    }
}
