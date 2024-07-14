using FluentValidation;
using MediatR;
using TinyUrlService.API;
using TinyUrlService.Domain.Entities;

namespace TinyUrlService.Domain.Services.Handlers;

public class GetStatisticsHandler : IRequestHandler<GetStatisticsQuery, Dictionary<string, UrlMapping>>
{
    private readonly IUrlService _urlService;
    private readonly IValidator<GetStatisticsQuery> _validator;

    public GetStatisticsHandler(IUrlService urlService, IValidator<GetStatisticsQuery> validator)
    {
        _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<Dictionary<string, UrlMapping>> Handle(GetStatisticsQuery request, CancellationToken cancellationToken)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));

        return await _urlService.GetStatisticsAsync(cancellationToken);
    }
}

public class GetStatisticsValidator : AbstractValidator<GetStatisticsQuery>
{
    public GetStatisticsValidator()
    {
    }
}