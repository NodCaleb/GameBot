using GameBot.Core.Interfaces;

namespace GameBot.Core.Services;

public class MemoryGameService : IGameService
{
    private readonly Dictionary<string, IGame> _games;

    public MemoryGameService()
    {
        _games = new Dictionary<string, IGame>();
    }

    public void AddGame(string key, IGame game)
    {
        _games[key] = game;
    }

    public IGame? GetGame(string key)
    {
        return _games.TryGetValue(key, out var game) ? game : null;
    }

    public void StopGame(string key)
    {
        _games.Remove(key);        
    }
}
