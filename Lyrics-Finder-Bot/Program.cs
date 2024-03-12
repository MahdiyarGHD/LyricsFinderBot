using Lyrics_Finder_Bot.Controllers;
using Lyrics_Finder_Bot.Logics;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.SimpleCommandHandler.Logics;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


WebProxy webProxy = new("socks5://127.0.0.1:2334");
HttpClient httpClient = new(
    new HttpClientHandler { Proxy = webProxy, UseProxy = true, }
);

var botClient = new TelegramBotClient("BOT_TOKEN", httpClient);

using CancellationTokenSource cts = new();

ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = [] 
};

CommandHandler commandHandler = new();

commandHandler.RegisterCommandHandler(command: "/start", handler: MainCommandsController.StartCommand);
commandHandler.RegisterCommandHandler(command: "/find", parameters: "%s", handler: MainCommandsController.FindCommand);

botClient.StartReceiving(
    updateHandler: commandHandler.Resolve,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

cts.Cancel();

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
