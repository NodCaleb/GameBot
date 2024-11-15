using GameBot.Core.CodeGuess;

namespace GameBot.Desktop
{
    internal class Program
    {
        private static CodeGuessGame _game;

        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start!");

            Console.ReadKey();

            _game = new CodeGuessGame(4);

            Console.WriteLine($"I thought of a {_game.CodeLength}-digit code, try to guess it.");

            while (true)
            {

               Console.WriteLine("Enter your guess:");

                string guess = Console.ReadLine();

                var response = _game.Guess(guess);

                if (!response.CorrectInput)
                {
                    Console.WriteLine($"Please enter {_game.CodeLength} digits!");
                    continue;
                }

                if (response.CorrectGuess)
                {
                    Console.WriteLine("You won!");
                    break;
                }

                Console.WriteLine($"Correct digits and positoins: {response.CorrectSymbolAndPositionCount}" +
                    Environment.NewLine +
                    $"correct digits: {response.CorrectSymbolCount}");
                Console.WriteLine();
            }
        }
    }
}
