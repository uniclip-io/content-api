using ContentApi.Dtos;
using ContentApi.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ContentApi.Controllers;

[ApiController]
[Route("content")]
public class ContentController : ControllerBase
{
    private readonly ContentService _contentService;

    public ContentController(ContentService contentService)
    {
        _contentService = contentService;
    }

    [HttpPost("/store")]
    public async Task<IActionResult> StoreContent([FromForm] UploadFile uploadFile)
    {
        var userId = uploadFile.UserId;
        var file = uploadFile.File;
        var type = uploadFile.Type;
        
        if (type == null || (type != "file" && type != "folder" && type != "diverse"))
        {
            return BadRequest("Invalid `type`.");
        }
        if (file == null || file.Length == 0)
        {
            return BadRequest("Invalid content.");
        }

        var contentId = await _contentService.UploadFile(userId, file, type);

        return Ok(contentId.ToString());
    }

    [HttpGet("/fetch/{contentId}")]
    public async Task<IActionResult> FetchContent(string contentId)
    {
        if (!ObjectId.TryParse(contentId, out var objectId))
        {
            return BadRequest("Invalid content ID.");
        }

        var file = await _contentService.DownloadFile(objectId);
        
        return File(file.Stream, file.ContentType, file.Filename);
    }
}