using System.Security.Claims;
using System.Text.Json;
using Application.DTOs.Payment;
using Application.Exceptions;
using VioVid.Core.Entities;
using VioVid.Core.Enum;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPushNotificationService _pushNotificationService;

    public PaymentService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor, IPushNotificationService pushNotificationService)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _pushNotificationService = pushNotificationService;
    }

    public async Task<Payment> CreatePayment(CreatePaymentRequest createPaymentRequest)
    {
        var user = _httpContextAccessor.HttpContext?.User!;
        var userId = user.FindFirstValue("UserId");
        var applicationUserId = Guid.Parse(userId);
        
        var plan = await _dbContext.Plans.FindAsync(createPaymentRequest.PlanId);

        if (plan == null)
        {
            throw new NotFoundException($"Plan {createPaymentRequest.PlanId} không tồn tại");
        }
        
        var newPayment = new Payment
        {
            ApplicationUserId = applicationUserId,
            PlanId = createPaymentRequest.PlanId,
            PlanName = plan.Name,
            CreatedAt = DateTime.UtcNow,
            IsDone = false
        };
        await _dbContext.Payments.AddAsync(newPayment);
        await _dbContext.SaveChangesAsync();

        return newPayment;
    }

    public async Task UpdatePaymentAndPushNoti(Payment payment, int amount, bool isSuccess)
    {
        if (isSuccess)
        {
            // Thanh toán thành công
            payment.IsDone = true;
            payment.StartDate = DateOnly.FromDateTime(DateTime.UtcNow); 
            payment.EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(payment.Plan.Duration));
            payment.Amount = amount;
            
            await _dbContext.SaveChangesAsync();
        }

        if (payment.ApplicationUser.FcmToken == null)
        {
            return;
        }
        
        var dataPayload = new Dictionary<string, string>
        {
            { "category", ((int)NotificationCategory.Payment).ToString() },
        };
        
        if (isSuccess)
        {
            await _pushNotificationService.PushNotificationToIndividualDevice(
                "Thanh toán thành công!", 
                "Cảm ơn bạn đã sử dụng dịch vụ",
                dataPayload,
                payment.ApplicationUser.FcmToken
            );
        }
        else
        {
            await _pushNotificationService.PushNotificationToIndividualDevice(
                "Thanh toán không thành công!", 
                "Xin vui lòng kiểm tra lại sự cố",
                dataPayload,
                payment.ApplicationUser.FcmToken
            );
        }
    }
}