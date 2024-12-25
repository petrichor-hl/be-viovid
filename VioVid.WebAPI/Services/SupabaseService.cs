using System.Net.Http.Headers;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class SupabaseService : ISupabaseService
{
    private readonly string _bucketName;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly string _supabaseUrl;

    public SupabaseService(HttpClient httpClient, IConfiguration configuration, ISupabaseService supabaseService)
    {
        _configuration = configuration;

        // Set Supabase API key
        _httpClient = httpClient;
        _supabaseUrl = configuration["Supabase:Url"];
        var _apiKey = _configuration["Supabase:ApiKey"];
        _bucketName = _configuration["Supabase:BucketName"];
        _httpClient.DefaultRequestHeaders.Add("apikey", _apiKey);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> UploadFileAsync(string filePath, Stream fileStream, string fileName)
    {
        // Construct the endpoint
        var url = $"{_supabaseUrl}/storage/v1/object/{_bucketName}/{filePath}/{fileName}";

        // Prepare the file content
        using var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        // Make the POST request
        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        // Return the file's public URL or the full response if needed
        return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{filePath}/{fileName}";
    }
}