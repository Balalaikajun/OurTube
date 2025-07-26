using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OurTube.Application.Interfaces;
using OurTube.Application.Replies.Common;
using OurTube.Application.Replies.Video;
using OurTube.Application.Requests.Video;

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
    public async Task<ActionResult<ListReply<MinVideo>>> Get(
        [FromQuery] string query = "",
        [FromQuery] int limit = 10,
        [FromQuery] int after = 0,
        [FromQuery] bool reload = true)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var sessionId = Guid.Parse(Request.Cookies["SessionId"]);
        
        var result = await _searchService.SearchVideos(new SearchRequest()
        {
            SearchQuery = query,
            UserId = userId,
            SessionId = sessionId,
            Limit = limit,
            After = after,
            Reload = reload
        });
        
        return Ok(result);
    }
}