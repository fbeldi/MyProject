using Microsoft.Extensions.Caching.Memory;
using MyProject.Models;
using Newtonsoft.Json;
using System;

namespace MyProject.Utility
{
    public static class Utils
    {
        private const string url = "https://eurosportdigital.github.io/eurosport-csharp-developer-recruitment/headtohead.json";
        private const string PlayersCacheKey = "Key##@0";
        public static async Task<List<Player>?> GetPlayersFromCacheAsync(HttpClient _httpClient, IMemoryCache _memoryCache)
        {
            // Check if players list is already in cache
            if (!_memoryCache.TryGetValue(PlayersCacheKey, out List<Player>? players))
            {
                // If not in cache, fetch from external source
                var response = await _httpClient.GetStringAsync(url);
                var playersList = JsonConvert.DeserializeObject<PlayersList>(response);
                players = playersList?.players;

                // Set the players list in the cache with an optional expiration
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(60)
                };
                _memoryCache.Set(PlayersCacheKey, players, cacheEntryOptions);
            }

            return players;
        }
    }
}
