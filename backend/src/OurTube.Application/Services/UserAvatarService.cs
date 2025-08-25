using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OurTube.Application.Extensions;
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

    public async Task<Replies.UserAvatar.UserAvatar> CreateOrUpdateUserAvatarAsync(IFormFile image, Guid userId)
    {
        if (image.Length == 0)
            throw new ArgumentException("Avatar cannot be empty");

        Console.WriteLine(image.ContentType);

        if (image.ContentType != "image/png" && image.ContentType != "image/jpeg")
            throw new ArgumentException("Avatar content type is not supported");

        var userAvatar = await _dbContext.UserAvatars.FirstOrDefaultAsync(x => x.UserId == userId);


        if (userAvatar == null)
        {
            userAvatar = new UserAvatar
            {
                UserId = userId,
                FileName = Path.Combine(userId.ToString(), "avatar" + FileExtensions[image.ContentType]).Replace(@"\", @"/"),
                Bucket = _bucket
            };

            _dbContext.UserAvatars.Add(userAvatar);
        }
        else
        {
            await _storageClient.DeleteFileAsync(userAvatar.FileName, userAvatar.Bucket);

            userAvatar.Update();
        }

        await _storageClient.UploadFileAsync(
                image,
                userAvatar.FileName,
                userAvatar.Bucket);

        await _dbContext.SaveChangesAsync();

        return _mapper.Map<Replies.UserAvatar.UserAvatar>(userAvatar);
    }

    public async Task DeleteUserAvatarAsync(Guid userId)
    {
        await _dbContext.ApplicationUsers
            .EnsureExistAsync(userId);

        var userAvatar = await _dbContext.UserAvatars
            .GetAsync(ua => ua.UserId == userId, true);

        userAvatar.Delete();

        await _dbContext.SaveChangesAsync();
    }
}