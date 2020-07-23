using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ObjectDiscordAPI.CacheData
{
    public static class ObjectComparer
    {
        public async static Task<T> CompareAndUpdate<T>(this T targetObject, T comparedTo)
        {
            await Task.Run(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Type type = typeof(T);
                var properties = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (var property in properties)
                {
                    object defaultValue = type.GetProperty(property.Name).GetValue(targetObject, null);
                    object newValue = type.GetProperty(property.Name).GetValue(comparedTo, null);
                    if (defaultValue == newValue || newValue == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (newValue is int)
                        {
                            if ((int)newValue == 0)
                                continue;
                        }
                        defaultValue = newValue;
                    }
                }
                stopwatch.Stop();
                Console.WriteLine($"Comparing object of type {type} took {stopwatch.Elapsed.TotalMilliseconds} ms");
            });           
            return targetObject;            
        }
    }
}
