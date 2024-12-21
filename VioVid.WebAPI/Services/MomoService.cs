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
    private readonly HttpClient _httpClient = new HttpClient();
    private readonly IPaymentService _paymentService;
    
    public MomoService(ApplicationDbContext dbContext, IPaymentService paymentService)
    {
        _dbContext = dbContext;
        _paymentService = paymentService;
    }
    
    public async Task<string> CreatePaymentUrl(Payment payment)
    {
        var plan = await _dbContext.Plans.FindAsync(payment.PlanId);

        if (plan == null)
        {
            throw new NotFoundException($"Không tìm thấy Plan {payment.PlanId}");
        }
        
        // Đường dẫn API
        const string url = "https://test-payment.momo.vn/v2/gateway/api/create";
        
        const string accessKey = "F8BBA842ECF85";
        const string secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
        
        // Data for rawSignature
        var amount = plan.Price;
        const string extraData = "";
        const string ipnUrl = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b";
        var orderId = payment.Id;
        var orderInfo = plan.Name;
        const string partnerCode = "MOMO";
        const string redirectUrl = "http://192.168.1.8:5416/momo-result";
        var requestId = Guid.NewGuid().ToString();
        const string requestType = "payWithMethod";
        
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
            storeId = "VioVid",
            requestId,
            amount = plan.Price,
            orderId = payment.Id,
            orderInfo = plan.Name,
            redirectUrl,
            ipnUrl,
            requestType,
            extraData,
            lang = "vi",
            signature = GetSignature(rawSignature, secretKey),
            // Bổ sung
            orderExpireTime = 30,
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
            return momoResponse.PayUrl;
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