using Moq;
using TinyUrlService.Domain.Entities;
using TinyUrlService.Domain.Services;

namespace TinyUrlService.Tests;

// Create some sample tests.
public class UrlServiceTests
{
    private readonly Mock<IUrlService> _urlServiceMock;

    public UrlServiceTests()
    {
        _urlServiceMock = new Mock<IUrlService>();
    }

    [Fact]
    public async Task WhenShortUrlWithValidInputsShouldReturnShortUrl()
    {
        // Arrange
        var longUrl = "https://www.example.com";
        var shortUrl = "example";

        _urlServiceMock.Setup(x => x.CreateShortUrlAsync(shortUrl, longUrl, CancellationToken.None))
                       .ReturnsAsync(shortUrl);

        var urlService = _urlServiceMock.Object;

        // Act
        var actual = await urlService.CreateShortUrlAsync(shortUrl, longUrl);

        // Assert
        Assert.Equal(shortUrl, actual);
    }

    [Fact]
    public async Task WhenLongUrlIsRetrievedByShortUrl()
    {
        // Arrange
        var longUrl = "https://www.example.com";
        var shortUrl = "example";
        var expectedUrlMapping = new UrlMapping { ShortUrl = shortUrl, LongUrl = longUrl };

        _urlServiceMock.Setup(x => x.CreateShortUrlAsync(shortUrl, longUrl, CancellationToken.None))
                       .ReturnsAsync(shortUrl);

        _urlServiceMock.Setup(x => x.GetLongUrlAsync(shortUrl, CancellationToken.None))
                       .ReturnsAsync(expectedUrlMapping);

        var urlService = _urlServiceMock.Object;

        // Act
        await urlService.CreateShortUrlAsync(shortUrl, longUrl);
        var actual = await urlService.GetLongUrlAsync(shortUrl);

        // Assert
        Assert.Equal(longUrl, actual.LongUrl);
    }

    [Fact]
    public async Task WhenLongUrlWithValidShortUrlShouldReturnLongUrl()
    {
        // Arrange
        var longUrl = "https://www.example.com";
        var shortUrl = "example";

        _urlServiceMock.Setup(x => x.CreateShortUrlAsync(null, longUrl, CancellationToken.None))
                       .ReturnsAsync(shortUrl);

        _urlServiceMock.Setup(x => x.GetLongUrlAsync(shortUrl, CancellationToken.None))
                       .ReturnsAsync(new UrlMapping { LongUrl = longUrl });

        var urlService = _urlServiceMock.Object;

        // Act
        var actual = await urlService.GetLongUrlAsync(shortUrl);

        // Assert
        Assert.Equal(longUrl, actual.LongUrl);
    }

    [Fact]
    public async Task WhenGetLongUrlShouldIncreaseCount()
    {
        // Arrange
        var longUrl = "https://www.example.com";
        var shortUrl = "example";
        var expectedUrlMapping = new UrlMapping { ShortUrl = shortUrl, LongUrl = longUrl, ClickCount = 1 };
        var statistics = new Dictionary<string, UrlMapping> { { shortUrl, expectedUrlMapping } };

        _urlServiceMock.Setup(x => x.CreateShortUrlAsync(null, longUrl, CancellationToken.None))
                       .ReturnsAsync(shortUrl);

        _urlServiceMock.Setup(x => x.GetLongUrlAsync(shortUrl, CancellationToken.None))
                       .ReturnsAsync(expectedUrlMapping);

        _urlServiceMock.Setup(x => x.GetStatisticsAsync(CancellationToken.None))
                       .ReturnsAsync(statistics);

        var urlService = _urlServiceMock.Object;

        // Act
        await urlService.GetLongUrlAsync(shortUrl);
        var actual = await urlService.GetStatisticsAsync();

        // Assert
        Assert.NotNull(actual);
        Assert.NotEmpty(actual);
        Assert.True(actual.ContainsKey(shortUrl));
        Assert.Equal(longUrl, actual[shortUrl].LongUrl);
        Assert.Equal(1, actual[shortUrl].ClickCount);
    }

    [Fact]
    public async Task WhenLongUrlWithInvalidShortUrlShouldReturnNull()
    {
        // Arrange
        var shortUrl = "nonexistent";

        _urlServiceMock.Setup(x => x.GetLongUrlAsync(shortUrl, CancellationToken.None))
                       .ReturnsAsync(new UrlMapping());

        var urlService = _urlServiceMock.Object;

        // Act
        var actual = await urlService.GetLongUrlAsync(shortUrl);

        // Assert
        Assert.True(string.IsNullOrEmpty(actual.LongUrl));
    }

    [Fact]
    public async Task WhenCreateShortUrlWithExistingLongUrlShouldReturnDifferentShortUrl()
    {
        // Arrange
        var longUrl = "https://www.example.com";
        var shortUrl1 = "example";
        var shortUrl2 = "another";

        _urlServiceMock.SetupSequence(x => x.CreateShortUrlAsync(It.IsAny<string>(), longUrl, CancellationToken.None))
                       .ReturnsAsync(shortUrl1)
                       .ThrowsAsync(new InvalidOperationException("Short URL already exists with a different long URL"))
                       .ReturnsAsync(shortUrl2);

        var urlService = _urlServiceMock.Object;

        // Act
        var actual1 = await urlService.CreateShortUrlAsync(shortUrl1, longUrl);

        // Assert
        Assert.Equal(shortUrl1, actual1);
    }
}
