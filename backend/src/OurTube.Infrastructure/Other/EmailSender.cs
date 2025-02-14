using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using Microsoft.Extensions.Configuration;


namespace OurTube.Infrastructure.Other
{
    public class EmailSender:IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _fromEmail;
        private readonly string _password;

        public EmailSender(IConfiguration configuration)
        {
            _smtpServer = configuration["SMTP:Server"];
            _port = configuration.GetValue<int>("SMTP:Port");
            _fromEmail = configuration["SMTP:Email"];
            _password = configuration["SMTP:Password"];
        }
        

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using SmtpClient client = new SmtpClient(_smtpServer, _port)
            {
                Credentials = new NetworkCredential(_fromEmail, _password),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage(_fromEmail, email, subject, message)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
