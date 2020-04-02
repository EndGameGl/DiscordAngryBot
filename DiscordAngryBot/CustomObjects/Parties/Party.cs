using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DiscordAngryBot.CustomObjects.Parties
{
    public class Party : IDisposable, IGroup
    {
        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public List<SocketUser> users { get; set; } = new List<SocketUser>();
        public int userLimit { get; set; } = 6;
        public SocketMessage sourceMessage { get; set; }
        public RestUserMessage targetMessage { get; set; }
        public DateTime created { get; set; }
        public string partyDestination { get; set; }
        public string localPath { get; set; }
        public GroupIO loadedInfo { get; set; }
        public bool isLoadedFromFile { get; set; }
        public SocketUser author { get; set; }

        /// <summary>
        /// Пустой конструктор группы
        /// </summary>
        public Party() { }
        /// <summary>
        /// Конструктор для группы, используемый при использовании команды
        /// </summary>
        /// <param name="message"></param>
        /// <param name="destination"></param>
        public Party(SocketMessage message, string[] destination)
        {
            partyDestination = String.Empty;
            foreach (var word in destination)
            {
                partyDestination += (" " + word);
            }
            sourceMessage = message;
            created = DateTime.Now;
            localPath = $"GroupCache\\Parties\\{sourceMessage.Author.Username}({sourceMessage.CreatedAt.ToLocalTime().ToString("yyyy.MM.dd-HH.mm.ss")}).json";
            author = sourceMessage.Author;
        }
        /// <summary>
        /// Добавить юзера в список юзеров
        /// </summary>
        /// <param name="user"></param>
        public async void Add(SocketUser user)
        {
            if (users.Count < 6) 
            {
                if (!users.Contains(user))
                    users.Add(user);
                else
                {
                    if (isLoadedFromFile)
                    {
                        await targetMessage.Channel.SendMessageAsync("Вы уже состоите в данной группе");
                    }
                    else
                    {
                        await sourceMessage.Channel.SendMessageAsync("Вы уже состоите в данной группе");
                    }
                }
            }
            else
            {
                if (isLoadedFromFile)
                {
                    await targetMessage.Channel.SendMessageAsync("Группа заполнена");
                }
                else
                {
                    await sourceMessage.Channel.SendMessageAsync("Группа заполнена");
                }
            }
        }
        /// <summary>
        /// Убрать юзера из списка юзеров
        /// </summary>
        /// <param name="user"></param>
        public async void Remove(SocketUser user)
        {
            if (users.Contains(user))
                users.Remove(user);
        }
        /// <summary>
        /// Первоначальная отправка сообщения
        /// </summary>
        public async void SendMessage()
        {
            string messageText = $"Собирается пати пользователем {sourceMessage.Author.Mention}: {partyDestination}\nОсталось {6 - users.Count()} мест.\nСостав группы:\n";
            string partyText = String.Empty;
            for (int i = 0; i < users.Where(x => x != null).Count(); i++)
            {
                partyText += $"{i+1}: {users[i].Mention}\n";
            }
            messageText += partyText;
            targetMessage = await sourceMessage.Channel.SendMessageAsync(messageText);
            await targetMessage.AddReactionAsync(new Emoji("✅"));
            await targetMessage.AddReactionAsync(new Emoji("\u274C"));
            await this.Save();
        }      
        /// <summary>
        /// Переписывание сообщения при новых реакциях
        /// </summary>
        public async void RewriteMessage()
        {
            string messageText = String.Empty;
            if (isLoadedFromFile)
            {
                messageText = $"Собирается пати пользователем {author.Mention}: {partyDestination}\nОсталось {6 - users.Count()} мест.\nСостав группы:\n";
            }
            else
            {
                messageText = $"Собирается пати пользователем {author.Mention}: {partyDestination}\nОсталось {6 - users.Count()} мест.\nСостав группы:\n";
            }
            string partyText = String.Empty;
            for (int i = 0; i < users.Where(x => x != null).Count(); i++)
            {
                partyText += $"{i + 1}: {users[i].Mention}\n";
            }
            messageText += partyText;
            await targetMessage.ModifyAsync(m => { m.Content = messageText; });
            await this.Save();
        }
        public bool IsRaid()
        {
            if (userLimit == 12)
            {
                return true;
            }
            else 
                return false;
        }
        public bool IsParty()
        {
            if (userLimit == 6)
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Метод удаления объекта
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            disposed = true;
        }
        public async Task Save()
        {
            GroupIO saver = new GroupIO(this);
            if (isLoadedFromFile)
            {
                var author = Program.serverObject.users.Where(x => x.Id == loadedInfo.sourceMessage_AuthorID).SingleOrDefault();
                using (FileStream stream = File.Create($"GroupCache\\Parties\\{author.Username}({loadedInfo.sourceMessage_CreatedAt.ToLocalTime().ToString("yyyy.MM.dd-HH.mm.ss")}).json"))
                {
                    await JsonSerializer.SerializeAsync<GroupIO>(stream, saver, new JsonSerializerOptions() { WriteIndented = true });
                }
            }
            else
            {
                using (FileStream stream = File.Create($"GroupCache\\Parties\\{sourceMessage.Author.Username}({sourceMessage.CreatedAt.ToLocalTime().ToString("yyyy.MM.dd-HH.mm.ss")}).json"))
                {
                    await JsonSerializer.SerializeAsync<GroupIO>(stream, saver, new JsonSerializerOptions() { WriteIndented = true });
                }
            }
        }
        public async Task<IGroup> Load(string text)
        {
            GroupIO groupInfo = JsonSerializer.Deserialize<GroupIO>(text);
            List<SocketUser> usersList = new List<SocketUser>();
            foreach (var userID in groupInfo.userIDs)
            {
                usersList.Add(Program.serverObject.users.Where(x => x.Id == userID).SingleOrDefault());
            }
            var target = await Program.serverObject.server.GetTextChannel(groupInfo.targetMessage_ChannelID).GetMessageAsync(groupInfo.targetMessage_ID);
            var authorObj = Program.serverObject.users.Where(x => x.Id == groupInfo.sourceMessage_AuthorID).SingleOrDefault();
            IGroup party = new Party()
            {
                users = usersList,
                userLimit = groupInfo.userLimit,
                created = groupInfo.created,
                partyDestination = groupInfo.partyDestination,
                localPath = $"GroupCache\\Parties\\{authorObj.Username}({groupInfo.sourceMessage_CreatedAt.ToLocalTime().ToString("yyyy.MM.dd-HH.mm.ss")}).json",
                targetMessage = (RestUserMessage)target,
                loadedInfo = groupInfo,
                isLoadedFromFile = true,
                disposed = false,
                handle = new SafeFileHandle(IntPtr.Zero, true),
                author = authorObj
            };
            return party;
        }
    }
}
