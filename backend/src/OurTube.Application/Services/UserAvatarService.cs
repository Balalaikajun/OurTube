using System.Drawing;
using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OurTube.Application.DTOs.UserAvatar;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public class UserAvatarService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IBlobService _blobService;
    private readonly IMapper _mapper;
    private readonly string _bucket;

    private static readonly Dictionary<string, string> FileExtensions = new Dictionary<string, string>
    {
        { "image/jpeg", ".jpg" },
        { "image/png", ".png" },
        { "image/gif", ".gif" },
        { "image/webp", ".webp" }
    };

    public UserAvatarService(IApplicationDbContext dbContext, IBlobService blobService, IMapper mapper,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _blobService = blobService;
        _mapper = mapper;
        _bucket = configuration.GetSection("Minio:UserBucket").Get<string>();
    }

    public async Task<UserAvatarDto> CreateOrUpdateUserAvatarAsync(IFormFile image, string userId)
    {
        if (!_dbContext.ApplicationUsers.Any(x => x.Id == userId))
            throw new InvalidOperationException($"User with id {userId} not found");

        if (image.Length == 0)
            throw new ArgumentException($"Avatar cannot be empty");

        Console.WriteLine(image.ContentType);
        if (image.ContentType != "image/png" && image.ContentType != "image/jpeg")
            throw new ArgumentException($"Avatar content type is not supported");

        var userAvatar = await _dbContext.UserAvatars.FindAsync(userId);

   

        if (userAvatar != null)
            await _blobService.DeleteFileAsync(userAvatar.FileName, userAvatar.FileName);
        else
            userAvatar = new UserAvatar()
            {
                UserId = userId,
                FileName = Path.Combine(userId, "avatar"+FileExtensions[image.ContentType]).Replace(@"\", @"/"),
                Bucket = _bucket
            };
        
        await _blobService.UploadFileAsync(
            image,
            userAvatar.FileName,
            userAvatar.Bucket);
        
        _dbContext.UserAvatars.Add(userAvatar);
        
        await _dbContext.SaveChangesAsync();

        return _mapper.Map<UserAvatarDto>(userAvatar);
    }

    public async Task DeleteUserAvatarAsync(string userId)
    {
        if (!_dbContext.ApplicationUsers.Any(x => x.Id == userId))
            throw new InvalidOperationException($"User with id {userId} not found");

        var userAvatar = await _dbContext.UserAvatars.FindAsync(userId);

        if (userAvatar == null)
            throw new InvalidOperationException($"User with id {userId} not found");

        await _blobService.DeleteFileAsync(userAvatar.Bucket, userAvatar.FileName);

        _dbContext.UserAvatars.Remove(userAvatar);

        await _dbContext.SaveChangesAsync();
    }
}