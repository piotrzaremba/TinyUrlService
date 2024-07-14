namespace TinyUrlService.Domain.Entities;

public class UrlMapping
{
    public string? ShortUrl { get; set; }
    public string? LongUrl { get; set; }
    public int? ClickCount { get; set; }
}
