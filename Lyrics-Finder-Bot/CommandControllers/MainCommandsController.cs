using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Lyrics_Finder_Bot.Controllers
{
    public static class MainCommandsController
    {
        public static async Task StartCommand(Update update, ITelegramBotClient botClient, string[] parameters)
        {
            Console.WriteLine($"Start command called with parameters:\n{string.Join('\n', parameters)}");
        }
    }
}
