namespace OurTube.Application.DTOs.Common;

public record Error(
    string Code,
    int StatusCode,
    string Message);