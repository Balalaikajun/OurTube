using Microsoft.EntityFrameworkCore;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;

namespace OurTube.Application.Services;

///<inheritdoc cref="IAccessChecker"/> 
public class AccessChacker : IAccessChecker
{
    private readonly IApplicationDbContext _context;

    private static readonly Dictionary<string, Func<IApplicationDbContext, Guid, Guid, Task<bool>>> Rules = new()
    {
        [nameof(ApplicationUser)] = (ctx, id, userId) =>
            Task.FromResult(id == userId),
        [nameof(Video)] = async (ctx, id, userId) =>
            await ctx.Videos.AnyAsync(v => v.Id == id && v.ApplicationUserId == userId),
        [nameof(Comment)] = async (ctx, id, userId) =>
            await ctx.Comments.AnyAsync(c => c.Id == id && c.ApplicationUserId == userId),
        [nameof(Playlist)] = async (ctx, id, userId) =>
            await ctx.Playlists.AnyAsync(p => p.Id == id && p.ApplicationUserId == userId),
    };

    public AccessChacker(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanEditAsync(Type type,Guid userId, Guid entityId)
    {
        Rules.TryGetValue(type.Name, out var rule);
        
        if (rule == null)
            throw new InvalidOperationException($"Для типа {type.Name} не зарегистрировано правило доступа");
        
        return await rule(_context, userId, entityId);
    }
}