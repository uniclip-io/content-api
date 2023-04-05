using ContentApi.Repository;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace ContentApi.Controllers;

[ApiController]
[Route("content")]
public class ContentController : ControllerBase
{
    private readonly ContentRepository _contentRepository;

    public ContentController(ContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }
    
    [HttpPost("/store")]
    public async Task<IActionResult> StoreContent(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Invalid content.");
        }
        var contentId = await _contentRepository.UploadFile(file);

        return Ok(contentId.ToString());
    }
    
    [HttpGet("/fetch/{contentId}")]
    public async Task<IActionResult> FetchContent(string contentId)
    {
        if (!ObjectId.TryParse(contentId, out var objectId))
        {
            return BadRequest("Invalid content ID.");
        }
        var stream = await _contentRepository.GetFile(objectId);
        
        return File(stream, stream.FileInfo.Metadata["contentType"].AsString);
    }
}