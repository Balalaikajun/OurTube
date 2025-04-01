using Microsoft.EntityFrameworkCore;
using OurTube.Domain.Interfaces;
using OurTube.Domain.Entities;
using OurTube.Infrastructure.Data;

namespace OurTube.Infrastructure.Persistence.Repositories;

public class TagRepository:Repository<Tag>,ITagRepository
{
    public ApplicationDbContext ApplicationDbContext
    {
        get { return Context as ApplicationDbContext; }
    }

    public TagRepository(ApplicationDbContext context)
        : base(context) { }
    
    public async Task<bool> ContainsAsync(string name)
    {
        return await ApplicationDbContext.Tags.AnyAsync(t => t.Name == name);
    }
}