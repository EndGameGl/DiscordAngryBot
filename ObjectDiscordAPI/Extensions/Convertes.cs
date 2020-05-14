using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.Extensions
{
    public static class Convertes
    {
        public static async Task<ArraySegment<byte>> ConvertObjectToArraySegment(this object obj, bool shouldSerializeNulls)
        {
            JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            if (shouldSerializeNulls)
                jsonSerializerSettings.NullValueHandling = NullValueHandling.Include;
            string jsonString = await Task.Run(() => JsonConvert.SerializeObject(obj, Formatting.None, jsonSerializerSettings)); ;
            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);
            var sendBuffer = new ArraySegment<byte>(bytes);
            return sendBuffer;
        }
    }
}
