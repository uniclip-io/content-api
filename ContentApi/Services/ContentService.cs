using ContentApi.Dtos;
using ContentApi.Models;
using ContentApi.Repository;
using MongoDB.Bson;

namespace ContentApi.Services;

public class ContentService
{
    private readonly ContentRepository _contentRepository;
    private readonly RabbitMqService _rabbitMqService;
    private readonly EncryptionService _encryptionService;

    public ContentService(ContentRepository contentRepository, RabbitMqService rabbitMqService, EncryptionService encryptionService)
    {
        _contentRepository = contentRepository;
        _rabbitMqService = rabbitMqService;
        _encryptionService = encryptionService;
    }

    public async Task<ObjectId> UploadFile(string userId, IFormFile file, string type)
    {
        using var output = new MemoryStream();
        _encryptionService.EncryptStream(file.OpenReadStream(), output);

        var contentId = await _contentRepository.AddFile(file.FileName, file.ContentType, new MemoryStream(output.ToArray()));
        var fileContent = new FileContent(userId, contentId.ToString(), type);

        _rabbitMqService.PublishFileUpload(fileContent);
        
        return contentId;
    }

    public async Task<DecryptedFile> DownloadFile(ObjectId contentId)
    {
        var data = await _contentRepository.GetFile(contentId);

        using var output = new MemoryStream();
        _encryptionService.DecryptStream(data, output);
        
        var contentType = data.FileInfo.Metadata["contentType"].AsString;
        var filename = data.FileInfo.Filename;
        return new DecryptedFile(filename, contentType, new MemoryStream(output.ToArray()));
    }
}