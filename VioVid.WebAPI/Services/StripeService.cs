using System.Text.Json;
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
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StripeService(IConfiguration configuration, ApplicationDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;

        // Set Stripe API key
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    public async Task<string> CreatePaymentSession(Payment payment)
    {
        // Get payment info from the database
        var plan = await _dbContext.Plans.FindAsync(payment.PlanId);
        if (plan == null) throw new ArgumentException("Invalid PlanId");

        // Log the payment request and plan details for debugging
        Console.WriteLine($"Payment Request details: {JsonSerializer.Serialize(payment)}");
        Console.WriteLine($"Plan details: {JsonSerializer.Serialize(plan)}");

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
            SuccessUrl = _configuration["Stripe:SuccessUrl"] + "?session_id={CHECKOUT_SESSION_ID}",
            CancelUrl = _configuration["Stripe:CancelUrl"]
        };

        var service = new SessionService();
        var session = service.Create(options);

        // Return the URL to redirect the user to the Stripe Checkout page
        return session.Url;
    }

    public async Task<bool> Success(string sessionId)
    {
        Console.WriteLine($"Success Session details: {JsonSerializer.Serialize(sessionId)}");
        if (string.IsNullOrEmpty(sessionId)) return false;
        try
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            // Verify that the session is paid
            if (session.PaymentStatus == "paid")
            {
                //Extract from metadata
                // Extract the ApplicationUserId and PlanId from metadata
                var paymentId = session.Metadata["PaymentId"];
                Console.WriteLine($"paymentId: {JsonSerializer.Serialize(paymentId)}");

                if (string.IsNullOrEmpty(paymentId)) return false;

                Payment? payment = null;

                // Try to parse the value from vnpParams["vnp_TxnRef"] into a Guid
                if (Guid.TryParse(paymentId, out var parsedPaymentId))
                {
                    Console.WriteLine($"Payment Id successfully parsed: {parsedPaymentId}");

                    // Now you can use parsedPaymentId to query the database or further logic
                    payment = await _dbContext.Payments.FindAsync(parsedPaymentId);
                    if (payment == null)
                    {
                        Console.WriteLine("No payment record found with the given PaymentId.");
                        return false;
                    }

                    Console.WriteLine($"Payment: {JsonSerializer.Serialize(payment)}");
                }
                else
                {
                    Console.WriteLine("Invalid PaymentId format.");
                    return false;
                }

                //Update payment status
                payment.IsDone = true;

                var applicationUserId = payment.ApplicationUserId;
                var planId = payment.PlanId;

                //Get plan
                var plan = await _dbContext.Plans.FirstOrDefaultAsync(p =>
                    p.Id == planId);

                //Update the UserPlan 
                var userPlan = await _dbContext.UserPlans
                    .FirstOrDefaultAsync(up =>
                        up.ApplicationUserId == applicationUserId && up.PlanId == planId);

                if (userPlan != null)
                {
                    // Update existing plan if needed
                    userPlan.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
                    userPlan.EndDate = userPlan.StartDate.AddDays(plan!.Duration);

                    _dbContext.UserPlans.Update(userPlan);
                }
                else
                {
                    // Create a new user plan
                    var newUserPlan = new UserPlan
                    {
                        Id = Guid.NewGuid(),
                        ApplicationUserId = applicationUserId,
                        PlanId = planId,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow
                            .AddDays(plan!.Duration))
                    };

                    await _dbContext.UserPlans.AddAsync(newUserPlan);
                }

                //Update to plan order number
                plan.Order += 1;
                _dbContext.Plans.Update(plan);

                await _dbContext.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during success: {ex.Message}");
            return false;
        }

        return false;
    }

    public bool Cancelled(string sessionId)
    {
        return true;
    }
}