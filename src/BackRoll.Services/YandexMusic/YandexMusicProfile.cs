using AutoMapper;
using BackRoll.Services.Models;
using YandexMusicResolver.AudioItems;

namespace BackRoll.Services.YandexMusic
{
    public class YandexMusicProfile : Profile
    {
        public YandexMusicProfile()
        {
            CreateMap<YandexMusicTrack, Track>()
                .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Title))
                .ForMember(x => x.Url, opt => opt.MapFrom(x => x.Uri))
                .ForMember(x => x.Artists, opt => opt.MapFrom(x => x.Authors));

            CreateMap<YandexMusicArtist, Artist>();
        }
    }
}
