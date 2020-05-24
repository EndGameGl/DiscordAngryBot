using ObjectDiscordAPI.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ObjectDiscordAPI.Resources.GuildResources;
using System.Drawing;
using System.IO;
using ObjectDiscordAPI.GatewayData;
using System.Net;

namespace ObjectDiscordAPI.Extensions
{
    /// <summary>
    /// Класс-расширение для получения данных от API дискорда
    /// </summary>
    public static class GETMethods
    {

        #region Методы для получения данных о гильдии

        /// <summary>
        /// Получение объекта гильдии от клиента дискорда
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="ID">Идентификатор гильдии</param>
        /// <returns></returns>
        public async static Task<Guild> GetGuildAsync(this DiscordClient client, ulong ID)
        {
            return JsonConvert.DeserializeObject<Guild>(await client.GET($"guilds/{ID}"));
        }

        /// <summary>
        /// Получение объекта пользователя
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="ID">Идентификатор пользователя</param>
        /// <returns></returns>
        public async static Task<User> GetUserAsync(this DiscordClient client, ulong ID)
        {
            return JsonConvert.DeserializeObject<User>(await client.GET($"users/{ID}"));
        }

        /// <summary>
        /// Получение списка всех участников сервера дискорда
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="ID">Идентификатор гильдии</param>
        /// <param name="maxAmount">Максимальное количество пользователей к получению (Макс. 1000)</param>
        /// <param name="afterUser">Параметр, определяющий, после какого пользователя будет получен список</param>
        /// <returns></returns>
        public async static Task<List<GuildMember>> GetGuildMembersAsync(this DiscordClient client, ulong ID, int maxAmount = 1, int afterUser = 0)
        {
            return JsonConvert.DeserializeObject<List<GuildMember>>(await client.GET($"guilds/{ID}/members?limit={maxAmount}&after{afterUser}"));
        }

        /// <summary>
        /// Получение конкретного участника гильдии дискорда
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="guildID">Идентификатор гильдии</param>
        /// <param name="memberID">Идентификатор гильдии</param>
        /// <returns></returns>
        public async static Task<GuildMember> GetGuildMemberAsync(this DiscordClient client, ulong guildID, ulong memberID)
        {
            return JsonConvert.DeserializeObject<GuildMember>(await client.GET($"guilds/{guildID}/members/{memberID}"));
        }

        /// <summary>
        /// Получение каналов гильдии
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="ID">Идентификатор гильдии</param>
        /// <returns></returns>
        public async static Task<Channel[]> GetGuildChannelsAsync(this DiscordClient client, ulong ID)
        {
            return JsonConvert.DeserializeObject<Channel[]>(await client.GET($"guilds/{ID}/channels"));
        }

        /// <summary>
        /// Получение всех банов гильдии
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="guildID">Идентификатор гильдии</param>
        /// <returns></returns>
        public async static Task<GuildBan[]> GetGuildBansAsync(this DiscordClient client, ulong guildID)
        {
            return JsonConvert.DeserializeObject<GuildBan[]>(await client.GET($"guilds/{guildID}/bans"));
        }

        /// <summary>
        /// Получение конкретного бана гильдии
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="guildID">Идентификатор гильдии</param>
        /// <param name="userID">Идентификатор пользователя</param>
        /// <returns></returns>
        public async static Task<GuildBan> GetGuildBanAsync(this DiscordClient client, ulong guildID, ulong userID)
        {
            return JsonConvert.DeserializeObject<GuildBan>(await client.GET($"guilds/{guildID}/bans/{userID}"));
        }

        /// <summary>
        /// Получение ролей гильдии
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="guildID">Идентификатор гильдии</param>
        /// <returns></returns>
        public async static Task<GuildRole[]> GetGuildRolesAsync(this DiscordClient client, ulong guildID)
        {
            return JsonConvert.DeserializeObject<GuildRole[]>(await client.GET($"guilds/{guildID}/roles"));
        }

