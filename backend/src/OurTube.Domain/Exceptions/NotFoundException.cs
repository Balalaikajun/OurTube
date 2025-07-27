namespace OurTube.Domain.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
    public NotFoundException(Type type) : this($"{type.Name} was not found") { }

    public NotFoundException(Type type, object id) : this($"{type.Name} with id - {id.ToString()}, was not found") { }
}
