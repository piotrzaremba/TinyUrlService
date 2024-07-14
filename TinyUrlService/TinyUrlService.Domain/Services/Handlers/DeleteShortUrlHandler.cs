using FluentValidation;
using MediatR;
using TinyUrlService.Domain.Services.Commands;

namespace TinyUrlService.Domain.Services.Handlers;

public class DeleteShortUrlHandler : IRequestHandler<DeleteShortUrlCommand, bool>
{
    private readonly IUrlService _urlService;
    private readonly IValidator<DeleteShortUrlCommand> _validator;

    public DeleteShortUrlHandler(IUrlService urlService, IValidator<DeleteShortUrlCommand> validator)
    {
        _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async Task<bool> Handle(DeleteShortUrlCommand request, CancellationToken cancellationToken)
    {
        _ = request ?? throw new ArgumentNullException(nameof(request));

        return await _urlService.DeleteShortUrlAsync(request.ShortUrl, cancellationToken);
    }
}

public class DeleteShortUrlValidator : AbstractValidator<DeleteShortUrlCommand>
{
    public DeleteShortUrlValidator()
    {
        RuleFor(request => request.ShortUrl)
            .NotEmpty().WithMessage("URL cannot be empty");
    }
}