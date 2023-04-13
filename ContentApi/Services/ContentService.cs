using ContentApi.Dtos;
using ContentApi.Repository;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;

namespace ContentApi.Services;

public class ContentService
{
    private readonly ContentRepository _contentRepository;
    private readonly RabbitMqService _rabbitMqService;

    public ContentService(ContentRepository contentRepository, RabbitMqService rabbitMqService)
    {
        _contentRepository = contentRepository;
        _rabbitMqService = rabbitMqService;
    }

    public async Task<ObjectId> UploadFile(string userId, IFormFile file, string type)
    {
        var contentId = await _contentRepository.UploadFile(file);
        var fileContent = new FileContent(userId, contentId.ToString(), type);

        _rabbitMqService.PublishFileUpload(fileContent);
        
        return contentId;
    }

    public async Task<GridFSDownloadStream> DownloadFile(ObjectId contentId)
    {
        return await _contentRepository.GetFile(contentId);
    }
}