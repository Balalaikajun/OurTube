using System.ComponentModel.DataAnnotations.Schema;
using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Entities;

public abstract class Base
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Base()
    {
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    [NotMapped] public bool IsEdited => UpdatedDate != CreatedDate;

    public DateTime? DeletedDate { get; set; }
    public bool IsDeleted { get; set; } = false;

    [NotMapped] public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public virtual void Update()
    {
        UpdatedDate = DateTime.UtcNow;
    }
    
    public virtual void Delete()
    {
        DeletedDate = DateTime.UtcNow;
        IsDeleted = true;
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}