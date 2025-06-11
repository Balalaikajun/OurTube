namespace OurTube.Application.DTOs.Views;

public class PagedHistoryDto
{
    public IEnumerable<ViewGetDto> Views { get; set; }
    public int? NextAfter { get; set; }
}