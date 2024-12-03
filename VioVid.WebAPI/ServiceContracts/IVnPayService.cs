using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IVnPayService
{
    Task<string> CreatePaymentUrl(Payment payment, HttpContext context);
    Task<bool> VerifyPayment(Dictionary<string, string> vnpParams);
}