using Microsoft.EntityFrameworkCore;
using OurTube.Application.Interfaces;
using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services;

public class TagService
{
    private readonly IApplicationDbContext _dbContext;

    public TagService(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tag> GetOrCreate(string name)
    {
        var newTag = new Tag(name);

        var tag = await _dbContext.Tags.FirstOrDefaultAsync(tag => tag.Name == newTag.Name);

        if (tag == null)
        {
            tag = newTag;
            
            _dbContext.Tags.Add(tag);
            
            await _dbContext.SaveChangesAsync();
            
        }
        
        return tag;
    }
}