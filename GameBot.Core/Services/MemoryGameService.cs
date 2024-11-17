using GameBot.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GameBot.Core.Services;

public class MemoryGameService : IGameService
{
    private readonly MemoryCache _cache;

    public MemoryGameService()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
    }

    public void AddGame(string key, IGame game)
    {
        _cache.Set(key, game);
    }

    public IGame? GetGame(string key)
    {
        return _cache.Get<IGame>(key);
    }
}
