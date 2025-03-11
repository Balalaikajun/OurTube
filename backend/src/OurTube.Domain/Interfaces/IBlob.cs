namespace OurTube.Domain.Interfaces
{
    public interface IBlob
    {
        string FileName { get; set; }
        string Bucket { get; set; }
    }
}
