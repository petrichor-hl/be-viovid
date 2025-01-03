namespace Application.DTOs.Dashboard.Res;

public class PaymentSummaryResponse
{
    public int Month { get; set; }
    public int Momo { get; set; }
    public int VnPay { get; set; }
    public int Stripe { get; set; }
}