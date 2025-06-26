using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Interfaces;

namespace OurTube.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedVideoDto>> Get(
        [FromQuery] string query = "",
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] bool reload = true)
    {
        var result = await _searchService.SearchVideos(
            query,
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            Request.Cookies["SessionId"],
            limit,
            after,
            reload);
        return Ok(result);
    }
}