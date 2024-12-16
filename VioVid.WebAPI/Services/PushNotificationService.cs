using FirebaseAdmin.Messaging;
using VioVid.WebAPI.ServiceContracts;

namespace VioVid.WebAPI.Services;

public class PushNotificationService : IPushNotificationService
{
    public async Task PushNotificationToTopicAsync(string title, string body, Dictionary<string, string> data, string topic)
    {
        var message = new Message
        {
            Topic = topic,
            Notification = new Notification
            {
                Title = title,
                Body = body,
            },
            Data = data,
            Android = new AndroidConfig()
            {
                TimeToLive = TimeSpan.FromHours(1),
                Notification = new AndroidNotification()
                {
                    // Icon = "stock_ticker_update", // Tên icon phải trùng với tên file trong res/drawable
                    Color = "#000000",
                },
            },
        };
        
        var response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        Console.WriteLine("Successfully sent message: " + response);
    }

    public Task PushNotificationToIndividualDevice(string title, string message, Dictionary<string, object> data, string fcmToken)
    {
        throw new NotImplementedException();
    }
}