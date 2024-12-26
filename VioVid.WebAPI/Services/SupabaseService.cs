using Supabase;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class SupabaseService : ISupabaseService
{
    private readonly string _bucketName;
    private readonly IConfiguration _configuration;
    private readonly string _supabaseUrl;
    private readonly Client _supabaseClient;

    public SupabaseService(IConfiguration configuration)
    {
        _configuration = configuration;
        _supabaseUrl = _configuration["Supabase:Url"];
        var _apiKey = _configuration["Supabase:ApiKey"];
        _bucketName = _configuration["Supabase:BucketName"];

        // Initialize Supabase Client with options
        var options = new SupabaseOptions
        {
            AutoConnectRealtime = true
        };
        _supabaseClient = new Client(_supabaseUrl, _apiKey, options);
    }

    // Upload a single file to Supabase storage using byte[]
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        try
        {
            // Convert stream to byte[]
            byte[] fileBytes;
            using (var memoryStream = new MemoryStream())
            {
                await fileStream.CopyToAsync(memoryStream);
                fileBytes = memoryStream.ToArray();
            }

            // Initialize Supabase Storage service
            var storage = _supabaseClient.Storage.From(_bucketName);

            // Use Supabase Storage to upload file (uploading byte array)
            var filePath = "uploads/" + fileName; // Customize the folder path

            // File upload with byte[] content
            var response = await storage.Upload(fileBytes, filePath);

            // // Check the upload response for errors
            // if (response == null || !response.)
            //     throw new Exception($"File upload failed: {response?.Error?.Message}");

            // Return the public URL of the uploaded file
            return $"{_supabaseUrl}/storage/v1/object/public/{_bucketName}/{filePath}";
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return ex.Message; // You can handle error logging as needed
        }
    }

    // Initialize Supabase client asynchronously
    public async Task InitializeAsync()
    {
        await _supabaseClient.InitializeAsync();
    }

    // Upload multiple files
    public async Task<List<string>> UploadFilesAsync(ICollection<IFormFile>? files)
    {
        var fileUrls = new List<string>();

        if (files != null && files.Any())
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file.FileName);

                // Use a memory stream to handle file content
                using (var fileStream = new MemoryStream())
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Position = 0; // Reset stream position to the beginning

                    // Upload file and get the URL
                    var fileUrl = await UploadFileAsync(fileStream, fileName);
                    fileUrls.Add(fileUrl);
                }
            }

        return fileUrls;
    }
}