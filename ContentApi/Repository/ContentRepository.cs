using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace ContentApi.Repository;

public class ContentRepository
{
    private readonly GridFSBucket _bucket;
    
    public ContentRepository(string connectionString)
    {
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase("content-api");
        _bucket = new GridFSBucket(database);
    }
    
    public async Task<ObjectId> UploadFile(IFormFile file)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
            {
                { "contentType", file.ContentType }
            }
        };
    
        return await _bucket.UploadFromStreamAsync(file.FileName, file.OpenReadStream(), options);
    }
    
    public async Task<GridFSDownloadStream> GetFile(ObjectId objectId)
    { 
        return await _bucket.OpenDownloadStreamAsync(objectId);
    }
}