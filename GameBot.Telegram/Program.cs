using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using GameBot.Core.CodeGuess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GameBot.Core.Interfaces;
using GameBot.Core.Services;
using Microsoft.Extensions.Hosting;

//Bot tutorial: https://gitlab.com/Athamaxy/telegram-bot-tutorial/-/blob/main/TutorialBot.cs

internal class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();


        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IGameService, MemoryGameService>();
                services.AddSingleton<ITelegramBotClient>(provider =>
                {
                    string botToken = configuration["Telegram:BotToken"];
                    return new TelegramBotClient(botToken);
                });
                services.AddHostedService<BackgroundWorker>();
            })
            .Build();

        await host.RunAsync();
    }

    private class BackgroundWorker : BackgroundService
    {
        string _greetings = "Привет! Напиши /game1 или /game2 чтобы начать игру.";
        private readonly IGameService _gameService;
        private readonly ITelegramBotClient _bot;

        public BackgroundWorker(IGameService gameService, ITelegramBotClient bot)
        {
            _gameService = gameService;
            _bot = bot;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var me = await _bot.GetMe();

            _bot.StartReceiving(
                updateHandler: HandleUpdate,
                errorHandler: HandleError,
                cancellationToken: stoppingToken
            );

            Console.WriteLine(
                $"Bot @{me.Username} is running." +
                Environment.NewLine +
                $"Listening for updates." +
                Environment.NewLine +
                $"Press enter to stop"
                );

            while (!stoppingToken.IsCancellationRequested)
            {
                
            }
        }

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

            var iGame = _gameService.GetGame(user.Id.ToString());

            if (iGame is not null && iGame.GetType() == typeof(CodeGuessGame))
            {
                var game = (CodeGuessGame)iGame;

                var response = game.Guess(text);

                if (!response.CorrectInput)
                {
                    await _bot.SendMessage(
                        user.Id,
                        $"Пожалуйста, введите {game.CodeLength} цифры!"
                    );
                }
                else if (response.CorrectGuess)
                {
                    await _bot.SendMessage(
                        user.Id,
                        "Вы выиграли 😉"
                    );
                }
                else
                {
                    await _bot.SendMessage(
                        user.Id,
                        $"Верных цифр: {response.CorrectSymbolCount}" +
                        Environment.NewLine +
                        $"Верных цифр в правильном месте: {response.CorrectSymbolAndPositionCount}"
                    );
                }

                return;
            }

            await _bot.SendMessage(
                user.Id,
                _greetings
            );
        }


        async Task HandleCommand(long userId, string command)
        {
            CodeGuessGame game;

            switch (command)
            {
                case "/start":
                    await _bot.SendMessage(
                        userId,
                        _greetings
                    );
                    break;

                case "/game1":
                    game = new CodeGuessGame(4);
                    _gameService.AddGame(userId.ToString(), game);
                    await _bot.SendMessage(
                        userId,
                        $"Я загадал код из {game.CodeLength} уникальных цифр, попробуй угадать ;)"
                    );
                    break;

                case "/game2":
                    game = new CodeGuessGame(6, true);
                    _gameService.AddGame(userId.ToString(), game);
                    await _bot.SendMessage(
                        userId,
                        $"Я загадал код из {game.CodeLength} цифр (цифры могут повторяться), попробуй угадать ;)"
                    );
                    break;

                case "/menu":
                    await SendMenu(userId);
                    break;
            }

            await Task.CompletedTask;
        }

        async Task HandleButton(CallbackQuery query)
        {

        }

        async Task SendMenu(long userId)
        {
            await _bot.SendMessage(
                userId,
                _greetings
            );
        }
    }
}