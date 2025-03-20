using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.DTOs.Video;
using OurTube.Application.Services;

namespace OurTube.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController: ControllerBase
{
    private readonly SearchService _searchService;

    public SearchController(SearchService searchService)
    {
        _searchService = searchService;
        
        
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VideoMinGetDto>>> Get([FromQuery] string query)
    {
        var result = await _searchService.SearchVideos(query);
        return Ok(result); // Явное указание кода 200
    }
}