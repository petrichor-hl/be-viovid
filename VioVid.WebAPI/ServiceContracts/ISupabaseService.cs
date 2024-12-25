namespace VioVid.WebAPI.ServiceContracts;

public interface ISupabaseService
{
    Task<string> UploadFileAsync(string filePath, Stream fileStream, string fileName);
}