namespace VioVid.Core.ServiceContracts
{
    public interface IEmailSender
    {
        Task SendMailAsync(string email, string subject, string message);
    }
}

