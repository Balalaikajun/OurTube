using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Services;

namespace OurTube.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly SearchService _searchService;

    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoMinGetDto>>> Get(
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