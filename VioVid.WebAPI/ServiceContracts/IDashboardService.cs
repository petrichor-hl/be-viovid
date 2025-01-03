using Application.DTOs.Dashboard.Req;
using Application.DTOs.Dashboard.Res;

namespace VioVid.WebAPI.ServiceContracts;

public interface IDashboardService
{
    Task<List<int>> GetRegistrationsPerMonthAsync(int year);

    Task<List<PaymentSummaryResponse>> GetPaymentSummaryPerMonthAsync(int year);
    
    Task<List<TopViewFilmResponse>> GetTopViewedFilmsAsync(GetTopViewsRequest getTopViewsRequest);
}