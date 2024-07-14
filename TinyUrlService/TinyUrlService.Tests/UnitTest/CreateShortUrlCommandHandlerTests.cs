using FluentValidation;
using FluentValidation.Results;
using Moq;
using TinyUrlService.Domain.Services;
using TinyUrlService.Domain.Services.Commands;
using TinyUrlService.Domain.Services.Handlers;
using Xunit;

namespace TinyUrlService.Tests;

public class CreateShortUrlCommandHandlerTests
{
    private readonly CreateShortUrlHandler _handler;
    private readonly Mock<IUrlService> _urlServiceMock;
    private readonly Mock<IValidator<CreateShortUrlCommand>> _validatorMock;

    public CreateShortUrlCommandHandlerTests()
    {
        _urlServiceMock = new Mock<IUrlService>();
        _validatorMock = new Mock<IValidator<CreateShortUrlCommand>>();
        _handler = new CreateShortUrlHandler(_urlServiceMock.Object, _validatorMock.Object);
    }

    [Fact]
    public async Task WhenWithValidInputsShouldReturnShortUrl()
    {
        // Arrange
        var longUrl = "https://www.example.com";
        var shortUrl = "example";
        var command = new CreateShortUrlCommand { LongUrl = longUrl, ShortUrl = shortUrl };

        _validatorMock
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _urlServiceMock
            .Setup(x => x.CreateShortUrlAsync(command.LongUrl, command.ShortUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(shortUrl);

        // Act
        var actual = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(shortUrl, actual);
        _urlServiceMock.Verify(x => x.CreateShortUrlAsync(command.LongUrl, command.ShortUrl, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WhenWithNullShortUrlShouldGenerateShortUrl()
    {
        // Arrange
        var longUrl = "https://www.example.com";
        var command = new CreateShortUrlCommand { LongUrl = longUrl, ShortUrl = null };
        var generatedShortUrl = "generated";

        _validatorMock
            .Setup(x => x.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _urlServiceMock
            .Setup(x => x.CreateShortUrlAsync(command.LongUrl, command.ShortUrl, It.IsAny<CancellationToken>()))
            .ReturnsAsync(generatedShortUrl);

        // Act
        var actual = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(generatedShortUrl, actual);
        _urlServiceMock.Verify(x => x.CreateShortUrlAsync(command.LongUrl, command.ShortUrl, It.IsAny<CancellationToken>()), Times.Once);
    }
}
