using System.ComponentModel.DataAnnotations.Schema;
using OurTube.Domain.Interfaces;

namespace OurTube.Domain.Entities;

public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    [NotMapped] public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}