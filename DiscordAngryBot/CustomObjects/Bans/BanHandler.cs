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
    public static class BanHandler
    {
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
        public static async Task SaveBanToDB(this DiscordBan ban)
        {
            string jsonString = await ban.SerializeToJson();
            int isInfinite = 0;
            if (ban.isInfinite)
                isInfinite = 1;
            string sqlQuery = $"INSERT INTO Bans (GUID, JSON, isInfinite) VALUES ('{ban.GUID}', '{jsonString}', {isInfinite})"; 
            await SQLIteHandler.SQLiteDataManager.PushToDB(Groups.configs.groups_dbPath, sqlQuery);
        }
        public static async Task<string> SerializeToJson(this DiscordBan ban)
        {
            using (MemoryStream serializationStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync<BanJSONobject>(serializationStream, new BanJSONobject(ban));
                var jsonResult = Encoding.UTF8.GetString(serializationStream.ToArray());
                return jsonResult;
            }
        }
        public static async Task<DiscordBan> DeserializeFromJson(string jsonText, DiscordSocketClient client)
        {
            using (MemoryStream serializationStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonText)))
            {
                var groupDataObject = await JsonSerializer.DeserializeAsync<BanJSONobject>(serializationStream);
                var group = await groupDataObject.ConvertToDiscordBan(client.GetGuild(636208919114547212));
                return group;
            }
        }
        public static async Task<List<DiscordBan>> LoadAllBansFromDB(DiscordSocketClient client)
        {
            string query = "SELECT * FROM Bans WHERE isInfinite = 0";
            DataTable data = await SQLiteDataManager.GetDataFromDB(Groups.configs.groups_dbPath, query);
            List<DiscordBan> bans = new List<DiscordBan>();
            foreach (DataRow row in data.AsEnumerable())
            {
                bans.Add(await BanBuilder.BuildLoadedDiscordBan(client, row["GUID"].ToString(), row["JSON"].ToString()));
            }
            return bans;
        }
    }
    public static class BanBuilder
    {
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
        public static async Task<DiscordBan> BuildLoadedDiscordBan(DiscordSocketClient client, string GUID, string JSON)
        {
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
