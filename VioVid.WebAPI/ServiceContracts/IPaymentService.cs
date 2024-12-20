using Application.DTOs.Payment;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPaymentService
{
    Task<Payment> CreatePayment(CreatePaymentRequest createPaymentRequest);

    // Task<Payment> UpdatePayment(Payment payment);
}