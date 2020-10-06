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
    /// Class for handling operations with discord bans
    /// </summary>
    public static class BanHandler
    {
        /// <summary>
        /// Ban specified user
        /// </summary>
        /// <param name="user">User to be banned</param>
        /// <param name="time">Time for ban</param>
        /// <param name="role">Role that will be removed</param>
        /// <param name="channel">Channel on which ban was made</param>
        /// <param name="isAuto">Wheter this ban was automatic</param>
        /// <param name="isSelf">Wheter this ban was self-given</param>
        /// <returns></returns>
        public static async Task Ban(this SocketGuildUser user, int? time, SocketRole role, SocketTextChannel channel, bool isAuto, bool isSelf = false)
        {
            DiscordBan ban = await BanBuilder.BuildDiscordBan(user, time, channel);
            string banMessageText = string.Empty;           
            if (ban.IsPermanent)
            {
                banMessageText = string.Format(BanResources.InfiniteBanLine, ban.BanTarget.Mention);               
            }
            else
            {                
                ban.BanTimer = new Timer(BanTimerCallBack, ban, time.Value, Timeout.Infinite);
                if (isAuto)
                {
                    banMessageText = string.Format(BanResources.AutoBanLine, ban.BanTarget.Mention);                    
                }
                else 
                {
                    if (isSelf)
                    {
                        banMessageText = string.Format(BanResources.FiniteSelfBanLine, ban.BanTarget.Mention, time / 60 / 1000);                      
                    }
                    else
                    {
                        banMessageText = string.Format(BanResources.FiniteBanLine, ban.BanTarget.Mention, time / 60 / 1000);
                    }
                }
            }
            await ban.BanTarget.RemoveRoleAsync(role);
            await ban.Channel.SendMessageAsync(banMessageText);
            BotCore.GetDiscordGuildBans(channel.Guild.Id).Add(ban);
            await ban.SaveBanToDB();
        }

        /// <summary>
        /// Unban specified user
        /// </summary>
        /// <param name="ban">Ban object</param>
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
                await SQLiteExtensions.PushToDB(dbPath, sqlBanDeleteQuery);
                BotCore.GetDiscordGuildBans(ban.Channel.Guild.Id).Remove(ban);
            }
        }

        /// <summary>
        /// Ban timer callback method
        /// </summary>
        /// <param name="ban">Ban object</param>
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
                await SQLiteExtensions.PushToDB(dbPath, sqlBanDeleteQuery);
            }
        }     

        /// <summary>
        /// Saves ban reference to DB
        /// </summary>
        /// <param name="ban">Ban object</param>
        /// <returns></returns>
        public static async Task SaveBanToDB(this DiscordBan ban)
        {
            string jsonString = await ban.SerializeToJson();
            string sqlQuery = $"INSERT INTO Bans (GUID, BanJSON) VALUES ('{ban.GUID}', '{jsonString}')"; 
            await SQLiteExtensions.PushToDB($"locals/Databases/{ban.Channel.Guild.Id}/Bans.sqlite", sqlQuery);
        }

        /// <summary>
        /// Serialize ban to JSON
        /// </summary>
        /// <param name="ban">Ban object</param>
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
        /// Deserialize ban from JSON reference
        /// </summary>
        /// <param name="jsonText">JSON data</param>
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
        /// Load all bans from guild DB
        /// </summary>
        /// <param name="guildID">Guild ID</param>
        /// <returns></returns>
        public static async Task<List<DiscordBan>> LoadBansFromGuildDB(ulong guildID)
        {
            string dbPath = string.Format(SQLiteResources.BanDBPath, guildID);
            DataTable data = await SQLiteExtensions.GetDataFromDB(dbPath, SQLiteResources.SelectAllBans);
            List<DiscordBan> bans = new List<DiscordBan>();
            foreach (DataRow row in data.AsEnumerable())
            {
                bans.Add(await BanBuilder.BuildLoadedDiscordBan(row["GUID"].ToString(), row["BanJSON"].ToString()));
            }
            return bans;
        }
    }  
}
