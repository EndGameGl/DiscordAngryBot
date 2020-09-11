using Discord;
using Discord.WebSocket;
using DiscordAngryBot.CustomObjects.ConsoleOutput;
using DiscordAngryBot.ReactionHandlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Groups
{
    /// <summary>
    /// Class for building groups
    /// </summary>
    public static class GroupBuilder
    {
        /// <summary>
        /// Build group party template
        /// </summary>
        /// <param name="sourceMessage">Source message</param>
        /// <param name="args">Creation parameters</param>
        /// <returns></returns>
        public static Group BuildPartyTemplate(SocketMessage sourceMessage, string[] args)
        {
            Group partyGroup = new Group()
            {
                Author = (SocketGuildUser)sourceMessage.Author,
                Channel = (SocketTextChannel)sourceMessage.Channel,
                CreatedAt = DateTime.Now,
                Destination = (args != null) ? string.Join(" ", args) : "",
                GUID = Guid.NewGuid().ToString(),
                UserLists = new List<UserList>(1)
                {
                    new UserList()
                    {
                        UserLimit = 6,
                        Users = new List<SocketGuildUser>(),
                        ListName = "Участники группы",
                        ListEmoji = EmojiGetter.GetEmoji(Emojis.WhiteCheckMark)
                    }
                },
                Type = GroupType.Simple
            };            
            return partyGroup;
        }

        /// <summary>
        /// Build group raid template
        /// </summary>
        /// <param name="sourceMessage">Source message</param>
        /// <param name="args">Creation parameters</param>
        /// <returns></returns>
        public async static Task<Group> BuildRaidTemplate(SocketMessage sourceMessage, string[] args)
        {
            Group raidGroup = new Group()
            {
                Author = (SocketGuildUser)sourceMessage.Author,
                Channel = (SocketTextChannel)sourceMessage.Channel,
                CreatedAt = DateTime.Now,
                Destination = (args != null) ? string.Join(" ", args) : "",
                GUID = Guid.NewGuid().ToString(),
                UserLists = new List<UserList>(1)
                {
                    new UserList()
                    {
                        UserLimit = 12,
                        Users = new List<SocketGuildUser>(),
                        ListName = "Участники рейда",
                        ListEmoji = EmojiGetter.GetEmoji(Emojis.WhiteCheckMark)
                    }
                },
                Type = GroupType.Simple
            };
            await sourceMessage.DeleteAsync();
            return raidGroup;
        }

        /// <summary>
        /// Build group guild fight template
        /// </summary>
        /// <param name="sourceMessage">Source message</param>
        /// <param name="args">Creation message</param>
        /// <param name="type">Guild fight type</param>
        /// <returns></returns>
        public async static Task<GuildFight> BuildGuildFight(SocketMessage sourceMessage, string[] args, GuildFightType type)
        {
            GuildFight guildFight = new GuildFight()
            {
                Author = (SocketGuildUser)sourceMessage.Author,
                Channel = (SocketTextChannel)sourceMessage.Channel,
                CreatedAt = DateTime.Now,
                Destination = (args != null) ? string.Join(" ", args) : "",
                GUID = Guid.NewGuid().ToString(),
                GuildFightType = type,
                Type = GroupType.GuildFight
            };
            switch (guildFight.GuildFightType)
            {
                case GuildFightType.EV:
                    guildFight.UserLists = new List<UserList>()
                    {
                        new UserList()
                        {
                            ListEmoji = EmojiGetter.GetEmoji(Emojis.WhiteCheckMark),
                            ListName = "Иду, гир есть",
                            UserLimit = null,
                            Users = new List<SocketGuildUser>()                     
                        },
                        new UserList()
                        {
                            ListEmoji = EmojiGetter.GetEmoji(Emojis.Feet),
                            ListName = "Иду, но гир слабый/нет вообще",
                            UserLimit = null,
                            Users = new List<SocketGuildUser>()
                        },
                        new UserList()
                        {
                            ListEmoji = EmojiGetter.GetEmoji(Emojis.Pig),
                            ListName = "Могу пойти, если людей не хватит",
                            UserLimit = null,
                            Users = new List<SocketGuildUser>()
                        },
                        new UserList()
                        {
                            ListEmoji = EmojiGetter.GetEmoji(Emojis.QuestionMark),
                            ListName = "Пока не уверен",
                            UserLimit = null,
                            Users = new List<SocketGuildUser>()
                        }
                    };
                    break;
                case GuildFightType.PR:
                    guildFight.UserLists = new List<UserList>()
                    {
                        new UserList()
                        {
                            ListEmoji = EmojiGetter.GetEmoji(Emojis.WhiteCheckMark),
                            ListName = "Основной состав буду",
                            UserLimit = null,
                            Users = new List<SocketGuildUser>()
                        },
                        new UserList()
                        {
                            ListEmoji = EmojiGetter.GetEmoji(Emojis.NegativeSquaredCrossMark),
                            ListName = "Основной состав не смогу",
                            UserLimit = null,
                            Users = new List<SocketGuildUser>()
                        },
                        new UserList()
                        {
                            ListEmoji = EmojiGetter.GetEmoji(Emojis.BallotBoxWithCheck),
                            ListName = "Резерв готов помочь",
                            UserLimit = null,
                            Users = new List<SocketGuildUser>()
                        },
                        new UserList()
                        {
                            ListEmoji = EmojiGetter.GetEmoji(Emojis.RegionalIndicatorX),
                            ListName = "Резерв не смогу",
                            UserLimit = null,
                            Users = new List<SocketGuildUser>()
                        }
                    };
                    break;
            }
            await sourceMessage.DeleteAsync();
            return guildFight;
        }

        /// <summary>
        /// Build group from DB json data
        /// </summary>
        /// <param name="GUID">Group GUID</param>
        /// <param name="json">JSON data</param>
        /// <returns></returns>
        public async static Task<Group> BuildLoadedGroup(string GUID, string json)
        {
            await Debug.Log($"Building group {GUID}", LogInfoType.Notice);
            Group group = await GroupHandler.DeserializeFromJson(json);
            if (group != null)
            {
                group.GUID = GUID;
                if (group is GuildFight)
                    return group as GuildFight;
                else
                    return group;
            }
            else
                return group;
        }

        /// <summary>
        /// Build poll (test)
        /// </summary>
        /// <param name="sourceMessage">Source message</param>
        /// <param name="args">Creation parameters</param>
        /// <returns></returns>
        public async static Task<Group> BuildPoll(SocketMessage sourceMessage, string[] args)
        {
            await Debug.Log("Building poll...");
            Group poll = new Group()
            {
                Author = (SocketGuildUser)sourceMessage.Author,
                Channel = (SocketTextChannel)sourceMessage.Channel,
                CreatedAt = DateTime.Now,
                Destination = "",
                GUID = Guid.NewGuid().ToString(),
                Type = GroupType.Poll,
                UserLists = new List<UserList>()
            };
            int optionsAmount = (int)args.Length / 2;
            await Debug.Log($"This poll will have {optionsAmount} options");
            for (int i = 0; i < optionsAmount; i += 1)
            {
                var list = new UserList()
                {
                    Users = new List<SocketGuildUser>(),
                    UserLimit = null,
                    ListEmoji = new Emoji(args[i * 2]),
                    ListName = args[i * 2 + 1]
                };
                poll.UserLists.Add(list);
            }
            await Debug.Log($"This poll will have {poll.UserLists.Count} lists...");
            return poll;
        }
    }
}
