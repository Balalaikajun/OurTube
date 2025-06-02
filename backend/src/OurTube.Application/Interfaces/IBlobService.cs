namespace OurTube.Application.Interfaces;

public interface IBlobService
{
    Task UploadFiles(string[] inputFiles, string bucket, string prefix);
    Task UploadFile(string input, string objectName, string bucket);
}