using Application.DTOs.Payment;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IMomoService
{
    Task<string> CreatePaymentUrl(Payment payment);
    
    Task<bool> HandleRecord(MomoCallbackRequest momoCallbackRequest);
}