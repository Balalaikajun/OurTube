using Microsoft.AspNetCore.Http;

namespace OurTube.Application.Interfaces;

public interface IStorageClient
{
    Task UploadFilesAsync(string[] inputFiles, string bucket, string prefix);
    Task UploadFileAsync(string input, string objectName, string bucketName);
    Task UploadFileAsync(IFormFile input, string objectName, string bucketName);
    Task DeleteFileAsync(string objectName, string bucket);
}