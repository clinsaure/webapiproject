using SendGrid;

namespace WebApiProject.Api.Services
{
    public interface IEmailService
    {
        Task<Response> SendEMailAsync(string userEmail, string bodySubject, string bodyMessage);
    }
}
