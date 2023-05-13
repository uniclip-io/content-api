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
    
    public async Task<ObjectId> AddFile(string fileName, string contentType, Stream buffer)
    {
        var options = new GridFSUploadOptions
        {
            Metadata = new BsonDocument
            {
                { "contentType", contentType }
            }
        };
    
        return await _bucket.UploadFromStreamAsync(fileName, buffer, options);
    }
    
    public async Task<GridFSDownloadStream> GetFile(ObjectId objectId)
    { 
        return await _bucket.OpenDownloadStreamAsync(objectId);
    }
}