using Application.DTOs;
using Application.DTOs.Notification.req;
using Microsoft.AspNetCore.Mvc;
using VioVid.Core.Entities;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    
    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(ApiResult<List<UserNotification>>.Success(await _notificationService.GetNotificationsAsync()));
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateReadStatus([FromBody] UpdateReadStatusReq updateReadStatusReq)
    {
        return Ok(ApiResult<bool>.Success(await _notificationService.UpdateReadStatusAsync(updateReadStatusReq)));
    }
}