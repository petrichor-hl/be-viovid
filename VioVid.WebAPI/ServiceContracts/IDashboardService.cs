using Application.DTOs.Dashboard;
using Application.DTOs.Dashboard.Req;
namespace VioVid.WebAPI.ServiceContracts;

public interface IDashboardService
{
    Task<List<int>> GetRegistrationsPerMonthAsync(GetUserRegistrationStatsRequest getUserRegistrationStatsRequest);
}