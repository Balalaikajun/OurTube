namespace OurTube.Application.DTOs.Common;

public class PagedDto<T> where T : class
{
    public IEnumerable<T> Elements { get; set; }
    public int? NextAfter { get; set; }
    public bool HasMore { get; set; }
}