        /// <summary>
        /// Получение конкретной роли гильдии
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="guildID">Идентификатор гильдии</param>
        /// <param name="roleID">Идентификатор роли</param>
        /// <returns></returns>
        public async static Task<GuildRole> GetGuildRoleAsync(this DiscordClient client, ulong guildID, ulong roleID)
        {
            return JsonConvert.DeserializeObject<GuildRole>(await client.GET($"guilds/{guildID}/roles/{roleID}"));
        }

        /// <summary>
        /// Получить голосовые регионы гильдии
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="guildID">Идентификатор гильдии</param>
        /// <returns></returns>
        public async static Task<VoiceRegion[]> GetGuildVoiceRegionsAsync(this DiscordClient client, ulong guildID)
        {
            return JsonConvert.DeserializeObject<VoiceRegion[]>(await client.GET($"guilds/{guildID}/regions"));
        }

        /// <summary>
        /// Получить количество людей, которые были бы кикнуты с сервера чисткой
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="guildID">Идентификатор гильдии</param>
        /// <param name="days">Количество дней</param>
        /// <returns></returns>
        public async static Task<GuildPrune> GetGuildPruneAsync(this DiscordClient client, ulong guildID, int days = 7)
        {
            return JsonConvert.DeserializeObject<GuildPrune>(await client.GET($"guilds/{guildID}/prune?days={days}"));
        }

        /// <summary>
        /// Получить все инвайты в гильдию
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="guildID">Идентификатор гильдии</param>
        /// <returns></returns>
        public async static Task<Invite[]> GetGuildInvitesAsync(this DiscordClient client, ulong guildID)
        {
            return JsonConvert.DeserializeObject<Invite[]>(await client.GET($"guilds/{guildID}/invites"));
        }

        /// <summary>
        /// Получить все интеграции гильдии
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="guildID">Идентификатор гильдии</param>
        /// <returns></returns>
        public async static Task<Integration[]> GetGuildIntegrationsAsync(this DiscordClient client, ulong guildID)
        {
            return JsonConvert.DeserializeObject<Integration[]>(await client.GET($"guilds/{guildID}/integrations"));
        }

        public async static Task<GuildEmbed> GetGuildEmbedAsync(this DiscordClient client, ulong guildID)
        {
            return JsonConvert.DeserializeObject<GuildEmbed>(await client.GET($"guilds/{guildID}/embed"));
        }

        public async static Task<Invite> GetVanityURLAsync(this DiscordClient client, ulong guildID)
        {
            return JsonConvert.DeserializeObject<Invite>(await client.GET($"guilds/{guildID}/vanity-url"));
        }

        public async static Task<Image> GetGuildWidgetImageAsync(this DiscordClient client, ulong guildID)
        {
            byte[] data = await client.GETFile($"guilds/{guildID}/widget.png");
            if (data != null)
            {
                using (var ms = new MemoryStream(data))
                {
                    Image image = Image.FromStream(ms);
                    return image;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Получение предпросмотра гильдии
        /// </summary>
        /// <param name="client">Клиент дискорда</param>
        /// <param name="guildID">Идентификатор гильдии</param>
        /// <returns></returns>
        public async static Task<GuildPreview> GetGuildPreviewAsync(this DiscordClient client, ulong guildID)
        {
            return JsonConvert.DeserializeObject<GuildPreview>(await client.GET($"guilds/{guildID}/preview"));
        }

        #endregion
        public async static Task<Gateway> GetGatewayAsync(this DiscordClient client)
        {
            return JsonConvert.DeserializeObject<Gateway>(await client.GET("gateway/bot"));
        }

        public async static Task<Message[]> GetChannelMessagesAsync(this DiscordClient client, ulong ChannelID, int MessagesAmount)
        {
            return JsonConvert.DeserializeObject<Message[]>(await client.GET($"channels/{ChannelID}/messages?limit={MessagesAmount}"));
        }
    }
}
