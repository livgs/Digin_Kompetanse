using System.Threading.Tasks;

namespace Digin_Kompetanse.Services
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }
}