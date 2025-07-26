namespace OurTube.Domain.Exceptions;

public class ForbiddenAccessException(string Message) : Exception(Message)
{
    public ForbiddenAccessException(Type type) : this($"Вы не имеете правд доступа к данному {nameof(type)}")
    {
    }
    
    public ForbiddenAccessException(Type type, Guid id) : this($"Вы не имеете прав доступа к {nameof(type)} c id - {id.ToString()}")
    {
    }
}