using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OurTube.Application.Extensions;
using OurTube.Application.Interfaces;
using OurTube.Application.Mapping.Custom;
using OurTube.Application.Replies.Common;
using OurTube.Application.Requests.Comment;
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

    public async Task<Replies.Comment.Comment> CreateAsync(Guid userId, PostCommentRequest postCommentDto)
    {
        var video = await _dbContext.Videos.GetByIdAsync(postCommentDto.VideoId, true);

        var parent = await _dbContext.Comments.FindAsync(postCommentDto.ParentId);

        if (parent != null && parent.ParentId != null)
            throw new InvalidOperationException("Максимальная глубина вложенности комментариев — 2 уровня");

        var comment = new Comment
        {
            ApplicationUserId = userId,
            VideoId = postCommentDto.VideoId,
            Text = postCommentDto.Text,
            Parent = parent
        };

        _dbContext.Comments.Add(comment);
        video.CommentsCount++;

        if (parent != null)
            parent.ChildsCount++;

        await _dbContext.SaveChangesAsync();

        return await GetAsync(comment.Id, userId);
    }

    public async Task UpdateAsync(UpdateCommentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Text))
            return;

        var comment = await _dbContext.Comments.GetByIdAsync(request.Id, true);
        
        comment.Text = request.Text;

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId)
    {
        var comment = await _dbContext.Comments
            .GetByIdAsync(commentId, true);
        
        comment.Delete();

        await _dbContext.SaveChangesAsync();
    }

    public async Task<ListReply<Replies.Comment.Comment>> GetCommentsWithLimitAsync(GetCommentRequest commentRequest)
    {
        var cacheKey = GetCacheKey(commentRequest.SessionId, commentRequest.VideoId, commentRequest.ParentId);

        if (commentRequest.Reload)
            _cache.Remove(cacheKey);

        if (!_cache.TryGetValue(cacheKey, out List<Guid> cachedRecommendations))
        {
            cachedRecommendations = [];

            _cache.Set(cacheKey, cachedRecommendations, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(15)
            });
        }

        if (cachedRecommendations.Count <= commentRequest.After + commentRequest.Limit)
            cachedRecommendations.AddRange(await GetMoreIds(commentRequest));

        var resultIds = cachedRecommendations.Skip(commentRequest.After).Take(commentRequest.Limit).ToList();

        var commentsDict = await _dbContext.Comments
            .Where(c => resultIds.Contains(c.Id))
            .ProjectToDto(_mapper, commentRequest.UserId)
            .ToDictionaryAsync(c => c.Id);

        var comments = resultIds
            .Select(id => commentsDict[id]);

        var nextAfter = commentRequest.After + commentRequest.Limit;
        var hasMore = cachedRecommendations.Count > nextAfter;

        return new ListReply<Replies.Comment.Comment>()
        {
            Elements= comments,
            NextAfter = nextAfter,
            HasMore = hasMore
        };
    }

    private async Task<IEnumerable<Guid>> GetMoreIds(GetCommentRequest commentRequest)
    {
        await _dbContext.ApplicationUsers.EnsureExistAsync(commentRequest.UserId);

        await _dbContext.Videos.EnsureExistAsync(commentRequest.VideoId);

        await _dbContext.Comments.EnsureExistAsync(commentRequest.ParentId);

        _cache.TryGetValue(GetCacheKey(commentRequest.SessionId, commentRequest.VideoId, commentRequest.ParentId), out List<Guid> usedId);

        var result = await _dbContext.Comments
            .Where(c => c.VideoId == commentRequest.VideoId && c.ParentId == commentRequest.ParentId)
            .Where(c => !usedId.Contains(c.Id))
            .OrderByDescending(c => c.LikesCount)
            .Take(commentRequest.Limit)
            .Select(c => c.Id)
            .ToListAsync();

        return result;
    }

    private async Task<Replies.Comment.Comment?> GetAsync(Guid commentId, Guid userId)
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