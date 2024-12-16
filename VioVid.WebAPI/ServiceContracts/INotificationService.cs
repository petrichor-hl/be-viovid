using Application.DTOs.Notification.req;
using VioVid.Core.Entities;
using UserNotification = VioVid.Core.Entities.UserNotification;

namespace VioVid.WebAPI.ServiceContracts;

public interface INotificationService
{
    Task<List<UserNotification>> GetNotificationsAsync();
    
    Task<bool> UpdateReadStatusAsync(UpdateReadStatusReq updateReadStatusReq);
}