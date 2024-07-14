using AutoMapper;
using TinyUrlService.Domain.Entities;
using TinyUrlService.Domain.Services.Commands;
using TinyUrlService.Domain.Services.Queries;

namespace TinyUrlService.API.Infrastructure;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<UrlMapping, CreateShortUrlCommand>().ReverseMap();
        CreateMap<string, DeleteShortUrlCommand>().ForMember(dest => dest.ShortUrl, opt => opt.MapFrom(src => src));
        CreateMap<string, GetLongUrlQuery>().ForMember(dest => dest.ShortUrl, opt => opt.MapFrom(src => src));
    }
}