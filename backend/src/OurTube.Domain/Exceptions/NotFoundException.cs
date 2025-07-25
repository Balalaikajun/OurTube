namespace OurTube.Domain.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
    public NotFoundException(Type type) : this($"{nameof(type)} was not found") { }

    public NotFoundException(Type type, object id) : this($"{nameof(type)} with id - {id.ToString()}, was not found") { }
}
