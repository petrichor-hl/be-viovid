using Application.DTOs.Payment;

namespace VioVid.WebAPI.ServiceContracts;

public interface IVnPayService
{
    Task<string> CreatePaymentUrl(CreatePaymentRequest createPaymentRequest, HttpContext context);
    Task<bool> VerifyPayment(Dictionary<string, string> vnpParams);
}