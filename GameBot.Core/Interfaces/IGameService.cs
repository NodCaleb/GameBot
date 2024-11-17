namespace GameBot.Core.Interfaces;

public interface IGameService
{
    public IGame? GetGame(string key);
    public void AddGame(string key, IGame game);
}
