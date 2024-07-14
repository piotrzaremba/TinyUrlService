using MediatR;
using TinyUrlService.Domain.Entities;

namespace TinyUrlService.Domain.Services.Queries;

public class GetLongUrlQuery : IRequest<UrlMapping>
{
    public string ShortUrl { get; set; }
}
