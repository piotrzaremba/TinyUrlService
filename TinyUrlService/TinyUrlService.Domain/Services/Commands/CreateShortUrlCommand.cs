using MediatR;

namespace TinyUrlService.Domain.Services.Commands;

public class CreateShortUrlCommand : IRequest<string>
{
    public string? LongUrl { get; set; }
    public string? ShortUrl { get; set; }
}
