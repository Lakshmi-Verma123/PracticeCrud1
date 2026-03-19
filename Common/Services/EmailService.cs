using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace PracticeCrud1.Common
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendOtpAsync(string toEmail, string otp)
        {
            var smtpHost = _config["Email:SmtpHost"];
            var smtpPort = int.Parse(_config["Email:SmtpPort"]);
            var smtpUser = _config["Email:SmtpUser"];
            var smtpPass = _config["Email:SmtpPass"];

            using (var client = new SmtpClient(smtpHost, smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(smtpUser, smtpPass);

                var message = new MailMessage();
                message.From = new MailAddress(smtpUser, "Your App Name");
                message.To.Add(toEmail);
                message.Subject = "Your OTP Code";
                message.Body = $"Your OTP code is: {otp}";
                message.IsBodyHtml = true;

                await client.SendMailAsync(message);
            }
        }
    }
}