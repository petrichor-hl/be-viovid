using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.DTOs;
using Application.DTOs.Payment;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;
using ApplicationException = Application.Exceptions.ApplicationException;

namespace VioVid.WebAPI.Services;

public class MomoService : IMomoService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IPaymentService _paymentService;
    private readonly IConfiguration _configuration;
    
    private readonly HttpClient _httpClient = new HttpClient();

    
    public MomoService(ApplicationDbContext dbContext, IPaymentService paymentService, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _paymentService = paymentService;
        _configuration = configuration;
    }
    
    public async Task<string> CreatePaymentUrl(Payment payment)
    {
        var plan = await _dbContext.Plans.FindAsync(payment.PlanId);

        if (plan == null)
        {
            throw new NotFoundException($"Không tìm thấy Plan {payment.PlanId}");
        }
        
        var url = _configuration["Momo:BaseUrl"];
        
        var accessKey = _configuration["Momo:AccessKey"];
        var secretKey = _configuration["Momo:SecretKey"];
        
        // Data for rawSignature
        var amount = plan.Price;
        var extraData = _configuration["Momo:ExtraData"];
        var ipnUrl = _configuration["Momo:IpnUrl"];
        var orderId = payment.Id;
        var orderInfo = plan.Name;
        var partnerCode = _configuration["Momo:PartnerCode"];
        var redirectUrl = _configuration["Momo:RedirectUrl"];
        var requestId = Guid.NewGuid().ToString();
        var requestType = _configuration["Momo:RequestType"];
        
        var rawSignature = "accessKey="+accessKey
                                   +"&amount="+amount
                                   +"&extraData="+extraData
                                   +"&ipnUrl="+ipnUrl
                                   +"&orderId="+orderId
                                   +"&orderInfo="+orderInfo
                                   +"&partnerCode="+partnerCode
                                   +"&redirectUrl="+redirectUrl
                                   +"&requestId="+requestId
                                   +"&requestType="+requestType;
        
        // Payload
        var payload = new
        {
            partnerCode,
            storeId = _configuration["Momo:StoreId"],
            requestId,
            amount = plan.Price,
            orderId = payment.Id,
            orderInfo = plan.Name,
            redirectUrl,
            ipnUrl,
            requestType,
            extraData,
            lang = _configuration["Momo:Lang"],
            signature = GetSignature(rawSignature, secretKey),
        };
        
        // Chuyển payload thành JSON
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Gửi yêu cầu POST
        var response = await _httpClient.PostAsync(url, content);

        // Kiểm tra trạng thái phản hồi
        if (response.IsSuccessStatusCode)
        {
            // Đọc kết quả phản hồi
            var responseContent = await response.Content.ReadAsStringAsync();
            var momoResponse = JsonSerializer.Deserialize<MomoResponse>(responseContent);
            Console.WriteLine(responseContent);
            return momoResponse!.PayUrl;
        }
        
        Console.WriteLine(response.StatusCode);
        Console.WriteLine(response.Content);

        throw new ApplicationException(
            ApiResultErrorCodes.InternalServerError,
            "Có lỗi xảy ra khi sinh PaymentUrl"
        );
    }

    public async Task<bool> HandleRecord(MomoCallbackRequest momoCallbackRequest)
    {
        var payment = await _dbContext.Payments
            .Include(p => p.ApplicationUser)
            .Include(p => p.Plan)
            .FirstOrDefaultAsync(p => p.Id == momoCallbackRequest.PaymentId);
        
        if (payment == null)
        {
            throw new NotFoundException($"Không tìm thấy giao dịch {momoCallbackRequest.PaymentId}");
        }

        var isSuccess = momoCallbackRequest.ResultCode == 0;
        
        await _paymentService.UpdatePaymentAndPushNoti(payment, momoCallbackRequest.Amount, isSuccess);

        return isSuccess;
    }

    private string GetSignature(String text, String key)
    {
        // change according to your needs, an UTF8Encoding
        // could be more suitable in certain situations
        ASCIIEncoding encoding = new ASCIIEncoding();

        Byte[] textBytes = encoding.GetBytes(text);
        Byte[] keyBytes = encoding.GetBytes(key);

        Byte[] hashBytes;

        using (HMACSHA256 hash = new HMACSHA256(keyBytes))
            hashBytes = hash.ComputeHash(textBytes);

        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
    
    public class MomoResponse
    {
        [JsonPropertyName("partnerCode")]
        public string PartnerCode { get; set; }

        [JsonPropertyName("orderId")]
        public string OrderId { get; set; }

        [JsonPropertyName("requestId")]
        public string RequestId { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("responseTime")]
        public long ResponseTime { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("resultCode")]
        public int ResultCode { get; set; }

        [JsonPropertyName("payUrl")]
        public string PayUrl { get; set; }

        [JsonPropertyName("shortLink")]
        public string ShortLink { get; set; }
    }
}