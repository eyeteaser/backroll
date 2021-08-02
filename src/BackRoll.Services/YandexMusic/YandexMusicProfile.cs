using AutoMapper;
using BackRoll.Services.Models;
using Yandex.Music.Api.Models.Artist;
using Yandex.Music.Api.Models.Search.Artist;
using Yandex.Music.Api.Models.Search.Track;
using Yandex.Music.Api.Models.Track;

namespace BackRoll.Services.YandexMusic
{
    public class YandexMusicProfile : Profile
    {
        public YandexMusicProfile()
        {
            CreateMap<YTrack, Track>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Title));

            CreateMap<YSearchTrackModel, Track>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Title));

            CreateMap<YArtist, Artist>();

            CreateMap<YSearchArtist, Artist>();
        }
    }
}
