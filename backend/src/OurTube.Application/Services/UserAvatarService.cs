using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using OurTube.Application.DTOs.UserAvatar;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public class UserAvatarService : IUserAvatarService
{
    private static readonly Dictionary<string, string> FileExtensions = new()
    {
        { "image/jpeg", ".jpg" },
        { "image/png", ".png" },
        { "image/gif", ".gif" },
        { "image/webp", ".webp" }
    };

    private readonly IStorageClient _storageClient;
    private readonly string _bucket;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public UserAvatarService(IApplicationDbContext dbContext, IStorageClient storageClient, IMapper mapper,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _storageClient = storageClient;
        _mapper = mapper;
        _bucket = configuration.GetSection("Minio:UserBucket").Get<string>();
    }

    public async Task<UserAvatarDto> CreateOrUpdateUserAvatarAsync(IFormFile image, Guid userId)
    {
        if (!_dbContext.ApplicationUsers.Any(x => x.Id == userId))
            throw new InvalidOperationException($"User with id {userId} not found");

        if (image.Length == 0)
            throw new ArgumentException("Avatar cannot be empty");

        Console.WriteLine(image.ContentType);
        if (image.ContentType != "image/png" && image.ContentType != "image/jpeg")
            throw new ArgumentException("Avatar content type is not supported");

        var userAvatar = await _dbContext.UserAvatars.FindAsync(userId);


        if (userAvatar != null)
            await _storageClient.DeleteFileAsync(userAvatar.FileName, userAvatar.FileName);
        else
            userAvatar = new UserAvatar
            {
                UserId = userId,
                FileName = Path.Combine(userId.ToString(), "avatar" + FileExtensions[image.ContentType]).Replace(@"\", @"/"),
                Bucket = _bucket
            };

        await _storageClient.UploadFileAsync(
            image,
            userAvatar.FileName,
            userAvatar.Bucket);

        _dbContext.UserAvatars.Add(userAvatar);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<UserAvatarDto>(userAvatar);
    }

    public async Task DeleteUserAvatarAsync(Guid userId)
    {
        if (!_dbContext.ApplicationUsers.Any(x => x.Id == userId))
            throw new InvalidOperationException($"User with id {userId} not found");

        var userAvatar = await _dbContext.UserAvatars.FindAsync(userId);

        if (userAvatar == null)
            throw new InvalidOperationException($"User with id {userId} not found");

        await _storageClient.DeleteFileAsync(userAvatar.Bucket, userAvatar.FileName);

        userAvatar.Delete();

        await _dbContext.SaveChangesAsync();
    }
}