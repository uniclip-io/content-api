namespace ContentApi.Models;

public record DecryptedFile(
    string Filename,
    string ContentType, 
    Stream Stream
);