using Lyrics_Finder_Bot.Logics;
using Lyrics_Finder_Bot.Logics.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Lyrics_Finder_Bot.Controllers
{
    public class MainCommandsController
    {
        public static async Task StartCommand(Update update, ITelegramBotClient botClient, string[] parameters)
        {
            _ = await botClient.SendTextMessageAsync(
                chatId: update?.Message?.Chat.Id,
                text: $"💡 Welcome to lyrics finder bot.\n  Please use <code>/find music name</code> to search the music lyrics\n  Contact owner: {ConfigurationHelper.config.GetValue<string>("OwnerUsername")}",
                parseMode: ParseMode.Html
            );
        }

        public static async Task FindCommand(Update update, ITelegramBotClient botClient, string[] parameters)
        {
            _ = await botClient.SendTextMessageAsync(
                chatId: update?.Message?.Chat.Id,
                text: $"Proccessing..."
            );

            var getLyricsResponse = await GeniusLyricsExtractor.GetLyricsByPhrase(parameters[0]);
            if(!getLyricsResponse)
                _ = await botClient.SendTextMessageAsync(
                    chatId: update?.Message?.Chat.Id,
                    text: $"❌ An error has occured"
                );

            var sendPhotoRequest = await botClient.SendPhotoAsync(
                photo: InputFile.FromUri(getLyricsResponse.Result.SongArtImageUrl),
                chatId: update?.Message?.Chat.Id
            );

            _ = await botClient.SendTextMessageAsync(
                chatId: update?.Message?.Chat.Id,
                text: $"🔰 {getLyricsResponse.Result.Title}\n\n{(getLyricsResponse.Result.Lyrics.Length > 1800 ? getLyricsResponse.Result.Lyrics[..1800] : getLyricsResponse.Result.Lyrics)}\n\n📅 Release Date: {getLyricsResponse.Result.ReleaseDate}\n👥 Artist Names: {getLyricsResponse.Result.ArtistNames}",
                replyToMessageId: sendPhotoRequest.MessageId,
                parseMode: ParseMode.Html
            );
        }
    }
}
