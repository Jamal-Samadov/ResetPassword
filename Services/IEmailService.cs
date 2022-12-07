using FlowerSite.Data;

namespace FlowerSite.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(RequestEmail requestEmail);
    }
}
