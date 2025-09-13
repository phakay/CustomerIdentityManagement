using IdentityManagement.API.Core.Infrastructure;
using IdentityManagement.API.Core.Security.Models;

namespace IdentityManagement.API.Infrastructure
{
    public class SmsService : ISmsService
    {
        public async Task SendAync(SecurityMessage message)
        {
            Console.WriteLine($"Sending sms to {message.To}");
            Console.WriteLine($"Sms Subject: {message.Subject}");
            Console.WriteLine($"Sms Body: {message.Body}");

            await Task.CompletedTask;
        }
    }
}
