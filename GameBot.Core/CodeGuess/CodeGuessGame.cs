using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameBot.Core.CodeGuess;

public class CodeGuessGame
{
    private readonly int[] _code;

    public int CodeLength => _code.Length;

    public CodeGuessGame(int codeLength = 4, bool repeatsAllowed = false)
    {
        _code = GenerateRandomArray(codeLength, 0, 9, repeatsAllowed);
    }

    public CodeGuessResponse Guess(string guess)
    {
        CodeGuessResponse response = new CodeGuessResponse();
        response.CorrectInput = true;

        if (guess.Length != _code.Length)
        {
            response.CorrectInput = false;
        }

        int[] guessArray = new int[guess.Length];

        for (int i = 0; i < guess.Length; i++)
        {
            if (!int.TryParse(guess[i].ToString(), out guessArray[i]))
            {
                response.CorrectInput = false;
                break;
            }
        }

        if (!response.CorrectInput)
        {
            return response;
        }

        return Guess(guessArray);
    }

    public CodeGuessResponse Guess(int[] guess)
    {
        CodeGuessResponse response = new CodeGuessResponse();
        response.CorrectInput = true;

        if (guess.Length != _code.Length)
        {
            response.CorrectInput = false;
        }

        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] < 0 || guess[i] > 9)
            {
                response.CorrectInput = false;
                break;
            }
        }

        if (!response.CorrectInput)
        {
            return response;
        }

        response.CorrectGuess = true;

        var tempCode = new int[_code.Length];

        for (int i = 0; i < guess.Length; i++)
        {
            tempCode[i] = _code[i];

            if (guess[i] != _code[i])
            {
                response.CorrectGuess = false;
            }
        }

        if (response.CorrectGuess)
        {
            return response;
        }

        response.CorrectSymbolAndPositionCount = 0;
        response.CorrectSymbolCount = 0;

        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == tempCode[i])
            {
                response.CorrectSymbolAndPositionCount++;
                tempCode[i] = -1;
                guess[i] = -1;
            }
        }

        for (int i = 0; i < guess.Length; i++)
        {
            if (guess[i] == -1)
            {
                continue; // already matched
            }

            var index = Array.IndexOf(tempCode, guess[i]);

            if (index >= 0)
            {
                response.CorrectSymbolCount++;
                tempCode[index] = -1;
            }
        }

        return response;
    }

    public CodeGuessGame(int[] code)
    {
        for (int i = 0; i < code.Length; i++)
        {
            if (code[i] < 0 || code[i] > 9)
            {
                throw new ArgumentException("Code must be an array of integers between 0 and 9.");
            }
        }

        _code = code;        
    }

    public static int[] GenerateRandomArray(int n, int minValue, int maxValue, bool repeatsAllowed)
    {
        Random random = new Random(DateTime.UtcNow.Millisecond);
        int[] randomNumbers = new int[n];

        if (repeatsAllowed)
        {
            for (int i = 0; i < n; i++)
            {
                randomNumbers[i] = random.Next(minValue, maxValue);
            } 
        }
        else
        {
            randomNumbers = Enumerable.Range(minValue, maxValue - minValue + 1).OrderBy(x => random.Next()).Take(n).ToArray();
        }

        return randomNumbers;
    }
}