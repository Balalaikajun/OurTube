using OurTube.Domain.Entities;

namespace OurTube.Application.Interfaces;

public interface ITagService
{
    Task<Tag> GetOrCreate(string name);
}