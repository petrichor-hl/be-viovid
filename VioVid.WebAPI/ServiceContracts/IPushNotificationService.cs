namespace VioVid.WebAPI.ServiceContracts;

public interface IPushNotificationService
{
    Task PushNotificationToTopicAsync(string title, string body, Dictionary<string, string> data, string topic);
    
    Task PushNotificationToIndividualDevice(string title, string message, Dictionary<string, object> data, string fcmToken);
}