namespace ContentApi.Dtos;

public record UploadFile(
    string UserId,
    IFormFile File,
    string Type
);