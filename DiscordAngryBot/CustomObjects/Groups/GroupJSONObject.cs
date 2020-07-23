using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{

    public enum GroupType
    {
        Party,
        Raid,
        GuildFight
    }

    /// <summary>
    /// Вспомогательный класс для сохранения данных группы
    /// </summary>
    public class GroupJSONObject
    {
        public GroupType _GroupType { get; set; }
        /// <summary>
        /// Идентификатор автора
        /// </summary>
        public ulong _AuthorID { get; set; }
        /// <summary>
        /// Идентификатор канала
        /// </summary>
        public ulong _ChannelID { get; set; }
        /// <summary>
        /// Идентификаторы всех пользователей в группе
        /// </summary>
        public List<ulong> _UsersID { get; set; } = new List<ulong>();
        /// <summary>
        /// Идентификатор целевого сообщения
        /// </summary>
        public ulong _TargetMessageID { get; set; }
        /// <summary>
        /// Цель сбора группы
        /// </summary>
        public string _Destination { get; set; }
        /// <summary>
        /// Дата создания группы
        /// </summary>
        public DateTime _CreatedAt { get; set; }
        /// <summary>
        /// Тип битвы БШ
        /// </summary>
        public GuildFightType? _GuildFightType { get; set; }
        /// <summary>
        /// Идентификаторы пользователей без гира (ТОЛЬКО ДЛЯ БИТВ БШ)
        /// </summary>
        public List<ulong>? _NoGearUsersID { get; set; }
        /// <summary>
        /// Идентификаторы пользователей на замену (ТОЛЬКО ДЛЯ БИТВ БШ)
        /// </summary>
        public List<ulong>? _UnwillingUsersID { get; set; }
        /// <summary>
        /// Идентификаторы пользоваелей, не уверенных в возможности быть (ТОЛЬКО ДЛЯ БИТВ БШ)
        /// </summary>
        public List<ulong>? _UnsureUsersID { get; set; }

        /// <summary>
        /// Пустой конструктор класса
        /// </summary>
        public GroupJSONObject() { }
        /// <summary>
        /// Конструктор классе на основе объекта Group
        /// </summary>
        /// <param name="group"></param>
        public GroupJSONObject(Group group)
        {
            _AuthorID = group.Author.Id;
            _ChannelID = group.Channel.Id;
            foreach (var user in group.Users)
            {
                _UsersID.Add(user.Id);
            }
            _TargetMessageID = group.TargetMessage.Id;
            _Destination = group.Destination;
            _CreatedAt = group.CreatedAt;
            if (group is Party)
                _GroupType = GroupType.Party;
            if (group is Raid)
                _GroupType = GroupType.Raid;
            if (group is GuildFight)
            {
                _GroupType = GroupType.GuildFight;
                _NoGearUsersID = new List<ulong>();
                _UnwillingUsersID = new List<ulong>();
                _UnsureUsersID = new List<ulong>();
                foreach (var user in ((GuildFight)group).noGearUsers)
                {
                    _NoGearUsersID.Add(user.Id);
                }
                foreach (var user in ((GuildFight)group).unwillingUsers)
                {
                    _UnwillingUsersID.Add(user.Id);
                }
                foreach (var user in ((GuildFight)group).unsureUsers)
                {
                    _UnsureUsersID.Add(user.Id);
                }
                _GuildFightType = ((GuildFight)group).GuildFightType;
            }
        }

        /// <summary>
        /// Ковертация объекта данных в группу
        /// </summary>
        /// <param name="guild">Сервер, к которому принадлежит группа</param>
        /// <returns></returns>
        public async Task<Group> ConvertToGroup(SocketGuild guild)
        {
            Group group = new Group();
            var Channel = guild.GetTextChannel(_ChannelID);
            var Author = guild.GetUser(_AuthorID);
            var TargetMessage = (RestUserMessage)await Channel.GetMessageAsync(_TargetMessageID);
            var CreatedAt = _CreatedAt;
            var Destination = _Destination;
            switch (_GroupType)
            {
                case GroupType.Party:
                    group = new Party()
                    {
                        Author = Author,
                        Channel = Channel,
                        TargetMessage = TargetMessage,
                        CreatedAt = CreatedAt,
                        Destination = Destination,
                        UserLimit = 6
                    };
                    break;
                case GroupType.Raid:
                    group = new Raid()
                    {
                        Author = Author,
                        Channel = Channel,
                        TargetMessage = TargetMessage,
                        CreatedAt = CreatedAt,
                        Destination = Destination,
                        UserLimit = 12
                    };
                    break;
                case GroupType.GuildFight:
                    group = new GuildFight()
                    {
                        Author = Author,
                        Channel = Channel,
                        TargetMessage = TargetMessage,
                        CreatedAt = CreatedAt,
                        Destination = Destination,
                        UserLimit = null
                    };
                    break;
            }
            group.Users = new List<SocketGuildUser>();
            foreach (var userId in _UsersID)
            {
                var user = guild.GetUser(userId);
                if (user != null)
                    group.Users.Add(user);
            }
            if (group is GuildFight)
            {
                ((GuildFight)group).noGearUsers = new List<SocketGuildUser>();
                ((GuildFight)group).unwillingUsers = new List<SocketGuildUser>();
                ((GuildFight)group).unsureUsers = new List<SocketGuildUser>();
                ((GuildFight)group).GuildFightType = _GuildFightType.Value;
                foreach (var userId in _NoGearUsersID)
                {
                    var user = guild.GetUser(userId);
                    if (user != null)
                        ((GuildFight)group).noGearUsers.Add(user);
                }
                foreach (var userId in _UnwillingUsersID)
                {
                    var user = guild.GetUser(userId);
                    if (user != null)
                        ((GuildFight)group).unwillingUsers.Add(user);
                }
                foreach (var userId in _UnsureUsersID)
                {
                    var user = guild.GetUser(userId);
                    if (user != null)
                        ((GuildFight)group).unsureUsers.Add(user);
                }
            }
            return group;
        }
    }
}
