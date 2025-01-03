using Application.DTOs;
using Application.DTOs.Dashboard.Res;
using Microsoft.AspNetCore.Mvc;
using VioVid.WebAPI.ServiceContracts;
namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    
    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }
    
    [HttpGet("registration-stats/{year:int}")]
    public async Task<IActionResult> GetUserRegistrationStats(int year)
    {
        return Ok(ApiResult<List<int>>.Success(await _dashboardService.GetRegistrationsPerMonthAsync(year)));
    }
    
    [HttpGet("payment-summary/{year:int}")]
    public async Task<IActionResult> GetPaymentSummary(int year)
    {
        return Ok(ApiResult<List<PaymentSummaryResponse>>.Success(await _dashboardService.GetPaymentSummaryPerMonthAsync(year)));
    }
}