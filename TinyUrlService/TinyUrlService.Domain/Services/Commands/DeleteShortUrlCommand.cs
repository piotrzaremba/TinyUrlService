using MediatR;

namespace TinyUrlService.Domain.Services.Commands;

public class DeleteShortUrlCommand : IRequest<bool>
{
    public string? ShortUrl { get; set; }
}
