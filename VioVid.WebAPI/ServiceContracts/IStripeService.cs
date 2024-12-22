using Application.DTOs.Payment;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IStripeService
{
    Task<string> CreatePaymentSession(Payment payment);
    Task<bool> HandleRecord(StripeCallbackRequest stripeCallbackRequest);
    Task ExpireSession(StripeCallbackRequest stripeCallbackRequest);
}