using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Microsoft.Extensions.Caching.Memory;
using GameBot.Core.CodeGuess;
using System.Xml.Linq;

//Bot tutorial: https://gitlab.com/Athamaxy/telegram-bot-tutorial/-/blob/main/TutorialBot.cs

string _greetings = "Привет! Напиши /game1 или /game2 чтобы начать игру.";

var cache = new MemoryCache(new MemoryCacheOptions());

var bot = new TelegramBotClient("7986779846:AAGcqs0zWm5wPxAsFgQTCYjCau3UtwzTZW0");
var me = await bot.GetMe();
//Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool, so we use cancellation token
bot.StartReceiving(
    updateHandler: HandleUpdate,
    errorHandler: HandleError,
    cancellationToken: cts.Token
);

// Tell the user the bot is online
Console.WriteLine(
    $"Bot @{me.Username} is running." +
    Environment.NewLine +
    $"Listening for updates." +
    Environment.NewLine +
    $"Press enter to stop"
    );
Console.ReadLine();

// Send cancellation request to stop the bot
cts.Cancel();

// Each time a user interacts with the bot, this method is called
async Task HandleUpdate(ITelegramBotClient _, Update update, CancellationToken cancellationToken)
{
    switch (update.Type)
    {
        // A message was received
        case UpdateType.Message:
            await HandleMessage(update.Message!);
            break;

        // A button was pressed
        case UpdateType.CallbackQuery:
            await HandleButton(update.CallbackQuery!);
            break;
    }
}

async Task HandleError(ITelegramBotClient _, Exception exception, CancellationToken cancellationToken)
{
    await Console.Error.WriteLineAsync(exception.Message);
}

async Task HandleMessage(Message msg)
{
    var user = msg.From;
    var text = msg.Text ?? string.Empty;

    if (user is null)
        return;

    // Print to console
    Console.WriteLine($"{user.FirstName} wrote {text}");

    // When we get a command, we react accordingly
    if (text.StartsWith("/"))
    {
        await HandleCommand(user.Id, text);
        return;
    }

    if (cache.TryGetValue(user.Id, out CodeGuessGame game))
    {
        var response = game.Guess(text);

        if (!response.CorrectInput)
        {
            await bot.SendMessage(
                user.Id,
                $"Пожалуйста, введите {game.CodeLength} цифры!",
                cancellationToken: cts.Token
            );
        }
        else if (response.CorrectGuess)
        {
            await bot.SendMessage(
                user.Id,
                "Вы выиграли 😉",
                cancellationToken: cts.Token
            );
        }
        else
        {
            await bot.SendMessage(
                user.Id,
                $"Верных цифр: {response.CorrectSymbolCount}" +
                Environment.NewLine +
                $"Верных цифр в правильном месте: {response.CorrectSymbolAndPositionCount}",
                cancellationToken: cts.Token
            );
        }

        return;
    }

    await bot.SendMessage(
        user.Id,
        _greetings,
        cancellationToken: cts.Token
    );
}


async Task HandleCommand(long userId, string command)
{
    CodeGuessGame game;

    switch (command)
    {
        case "/start":
            await bot.SendMessage(
                userId,
                _greetings,
                cancellationToken: cts.Token
            );
            break;

        case "/game1":
            game = new CodeGuessGame(4);
            cache.Set(userId, game, TimeSpan.FromMinutes(15));
            await bot.SendMessage(
                userId,
                $"Я загадал код из {game.CodeLength} уникальных цифр, попробуй угадать ;)",
                cancellationToken: cts.Token
            );
            break;

        case "/game2":
            game = new CodeGuessGame(6, true);
            cache.Set(userId, game, TimeSpan.FromMinutes(15));
            await bot.SendMessage(
                userId,
                $"Я загадал код из {game.CodeLength} цифр (цифры могут повторяться), попробуй угадать ;)",
                cancellationToken: cts.Token
            );
            break;

        case "/menu":
            await SendMenu(userId);
            break;
    }

    await Task.CompletedTask;
}

async Task SendMenu(long userId)
{
    //await bot.SendTextMessageAsync(
    //    userId,
    //    firstMenu,
    //    ParseMode.Html,
    //    replyMarkup: firstMenuMarkup
    //);
}

async Task HandleButton(CallbackQuery query)
{
    //string text = string.Empty;
    //InlineKeyboardMarkup markup = new(Array.Empty<InlineKeyboardButton>());

    //if (query.Data == nextButton)
    //{
    //    text = secondMenu;
    //    markup = secondMenuMarkup;
    //}
    //else if (query.Data == backButton)
    //{
    //    text = firstMenu;
    //    markup = firstMenuMarkup;
    //}

    //// Close the query to end the client-side loading animation
    //await bot.AnswerCallbackQueryAsync(query.Id);

    //// Replace menu text and keyboard
    //await bot.EditMessageTextAsync(
    //    query.Message!.Chat.Id,
    //    query.Message.MessageId,
    //    text,
    //    ParseMode.Html,
    //    replyMarkup: markup
    //);
}
