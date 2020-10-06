using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordAngryBot.CustomObjects
{
    public interface IDatabaseStorable<T, N> where T: IReferableTo<N>
    {
        Task<string> SerializeToJson();
        Task SaveToDB();
        Task UpdateAtDB();
        Task RemoveFromDB();
        Task<List<T>> LoadAllEntriesFromDB(ulong guildID);
        Task<T> LoadFromReference(N reference);
    }
}
