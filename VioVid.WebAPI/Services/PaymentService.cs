using System.Security.Claims;
using System.Text.Json;
using Application.DTOs.Payment;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class PaymentService : IPaymentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PaymentService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Payment> CreatePayment(CreatePaymentRequest createPaymentRequest)
    {
        var user = _httpContextAccessor.HttpContext?.User!;
        var userId = user.FindFirstValue("UserId");
        var applicationUserId = Guid.Parse(userId);
        
        var newPayment = new Payment
        {
            ApplicationUserId = applicationUserId,
            PlanId = createPaymentRequest.PlanId,
            CreatedAt = DateTime.UtcNow,
            IsDone = false
        };
        await _dbContext.Payments.AddAsync(newPayment);
        await _dbContext.SaveChangesAsync();

        return newPayment;
    }
}