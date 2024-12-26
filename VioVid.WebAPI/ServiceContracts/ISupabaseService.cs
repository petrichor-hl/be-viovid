namespace VioVid.WebAPI.ServiceContracts;

public interface ISupabaseService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName);
}