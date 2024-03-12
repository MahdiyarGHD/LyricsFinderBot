using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EasyMicroservices.ServiceContracts;
using HtmlAgilityPack;
using Lyrics_Finder_Bot.Contracts;
using Lyrics_Finder_Bot.Logics.Helpers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Telegram.Bot.Types.InlineQueryResults;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace Lyrics_Finder_Bot.Logics
{
    public static partial class GeniusLyricsExtractor
    {
        private static readonly HttpClient httpClient = new()
        {
            DefaultRequestHeaders = { Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "GENIUS_API_TOKEN") }
        };

        [GeneratedRegex("<.*?>", RegexOptions.Compiled)]
        private static partial Regex TagsMatch();

        private static async Task<MessageContract<string>> ExtractLyrics(string? url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    throw new ArgumentException($"'{nameof(url)}' cannot be null or empty.", nameof(url));
                }

                var data = await httpClient.GetStringAsync(url);
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(data);

                var lyricsNode = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='lyrics']");
                var lyrics = lyricsNode?.InnerText.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(lyrics))
                {
                    var snippetNodes = htmlDocument.DocumentNode.SelectNodes("//div[starts-with(@class,'Lyrics__Container')]");
                    foreach (var elem in snippetNodes)
                    {
                        var snippet = elem.InnerHtml
                            .Replace("<br>", "\n")
                            .Replace(TagsMatch().Replace(elem.InnerHtml, string.Empty), "");
                        lyrics += System.Net.WebUtility.HtmlDecode(snippet).Trim() + "\n\n";
                    }
                }

                return TagsMatch().Replace(lyrics, string.Empty).Trim();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occurred while extracting lyrics from genius: {ex.Message}");
                return FailedReasonType.Unknown;
            }
        }

        public static async Task<MessageContract<GetLyricsResponseContract>> GetLyricsByPhrase(string phrase)
        {
            try
            {
                var data = await httpClient.GetStringAsync($"{ConfigurationHelper.config?.GetValue<string>("GeniusAPIAddress")?.TrimEnd('/')}/search?q={Uri.EscapeDataString(phrase)}");
                var deserializedData = JsonConvert.DeserializeObject<GeniusSearchResponse>(data);
                var extractResult = await ExtractLyrics(deserializedData?.Response?.Hits?.FirstOrDefault()?.Result.Url);
                if (!extractResult)
                    throw new Exception("Exctracting lyrics from genius api wasn't successful.");

                return new GetLyricsResponseContract
                {
                    Lyrics = extractResult.Result,
                    ArtistNames = deserializedData?.Response?.Hits?.FirstOrDefault()?.Result.ArtistNames,
                    ReleaseDate = deserializedData?.Response?.Hits?.FirstOrDefault()?.Result.ReleaseDate,
                    SongArtImageUrl = deserializedData?.Response?.Hits?.FirstOrDefault()?.Result.SongArtImageUrl,
                    Title = deserializedData?.Response?.Hits?.FirstOrDefault()?.Result.Title
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occurred while getting lyrics from genius: {ex.Message}");
                return FailedReasonType.Unknown;
            }
        }

    }
}
