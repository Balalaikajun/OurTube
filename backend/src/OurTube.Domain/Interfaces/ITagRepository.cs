using OurTube.Domain.Entities;

namespace OurTube.Domain.Interfaces;

public interface ITagRepository:IRepository<Tag>
{
    Task<bool> ContainsAsync(string name);
}