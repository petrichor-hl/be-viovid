using Application.DTOs;
using Application.DTOs.Dashboard;
using Application.DTOs.Dashboard.Req;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Entities;
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
    
    [HttpGet("registration-stats")]
    public async Task<IActionResult> GetUserRegistrationStats([FromQuery] GetUserRegistrationStatsRequest getUserRegistrationStatsRequest)
    {
        return Ok(ApiResult<List<int>>.Success(await _dashboardService.GetRegistrationsPerMonthAsync(getUserRegistrationStatsRequest)));
    }
}