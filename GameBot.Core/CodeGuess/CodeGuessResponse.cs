namespace GameBot.Core.CodeGuess;

public class CodeGuessResponse
{
    public bool CorrectInput { get; set; }
    public bool CorrectGuess { get; set; }
    public int CorrectSymbolAndPositionCount { get; set; }
    public int CorrectSymbolCount { get; set; }
}
