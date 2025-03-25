using OurTube.Domain.Entities;
using OurTube.Domain.Interfaces;

namespace OurTube.Application.Services;

public class TagService
{
    private readonly IUnitOfWork _unitOfWork;

    public TagService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Tag> GetOrCreate(string name)
    {
        var newTag = new Tag(name);
        
        var tag = (await _unitOfWork.Tags.FindAsync(tag => tag.Name == newTag.Name)).FirstOrDefault();

        if (tag == null)
        {
            tag = newTag;
            
            _unitOfWork.Tags.Add(tag);
            
            await _unitOfWork.SaveChangesAsync();
            
        }
        
        return tag;
    }
}