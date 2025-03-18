using MediatR;

namespace OurTube.Domain.Interfaces;

public interface IDomainEvent:INotification
{
    DateTime OccurredOn { get; } 
}