using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;
namespace SocialMediaDownloader.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly YoutubeClient _youtube;
        private readonly IHttpClientFactory _httpClientFactory;
        public VideoController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _youtube = new YoutubeClient();
        }

        [HttpPost("extract")]
        public async Task<IActionResult> GetVideoDetails([FromBody] VideoRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Url))
            {
                return BadRequest(new { Message = "URL is required" });
            }

            try
            {
                var cleanUrl = request.Url.Split('?')[0];

                // 1. Get Metadata
                var video = await _youtube.Videos.GetAsync(cleanUrl);

                // 2. Get Stream Manifest
                var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(cleanUrl);

                // 3. Get Video Streams (Only Video or Video+Audio combined)
                var videoOnlyOrMuxed = streamManifest
                    .GetVideoOnlyStreams()
                    .Where(s => s.Container.Name == "mp4") // Only MP4 formats
                    .Select(s => new
                    {
                        Quality = s.VideoQuality.Label,
                        Container = s.Container.Name,
                        Type = "Video",
                        DownloadUrl = s.Url,
                        Size = $"{s.Size.MegaBytes:F2} MB"
                    });

                // 4. Get Audio Streams (MP3/M4A/WebM)
                var audioOnly = streamManifest
                    .GetAudioOnlyStreams()
                    .Select(s => new
                    {
                        Quality = $"{s.Bitrate.KiloBitsPerSecond:F0} kbps",
                        Container = s.Container.Name,
                        Type = "Audio",
                        DownloadUrl = s.Url,
                        Size = $"{s.Size.MegaBytes:F2} MB"
                    });

                // Combine both Video and Audio streams
                var allStreams = videoOnlyOrMuxed.Concat<object>(audioOnly);

                return Ok(new
                {
                    Title = video.Title,
                    Thumbnail = video.Thumbnails.FirstOrDefault()?.Url,
                    Streams = allStreams
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Error fetching video details", Error = ex.Message });
            }
        }
        [HttpGet("download-stream")]
        public async Task<IActionResult> DownloadStream([FromQuery] string url, [FromQuery] string fileName)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var stream = await httpClient.GetStreamAsync(url);

            return File(stream, "video/mp4", $"{fileName}.mp4");
        }
        [HttpGet("download")]
        public async Task<IActionResult> DownloadVideo([FromQuery] string streamUrl, [FromQuery] string fileName = "video")
        {
            if (string.IsNullOrEmpty(streamUrl))
                return BadRequest("Stream URL is required.");

            var client = _httpClientFactory.CreateClient();

            // YouTube stream-ஐ C# Backend வழியே பெறுதல்
            var response = await client.GetAsync(streamUrl, HttpCompletionOption.ResponseHeadersRead);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "Failed to stream video from source.");

            var stream = await response.Content.ReadAsStreamAsync();
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "video/mp4";

            // Attachment header மூலம் பிரவுசரில் நேரடியாக டவுன்லோட் ஆக வைப்பது
            return File(stream, contentType, $"{fileName}.mp4");
        }
    }

    }
public class VideoRequest
{
    public string Url { get; set; }
}