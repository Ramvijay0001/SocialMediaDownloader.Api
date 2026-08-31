namespace SocialMediaDownloader.Api.Models;

public class MediaResponse
{
    public string Title { get; set; } = string.Empty;

    public string Thumbnail { get; set; } = string.Empty;

    public string Duration { get; set; } = string.Empty;

    public List<MediaQuality> Qualities { get; set; } = [];
}

public class MediaQuality
{
    public string Label { get; set; } = string.Empty;

    public string Resolution { get; set; } = string.Empty;

    public string Format { get; set; } = string.Empty;
}