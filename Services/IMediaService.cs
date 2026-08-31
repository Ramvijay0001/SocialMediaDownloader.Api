using SocialMediaDownloader.Api.Models;

namespace SocialMediaDownloader.Api.Services;

public interface IMediaService
{
    Task<MediaResponse> GetMetadataAsync(string url);
}