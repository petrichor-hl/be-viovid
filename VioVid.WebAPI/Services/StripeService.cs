using System.Text.Json;
using Application.DTOs.Payment;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using VioVid.Core.Entities;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;
using SessionService = Stripe.Checkout.SessionService;

namespace VioVid.WebAPI.Services;

public class StripeService : IStripeService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _dbContext;
    private readonly IPaymentService _paymentService;
    public StripeService(IConfiguration configuration, ApplicationDbContext dbContext, IPaymentService paymentService)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _paymentService = paymentService;
    
        // Set Stripe API key
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }
    
    public async Task<string> CreatePaymentSession(Payment payment)
    {
        // Get payment info from the database
        var plan = await _dbContext.Plans.FindAsync(payment.PlanId);
        if (plan == null)
        {
            throw new NotFoundException($"Không tìm thấy Plan {payment.PlanId}");
        }

        // Create the Stripe payment session
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "vnd",
                        UnitAmount = plan.Price,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = plan.Name
                        }
                    },
                    Quantity = 1
                }
            },
            Metadata = new Dictionary<string, string>
            {
                { "PaymentId", payment.Id.ToString() }
            },
            Mode = "payment",
            SuccessUrl = _configuration["Stripe:SuccessUrl"] + "?sessionId={CHECKOUT_SESSION_ID}",
            CancelUrl = _configuration["Stripe:CancelUrl"] + "?sessionId={CHECKOUT_SESSION_ID}",
            
        };
    
        var service = new SessionService();
        var session = await service.CreateAsync(options);
    
        // Return the URL to redirect the user to the Stripe Checkout page
        return session.Url;
    }
    
    public async Task<bool> HandleRecord(StripeCallbackRequest stripeCallbackRequest)
    {
        var service = new SessionService();
        var session = await service.GetAsync(stripeCallbackRequest.sessionId);

        if (session == null)
        {
            throw new NotFoundException($"Không tìm thấy Session {stripeCallbackRequest.sessionId}");
        }
        
        
        var paymentId = session.Metadata["PaymentId"];

        if (paymentId == null) 
        {
            throw new NotFoundException("Không tìm thấy PaymentId");
        }

        Payment? payment;

        // Try to parse the value from vnpParams["vnp_TxnRef"] into a Guid
        if (Guid.TryParse(paymentId, out var parsedPaymentId))
        {
            // Now you can use parsedPaymentId to query the database or further logic
            payment = await _dbContext.Payments
                .Include(p => p.ApplicationUser)
                .Include(p => p.Plan)
                .FirstOrDefaultAsync(p => p.Id == parsedPaymentId);
            
            if (payment == null)
            {
                throw new NotFoundException($"Không tìm thấy PaymentId {parsedPaymentId}");
            }
            
            if (payment.IsDone)
            {
                return true;
            }
        }
        else
        {
            throw new InvalidModelException("PaymentId không hợp lệ");
        }

        var isSuccess = session.PaymentStatus == "paid";
        
        Console.WriteLine(session.AmountTotal);
        
        await _paymentService.UpdatePaymentAndPushNoti(payment, Convert.ToInt32(session.AmountTotal), isSuccess);

        return isSuccess;
    }

    public async Task ExpireSession(StripeCallbackRequest stripeCallbackRequest)
    {
        var service = new SessionService();
        var session = await service.GetAsync(stripeCallbackRequest.sessionId);

        if (session == null)
        {
            throw new NotFoundException($"Không tìm thấy Session {stripeCallbackRequest.sessionId}");
        }
        
        var paymentId = session.Metadata["PaymentId"];

        if (paymentId == null) 
        {
            throw new NotFoundException("Không tìm thấy PaymentId");
        }

        Payment? payment;

        // Try to parse the value from vnpParams["vnp_TxnRef"] into a Guid
        if (Guid.TryParse(paymentId, out var parsedPaymentId))
        {
            // Now you can use parsedPaymentId to query the database or further logic
            payment = await _dbContext.Payments
                .Include(p => p.ApplicationUser)
                .Include(p => p.Plan)
                .FirstOrDefaultAsync(p => p.Id == parsedPaymentId);
            
            if (payment == null)
            {
                throw new NotFoundException($"Không tìm thấy PaymentId {parsedPaymentId}");
            }
        }
        else
        {
            throw new InvalidModelException("PaymentId không hợp lệ");
        }

        // Expire this session
        await service.ExpireAsync(stripeCallbackRequest.sessionId);
        
        // Push Noti
        await _paymentService.UpdatePaymentAndPushNoti(payment, Convert.ToInt32(session.AmountTotal), false);
    }
}