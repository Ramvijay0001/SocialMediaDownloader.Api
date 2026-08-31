using SocialMediaDownloader.Api.Models;

namespace SocialMediaDownloader.Api.Services;

public class MediaService : IMediaService
{
    public async Task<MediaResponse> GetMetadataAsync(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("Invalid URL.");
        }

        var host = uri.Host.ToLowerInvariant();

        string platform;

        if (host.Contains("youtube.com") || host.Contains("youtu.be"))
        {
            platform = "YouTube";
        }
        else if (host.Contains("instagram.com"))
        {
            platform = "Instagram";
        }
        else if (host.Contains("facebook.com"))
        {
            platform = "Facebook";
        }
        else if (host.Contains("tiktok.com"))
        {
            platform = "TikTok";
        }
        else
        {
            platform = "Unknown";
        }

        await Task.Delay(100);

        return new MediaResponse
        {
            Title = $"Sample Video - {platform}",
            Thumbnail = "",
            Duration = "02:35",
            Qualities =
            [
                new MediaQuality
                {
                    Label = "Original",
                    Resolution = "1920x1080",
                    Format = "MP4"
                },
                new MediaQuality
                {
                    Label = "720p",
                    Resolution = "1280x720",
                    Format = "MP4"
                },
                new MediaQuality
                {
                    Label = "480p",
                    Resolution = "854x480",
                    Format = "MP4"
                },
                new MediaQuality
                {
                    Label = "360p",
                    Resolution = "640x360",
                    Format = "MP4"
                }
            ]
        };
    }
}