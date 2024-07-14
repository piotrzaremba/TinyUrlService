using MediatR;
using TinyUrlService.Domain.Entities;

namespace TinyUrlService.API
{
    public class GetStatisticsQuery : IRequest<Dictionary<string, UrlMapping>>
    {
    }
}
