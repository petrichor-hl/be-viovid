using Application.DTOs.Notification.req;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using VioVid.Core.Entities;
using VioVid.Core.Enum;
using VioVid.Infrastructure.DatabaseContext;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class NotificationService : INotificationService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public NotificationService(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<List<UserNotification>> GetNotificationsAsync()
    {
        var user = _httpContextAccessor.HttpContext?.User!;
        var userIdClaim = user.FindFirst("UserId");
        var applicationUserId = Guid.Parse(userIdClaim!.Value);

        return await _dbContext.UserNotifications
            .Where(userNotification => userNotification.ApplicationUserId == applicationUserId || userNotification.ApplicationUserId == null)
            .OrderByDescending(userNotification => userNotification.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<bool> UpdateReadStatusAsync(UpdateReadStatusReq updateReadStatusReq)
    {
        var notification = await _dbContext.UserNotifications
            .FindAsync(updateReadStatusReq.NotificationId);

        if (notification == null)
        {
            throw new NotFoundException($"Không tìm thấy Notification {updateReadStatusReq.NotificationId}.");
        }

        notification.ReadStatus = NotificationReadStatus.Read;
        await _dbContext.SaveChangesAsync();
        return true;
    }
}