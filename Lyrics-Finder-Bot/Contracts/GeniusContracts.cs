using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lyrics_Finder_Bot.Contracts
{
    public class GeniusSearchHit
    {
        public class GeniusSearchResult
        {
            public int Id { get; set; }
            public string Title { get; set; }
            [JsonProperty("artist_names")]
            public string ArtistNames { get; set; }
            public string Url { get; set; }
            [JsonProperty("release_date_for_display")]
            public string ReleaseDate { get; set; }
            [JsonProperty("song_art_image_url")]
            public string SongArtImageUrl { get; set; }
        }

        public GeniusSearchResult Result { get; set; }
    }

    public class GeniusSearchMeta
    {
        public int Status { get; set; }
    }

    public class GeniusSearchHitsResponse
    {
        public List<GeniusSearchHit> Hits { get; set; }
    }

    public class GeniusSearchResponse
    {
        public GeniusSearchMeta Meta { get; set; }
        public GeniusSearchHitsResponse Response { get; set; }
    }

    public class GetLyricsResponseContract
    {
        public string? Title { get; set; }
        public string? Lyrics { get; set; }
        public string? ArtistNames { get; set; }
        public string? ReleaseDate { get; set; }
        public string? SongArtImageUrl { get; set; }
    }
}
