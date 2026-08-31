using Microsoft.AspNetCore.Mvc;
using SocialMediaDownloader.Api.Models;
using SocialMediaDownloader.Api.Services;

namespace SocialMediaDownloader.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly IMediaService _mediaService;

    public MediaController(IMediaService mediaService)
    {
        _mediaService = mediaService;
    }

    [HttpPost("metadata")]
    public async Task<IActionResult> GetMetadata(
        [FromBody] MediaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("URL is required.");
        }

        var result = await _mediaService.GetMetadataAsync(request.Url);

        return Ok(result);
    }
}