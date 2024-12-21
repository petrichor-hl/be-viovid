using Application.DTOs.Payment;
using VioVid.Core.Entities;

namespace VioVid.WebAPI.ServiceContracts;

public interface IPaymentService
{
    Task<Payment> CreatePayment(CreatePaymentRequest createPaymentRequest);

    Task UpdatePaymentAndPushNoti(Payment payment, int amount, bool isSuccess);
}