using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TinyUrlService.Domain.Entities;
using TinyUrlService.Domain.Services.Commands;
using TinyUrlService.Domain.Services.Queries;
using AutoMapper;
using System.Xml.Serialization;

namespace TinyUrlService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("ReactPolicy")]
public class UrlController : ControllerBase
{
    private readonly ActivitySource _activitySource;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UrlController(ActivitySource activitySource, IMediator mediator, IMapper mapper)
    {
        _activitySource = activitySource ?? throw new ArgumentNullException(nameof(activitySource));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateShortUrlAsync([FromBody] UrlMapping urlMapping, CancellationToken cancellationToken)
    {
        using Activity activity = _activitySource.StartActivity("CreateShortUrl");
        activity?.SetTag("UrlTag", urlMapping.LongUrl);
        var command = _mapper.Map<CreateShortUrlCommand>(urlMapping);
        var shortUrl = await _mediator.Send(command, cancellationToken);
        return Ok(shortUrl);
    }

    [HttpGet("{shortUrl}")]
    public async Task<IActionResult> GetLongUrlAsync(string shortUrl, CancellationToken cancellationToken)
    {
        using Activity activity = _activitySource.StartActivity("GetLongUrl");
        activity?.SetTag("UrlTag", shortUrl);
        var query = _mapper.Map<GetLongUrlQuery>(shortUrl);
        var urlMapping = await _mediator.Send(query, cancellationToken);
        return Ok(urlMapping.LongUrl); // This should be a Redirect but for some reason there is a CORS error.
    }

    [HttpDelete("{shortUrl}")]
    public async Task<IActionResult> DeleteShortUrlAsync(string shortUrl, CancellationToken cancellationToken)
    {

        using Activity activity = _activitySource.StartActivity("DeleteShortUrl");
        activity?.SetTag("UrlTag", shortUrl);
        var command = _mapper.Map<DeleteShortUrlCommand>(shortUrl);
        var success = await _mediator.Send(command, cancellationToken);
        if (success) return NoContent();
        return NotFound();
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStatisticsAsync(CancellationToken cancellationToken)
    {
        using Activity activity = _activitySource.StartActivity("GetStatistics");
        var query = new GetStatisticsQuery();
        var stats = await _mediator.Send(query, cancellationToken);
        return Ok(stats);
    }
}
