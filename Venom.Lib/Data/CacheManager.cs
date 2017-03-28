using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace Venom.Lib.Data
{
    public static class CacheManager
    {
        /// <summary>
        /// Adiciona um novo item ao cache
        /// </summary>
        /// <typeparam name="T">Tipo do contexto a ser criado</typeparam>
        /// <returns>Uma instância de T</returns>
        public static T AddItem<T>(string key, T value)
        {
            MemoryCache.Default.Add(key, value, System.Runtime.Caching.MemoryCache.InfiniteAbsoluteExpiration);
            return value;
        }

        /// <summary>
        /// Obtém um item do cache
        /// </summary>
        /// <typeparam name="T">Tipo do item a ser recuperado</typeparam>
        /// <returns>o tipo solicitado</returns>
        public static T Get<T>(string key)
        {
            var item = MemoryCache.Default.Get(key);
            return item == null ? default(T) : (T)item;
        }


        /// <summary>
        /// Remove um item do cache
        /// </summary>
        public static void Remove(string key)
        {
            MemoryCache.Default.Remove(key);
        }

        /// <summary>
        /// Destroy todos os itens em cache na memória
        /// </summary>
        public static void Destroy()
        {
            MemoryCache.Default.Dispose();
        }
    }
}
