using AutoMapper;
using BackRoll.Services.Models;
using SpotifyAPI.Web;

namespace BackRoll.Services.Spotify
{
    public class SpotifyProfile : Profile
    {
        public SpotifyProfile()
        {
            CreateMap<FullTrack, Track>();

            CreateMap<SimpleArtist, Artist>();
        }
    }
}
