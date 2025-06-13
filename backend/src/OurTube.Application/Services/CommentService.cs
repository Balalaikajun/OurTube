using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OurTube.Application.DTOs.Comment;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public class CommentService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly IMapper _mapper;
    
    private const int CommentPull = 15;

    public CommentService(IApplicationDbContext dbContext, IMemoryCache cache,IMapper mapper)
    {
        _dbContext = dbContext;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task<CommentGetDto> CreateAsync(string userId, CommentPostDto postDto)
    {
        var video = await _dbContext.Videos.FindAsync(postDto.VideoId);

        if (video == null)
            throw new InvalidOperationException("Видео не найдено");

        var parent = await _dbContext.Comments.FindAsync(postDto.ParentId);

        if (parent != null && parent.ParentId != null)
            throw new InvalidOperationException("Максимальная глубина вложенности комментариев — 2 уровня");

        var comment = new Comment
        {
            ApplicationUserId = userId,
            VideoId = postDto.VideoId,
            Text = postDto.Text,
            Parent = parent,
            Created = DateTime.UtcNow
        };

        _dbContext.Comments.Add(comment);
        video.CommentsCount++;
        if (parent != null)
            parent.ChildsCount++;


        await _dbContext.SaveChangesAsync();

        return await GetAsync(comment.Id, userId);
    }

    public async Task UpdateAsync(string userId, CommentPatchDto postDto)
    {
        var comment = await _dbContext.Comments
            .FindAsync(postDto.Id);

        if (comment == null)
            throw new InvalidOperationException("Комментарий не найден");

        if (comment.ApplicationUserId != userId)
            throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

        if (comment.IsDeleted)
            throw new InvalidOperationException("Комментарий удалён");

        if (postDto.Text != "")
        {
            comment.Text = postDto.Text;
            comment.IsEdited = true;
            comment.Updated = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int commentId, string userId)
    {
        var comment = await _dbContext.Comments
            .FindAsync(commentId);

        if (comment == null)
            throw new InvalidOperationException("Комментарий не найден");

        var video = await _dbContext.Videos.FindAsync(comment.VideoId);

        if (video == null)
            throw new InvalidOperationException("Видео не найдено");

        if (comment.ApplicationUserId != userId)
            throw new UnauthorizedAccessException("Вы не имеете доступа к редактированию данного комментария");

        comment.IsDeleted = true;
        comment.Deleted = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<PagedCommentDto> GetCommentsWithLimitAsync(int videoId, int limit, int after,
        string sessionId,
        string? userId,
        int? parentId = null,
        bool reload = false)
    {
        if (!string.IsNullOrEmpty(userId) && !await _dbContext.ApplicationUsers.AnyAsync(x => x.Id == userId))
            throw new InvalidOperationException("Пользователь не найдет");
        
        if (!await _dbContext.Videos.AnyAsync(v => v.Id == videoId))
            throw new InvalidOperationException("Видео не найдено");

        if (parentId != null && !await _dbContext.Comments.AnyAsync(p => p.Id == parentId))
            throw new InvalidOperationException("Комментарий не найден");
        
        var cacheKey = GetCacheKey(sessionId, videoId, parentId);
        
        if (reload)
            _cache.Remove(cacheKey);

        if (!_cache.TryGetValue(cacheKey, out List<int> cachedRecommendations))
        {
            cachedRecommendations = [];

            _cache.Set(cacheKey, cachedRecommendations, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }
        
        if (cachedRecommendations.Count < after + limit)
        {
            cachedRecommendations.AddRange(await GetMoreIds(videoId, CommentPull, sessionId, userId, parentId));
        }
        
        var resultIds = cachedRecommendations.Skip(after).Take(limit).ToList();

        var comments = await _dbContext.Comments
            .Where(c => resultIds.Contains(c.Id))
            .ProjectToDto(_mapper, userId)
            .ToListAsync();
        
        var hasMore = cachedRecommendations.Count > after + limit;

        return new PagedCommentDto
        {
            Comments = comments,
            NextAfter = limit+after,
            HasMore = hasMore
        };
    }

    private async Task<IEnumerable<int>> GetMoreIds(int videoId, int limit,
        string sessionId,
        string? userId,
        int? parentId = null)
    {
        if (!await _dbContext.Videos.AnyAsync(v => v.Id == videoId))
            throw new InvalidOperationException("Видео не найдено");

        if (parentId != null && !await _dbContext.Comments.AnyAsync(p => p.Id == parentId))
            throw new InvalidOperationException("Комментарий не найден");

        _cache.TryGetValue(GetCacheKey(sessionId, videoId, parentId), out List<int> usedId);
        
        var result = await _dbContext.Comments
            .Where(c => c.VideoId == videoId && c.ParentId == parentId)
            .Where(c => !usedId.Contains(c.Id))
            .OrderByDescending(c => c.LikesCount)
            .Take(limit)
            .Select(c => c.Id)
            .ToListAsync();

        return result;
    }
    
    private async Task<CommentGetDto?> GetAsync(int commentId, string userId)
    {
        return await _dbContext.Comments
            .Where(c => c.Id == commentId)
            .ProjectToDto(_mapper, userId)
            .FirstOrDefaultAsync();
    }
    
    private static string GetCacheKey(string sessionId, int videoId, int? parentId)
        => $"Comments_{sessionId}_{videoId}_{parentId}";
}