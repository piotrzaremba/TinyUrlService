using FluentValidation;
using MediatR;
using TinyUrlService.Domain.Services.Commands;

namespace TinyUrlService.Domain.Services.Handlers;

public class CreateShortUrlHandler : IRequestHandler<CreateShortUrlCommand, string>
{
    private readonly IUrlService _urlService;
    private readonly IValidator<CreateShortUrlCommand> _validator;

    public CreateShortUrlHandler(IUrlService urlService, IValidator<CreateShortUrlCommand> validator)
    {
        _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<string> Handle(CreateShortUrlCommand request, CancellationToken cancellationToken)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));
        
        return await _urlService.CreateShortUrlAsync(request.LongUrl, request.ShortUrl, cancellationToken);
    }
}

public class CreateShortUrlValidator : AbstractValidator<CreateShortUrlCommand>
{
    public CreateShortUrlValidator()
    {
        RuleFor(request => request.ShortUrl)
            .Must(IsValidShortUrl).WithMessage("Invalid Short URL format");

        RuleFor(request => request.LongUrl)
            .NotEmpty().WithMessage("Long URL cannot be empty")
            .Must(IsUrlValid).WithMessage("Invalid URL format");
    }

    private bool IsValidShortUrl(string? shortUrl)
    {
        return string.IsNullOrWhiteSpace(shortUrl) || !shortUrl.Contains(" ");
    }

    private bool IsUrlValid(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return false;
        }

        bool result = Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        return result;
    }
}
