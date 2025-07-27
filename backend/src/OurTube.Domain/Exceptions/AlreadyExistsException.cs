namespace OurTube.Domain.Exceptions;

public class AlreadyExistsException(string message):Exception(message)
{
    public AlreadyExistsException(Type type) : this($"{type.Name} was already exists") { }

    public AlreadyExistsException(Type type, object id) : this($"{type.Name} with id - {id.ToString()}, was already exists") { }
}