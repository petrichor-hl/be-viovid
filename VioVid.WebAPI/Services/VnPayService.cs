using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Entities;
using VioVid.Core.Services;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

public class VnPayService : IVnPayService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;

    public VnPayService(IConfiguration configuration, ApplicationDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public async Task<string> CreatePaymentUrl(Payment payment, HttpContext context)
    {
        
        Console.WriteLine("Payment: " + JsonSerializer.Serialize(payment));
        // Get payment info
        var plan = await _dbContext.Plans.FindAsync(payment.PlanId);

        // Access VNPay parameters from configuration
        var vnpUrl = _configuration["Vnpay:BaseUrl"];
        var vnpTmnCode = _configuration["Vnpay:TmnCode"];
        var vnpReturnUrl = _configuration["Vnpay:ReturnUrl"];
        var vnpHashSecret = _configuration["Vnpay:HashSecret"];
        var vnpCommand = _configuration["Vnpay:Command"];
        var vnpCurrCode = _configuration["Vnpay:CurrCode"];
        var vnpVersion = _configuration["Vnpay:Version"];
        var vnpLocale = _configuration["Vnpay:Locale"];


        Console.WriteLine($"Plan details: {JsonSerializer.Serialize(plan)}");
        Console.WriteLine($"Payment Request details: {JsonSerializer.Serialize(payment)}");
        // Replace parameters with Plan data
        var amount = plan?.Price;
        var orderType = plan?.Name;
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"; // Extract IP address
        var createDate = DateTime.Now;
        var expireDate = createDate.AddDays(double.Parse((plan?.Duration ?? 0).ToString()));

        // Create instance of VnPayLib to handle request URL creation
        var payLib = new VnPayLib();

        // Add parameters to VnPayLib
        payLib.AddRequestData("vnp_Version", vnpVersion);
        payLib.AddRequestData("vnp_Command", vnpCommand);
        payLib.AddRequestData("vnp_TmnCode", vnpTmnCode);
        payLib.AddRequestData("vnp_Amount", (amount * 100).ToString()); // Multiply by 100 to match VNPay amount format
        payLib.AddRequestData("vnp_CreateDate", createDate.ToString("yyyyMMddHHmmss"));
        payLib.AddRequestData("vnp_CurrCode", vnpCurrCode);
        payLib.AddRequestData("vnp_IpAddr", ipAddress);
        payLib.AddRequestData("vnp_Locale", vnpLocale);
        payLib.AddRequestData("vnp_OrderInfo", HttpUtility.UrlEncode($"Thanh toan goi dich vu {orderType}"));
        payLib.AddRequestData("vnp_OrderType", orderType!);
        payLib.AddRequestData("vnp_ReturnUrl", vnpReturnUrl);
        payLib.AddRequestData("vnp_ExpireDate", expireDate.ToString("yyyyMMddHHmmss"));
        payLib.AddRequestData("vnp_TxnRef",
            $"{payment.Id}");

        // Generate the payment URL using VnPayLib
        return payLib.CreateRequestUrl(vnpUrl, vnpHashSecret);
    }

    public async Task<bool> VerifyPayment(Dictionary<string, string> vnpParams)
    {
        try
        {
            // Extract and remove the secure hash from the parameters
            var vnpSecureHash = vnpParams["vnp_SecureHash"];
            vnpParams.Remove("vnp_SecureHash"); // Remove the hash so it is not part of the raw data

            vnpParams.Remove("vnp_SecureHashType");

            var data = new StringBuilder();
            foreach (var kv in vnpParams.OrderBy(k => k.Key))
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    if (data.Length > 0) data.Append("&"); // Add '&' between parameters
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value));
                }

            // Final raw query string without the secure hash
            var queryString = data.ToString();

            // Log raw data for debugging
            Console.WriteLine("Raw Data: " + queryString);

            // Compute the HMAC SHA512 hash using the secret key and the raw data
            var computedHash = VnPayLib.HmacSHA512(_configuration["Vnpay:HashSecret"], queryString);
            
            // Compare the computed hash with the received vnp_SecureHash
            if (computedHash != vnpSecureHash) return false; // Signature mismatch

            // Check the response code (00 indicates success)
            var responseCode = vnpParams["vnp_ResponseCode"];

            if (responseCode != "00") return false;

            Payment? payment = null;

            // Try to parse the value from vnpParams["vnp_TxnRef"] into a Guid
            if (Guid.TryParse(vnpParams["vnp_TxnRef"], out Guid parsedPaymentId))
            {
                Console.WriteLine($"Payment Id successfully parsed: {parsedPaymentId}");
    
                // Now you can use parsedPaymentId to query the database or further logic
                 payment = await _dbContext.Payments.FindAsync(parsedPaymentId);
                if (payment == null)
                {
                    Console.WriteLine("No payment record found with the given PaymentId.");
                    return false;
                }
                Console.WriteLine($"Payment: {payment}");
            }
            else
            {
                Console.WriteLine("Invalid PaymentId format.");
                return false;
            }


            Console.WriteLine($"Payment : {payment}");
            
            //Update payment status
            payment!.IsDone = true;

            var applicationUserId = payment!.ApplicationUserId;
            var planId = payment!.PlanId;

            Console.WriteLine("applicationUserId: " + applicationUserId);
            Console.WriteLine("planId: " + planId);

            //Update the UserPlan 
            var plan = await _dbContext.Plans.FindAsync(planId);

            Console.WriteLine("plan: " + plan!);

            var existingUserPlan = await _dbContext.UserPlans
                .FirstOrDefaultAsync(up =>
                    up.ApplicationUserId == applicationUserId && up.PlanId == planId);
            Console.WriteLine("existingUserPlan: " + existingUserPlan);
            if (existingUserPlan != null)
            {
                // Update the existing record
                existingUserPlan.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
                if (plan != null)
                    existingUserPlan.EndDate =
                        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(plan.Duration));
                _dbContext.UserPlans.Update(existingUserPlan);
            }
            else
            {
                Console.WriteLine("creating new UserPlan...");
                if (applicationUserId != Guid.Empty && planId != Guid.Empty)                {
                    var newUserPlan = new UserPlan
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = applicationUserId,
                        PlanId = planId,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(plan!.Duration))
                    };

                    Console.WriteLine("newUserPlan: " + newUserPlan);

                    await _dbContext.UserPlans.AddAsync(newUserPlan);
                }
                else
                {
                    throw new InvalidOperationException("Invalid GUID values for ApplicationUserId or PlanId.");
                }
            }

            // Increment the Order property by 1
            plan!.Order += 1;
            // Update the Plan in the context
            _dbContext.Plans.Update(plan);


            // Save changes to the database
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidModelException(ex.Message);
        }

        return false;
    }
}