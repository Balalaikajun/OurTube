using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OurTube.Application.DTOs.Comment;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

public class CommentService : ICommentCrudService, ICommentRecommendationService
{
    private const int CommentPull = 15;
    private readonly IMemoryCache _cache;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public CommentService(IApplicationDbContext dbContext, IMemoryCache cache, IMapper mapper)
    {
        _dbContext = dbContext;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task<CommentGetDto> CreateAsync(Guid userId, CommentPostDto postDto)
    {
        var video = await _dbContext.Videos.GetByIdAsync(postDto.VideoId, true);

        var parent = await _dbContext.Comments.FindAsync(postDto.ParentId);

        if (parent != null && parent.ParentId != null)
            throw new InvalidOperationException("Максимальная глубина вложенности комментариев — 2 уровня");

        var comment = new Comment
        {
            ApplicationUserId = userId,
            VideoId = postDto.VideoId,
            Text = postDto.Text,
            Parent = parent
        };

        _dbContext.Comments.Add(comment);
        video.CommentsCount++;

        if (parent != null)
            parent.ChildsCount++;

        await _dbContext.SaveChangesAsync();

        return await GetAsync(comment.Id, userId);
    }

    public async Task UpdateAsync(CommentPatchDto postDto)
    {
        if (string.IsNullOrWhiteSpace(postDto.Text))
            return;

        var comment = await _dbContext.Comments.GetByIdAsync(postDto.Id, true);
        
        comment.Text = postDto.Text;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId)
    {
        var comment = await _dbContext.Comments
            .GetByIdAsync(commentId, true);
        
        comment.Delete();

        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedCommentDto> GetCommentsWithLimitAsync(GetCommentsRequest request)
    {
        var cacheKey = GetCacheKey(request.SessionId, request.VideoId, request.ParentId);

        if (request.Reload)
            _cache.Remove(cacheKey);

        if (!_cache.TryGetValue(cacheKey, out List<Guid> cachedRecommendations))
        {
            cachedRecommendations = [];

            _cache.Set(cacheKey, cachedRecommendations, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }

        if (cachedRecommendations.Count <= request.After + request.Limit)
            cachedRecommendations.AddRange(await GetMoreIds(request));

        var resultIds = cachedRecommendations.Skip(request.After).Take(request.Limit).ToList();

        var commentsDict = await _dbContext.Comments
            .Where(c => resultIds.Contains(c.Id))
            .ProjectToDto(_mapper, request.UserId)
            .ToDictionaryAsync(c => c.Id);

        var comments = resultIds
            .Select(id => commentsDict[id]);

        var nextAfter = request.After + request.Limit;
        var hasMore = cachedRecommendations.Count > nextAfter;

        return new PagedCommentDto
        {
            Comments = comments,
            NextAfter = nextAfter,
            HasMore = hasMore
        };
    }

    private async Task<IEnumerable<Guid>> GetMoreIds(GetCommentsRequest request)
    {
        await _dbContext.ApplicationUsers.EnsureExistAsync(request.UserId);

        await _dbContext.Videos.EnsureExistAsync(request.VideoId);

        await _dbContext.Comments.EnsureExistAsync(request.ParentId);

        _cache.TryGetValue(GetCacheKey(request.SessionId, request.VideoId, request.ParentId), out List<Guid> usedId);

        var result = await _dbContext.Comments
            .Where(c => c.VideoId == request.VideoId && c.ParentId == request.ParentId)
            .Where(c => !usedId.Contains(c.Id))
            .OrderByDescending(c => c.LikesCount)
            .Take(request.Limit)
            .Select(c => c.Id)
            .ToListAsync();

        return result;
    }

    private async Task<CommentGetDto?> GetAsync(Guid commentId, Guid userId)
    {
        return await _dbContext.Comments
            .Where(c => c.Id == commentId)
            .ProjectToDto(_mapper, userId)
            .FirstOrDefaultAsync();
    }

    private static string GetCacheKey(Guid sessionId, Guid videoId, Guid? parentId)
    {
        return $"Comments_{sessionId}_{videoId}_{parentId}";
    }
}