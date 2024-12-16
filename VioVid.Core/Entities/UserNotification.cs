using VioVid.Core.Enum;

namespace VioVid.Core.Entities;

public class UserNotification
{
    public Guid Id { get; set; }
    public Guid? ApplicationUserId { get; set; }
    public NotificationCategory Category { get; set; }
    public DateTime CreatedDateTime { get; set; }
    public NotificationReadStatus ReadStatus { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    
    public Dictionary<string, object> Params { get; set; } = new Dictionary<string, object>();
}