
using FluentValidation;
using MediatR;
using TinyUrlService.Domain.Entities;
using TinyUrlService.Domain.Services.Commands;
using TinyUrlService.Domain.Services.Queries;

namespace TinyUrlService.Domain.Services.Handlers;

public class GetLongUrlHandler : IRequestHandler<GetLongUrlQuery, UrlMapping>
{
    private readonly IUrlService _urlService;
    private readonly IValidator<GetLongUrlQuery> _validator;

    public GetLongUrlHandler(IUrlService urlService, IValidator<GetLongUrlQuery> validator)
    {
        _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<UrlMapping> Handle(GetLongUrlQuery request, CancellationToken cancellationToken)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));

        return await _urlService.GetLongUrlAsync(request.ShortUrl, cancellationToken);
    }
}

public class GetLongUrlValidator : AbstractValidator<GetLongUrlQuery>
{
    public GetLongUrlValidator()
    {
        RuleFor(request => request.ShortUrl)
            .NotEmpty().WithMessage("URL cannot be empty");
    }
}