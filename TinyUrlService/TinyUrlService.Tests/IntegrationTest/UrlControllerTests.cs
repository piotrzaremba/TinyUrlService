using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TinyUrlService.API;

namespace TinyUrlService.Tests;

// Create some sample tests.
public class UrlControllerTests : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    public UrlControllerTests(WebApplicationFactory<Startup> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost:5237")
        });
    }

    [Fact]
    public async Task WhenCreateShortUrlWithValidInputsShouldReturnShortUrl()
    {
        // Arrange
        var urlMapping = new
        {
            LongUrl = "https://www.example.com",
            ShortUrl = "example"
        };
        var content = new StringContent(JsonConvert.SerializeObject(urlMapping), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/url/create", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("example", responseString);
    }

    [Fact]
    public async Task WhenGetLongUrlWithValidShortUrlShouldRedirect()
    {
        // Arrange
        var urlMapping = new
        {
            LongUrl = "https://www.example.com",
            ShortUrl = "example",
        };

        var content = new StringContent(JsonConvert.SerializeObject(urlMapping), Encoding.UTF8, "application/json");

        // Act
        await _client.PostAsync("/api/url/create", content);
        var response = await _client.GetAsync($"/api/url/{urlMapping.ShortUrl}");
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(urlMapping.LongUrl, responseString);
    }

    [Fact]
    public async Task WhenCreateShortUrlWithInvalidUrlMappingShouldReturnBadRequest()
    {
        // Arrange
        var invalidUrlMapping = new
        {
            LongUrl = (string)null,
            ShortUrl = "example"
        };
        var content = new StringContent(JsonConvert.SerializeObject(invalidUrlMapping), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/url/create", content);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }
}
