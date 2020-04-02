using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects.Parties
{
    
    public class Raid : IDisposable, IGroup
    {
        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        public List<SocketUser> users { get; set; } = new List<SocketUser>();
        public int userLimit { get; set; } = 12;
        public SocketMessage sourceMessage { get; set; }
        public RestUserMessage targetMessage { get; set; }
        public DateTime created { get; set; }
        public string partyDestination { get; set; }
        public string localPath { get; set; }
        public GroupIO loadedInfo { get; set; }
        public bool isLoadedFromFile { get; set; }
        public SocketUser author { get; set; }
        public Raid() { }
        public Raid(SocketMessage message, string[] destination)
        {
            partyDestination = String.Empty;
            foreach (var word in destination)
            {
                partyDestination += (" " + word);
            }
            sourceMessage = message;
            created = DateTime.Now;
            localPath = $"GroupCache\\Raids\\{sourceMessage.Author.Username}({sourceMessage.CreatedAt.ToLocalTime().ToString("yyyy.MM.dd-HH.mm.ss")}).json";
            author = sourceMessage.Author;
        }
        public async void Add(SocketUser user)
        {
            if (users.Count < userLimit)
            {
                if (!users.Contains(user))
                    users.Add(user);
                else
                {
                    if (isLoadedFromFile)
                    {
                        await targetMessage.Channel.SendMessageAsync("Вы уже состоите в данном рейде");
                    }
                    else
                    {
                        await sourceMessage.Channel.SendMessageAsync("Вы уже состоите в данном рейде");
                    }
                }
            }
            else
            {
                if (isLoadedFromFile)
                {
                    await targetMessage.Channel.SendMessageAsync("Рейд заполнен");
                }
                else
                {
                    await sourceMessage.Channel.SendMessageAsync("Рейд заполнен");
                }
            }
        }
        public async void Remove(SocketUser user)
        {
            if (users.Contains(user))
                users.Remove(user);
        }
        public async void SendMessage()
        {
            
            string messageText = $"Собирается рейд пользователем {sourceMessage.Author.Mention}: {partyDestination}\nОсталось {12 - users.Count()} мест.\nСостав рейда:\n";
            string partyText = String.Empty;
            for (int i = 0; i < users.Where(x => x != null).Count(); i++)
            {
                partyText += $"{i + 1}: {users[i].Mention}\n";
            }
            messageText += partyText;
            targetMessage = await sourceMessage.Channel.SendMessageAsync(messageText);
            await targetMessage.AddReactionAsync(new Emoji("✅"));
            await targetMessage.AddReactionAsync(new Emoji("\u274C"));
            await this.Save();
        }
        public async void RewriteMessage()
        {
            string messageText = String.Empty;
            if (isLoadedFromFile)
            {
                var author = Program.serverObject.users.Where(x => x.Id == loadedInfo.sourceMessage_AuthorID).SingleOrDefault();
                messageText = $"Собирается рейд пользователем {author.Mention}: {partyDestination}\nОсталось {12 - users.Count()} мест.\nСостав группы:\n";
            }
            else
            {
                messageText = $"Собирается рейд пользователем {sourceMessage.Author.Mention}: {partyDestination}\nОсталось {12 - users.Count()} мест.\nСостав группы:\n";
            }
            string partyText = String.Empty;
            for (int i = 0; i < users.Where(x => x != null).Count(); i++)
            {
                partyText += $"{i + 1}: {users[i].Mention}\n";
            }
            messageText += partyText;
            await this.Save();
            await targetMessage.ModifyAsync(m => { m.Content = messageText; });
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
                using (FileStream stream = File.Create($"GroupCache\\Raids\\{author.Username}({loadedInfo.sourceMessage_CreatedAt.ToLocalTime().ToString("yyyy.MM.dd-HH.mm.ss")}).json"))
                {
                    await JsonSerializer.SerializeAsync<GroupIO>(stream, saver, new JsonSerializerOptions() { WriteIndented = true });
                }
            }
            else
            {
                using (FileStream stream = File.Create($"GroupCache\\Raids\\{sourceMessage.Author.Username}({sourceMessage.CreatedAt.ToLocalTime().ToString("yyyy.MM.dd-HH.mm.ss")}).json"))
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
            IGroup raid = new Raid()
            {
                users = usersList,
                userLimit = groupInfo.userLimit,
                created = groupInfo.created,
                partyDestination = groupInfo.partyDestination,
                localPath = $"GroupCache\\Raids\\{authorObj.Username}({groupInfo.sourceMessage_CreatedAt.ToLocalTime().ToString("yyyy.MM.dd-HH.mm.ss")}).json",
                targetMessage = (RestUserMessage)target,
                loadedInfo = groupInfo,
                isLoadedFromFile = true,
                disposed = false,
                handle = new SafeFileHandle(IntPtr.Zero, true),
                author = authorObj
            };
            return raid;
        }
    }
}
