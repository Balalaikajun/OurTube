using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;


namespace OurTube.Infrastructure.Other
{
    public class EmailSender:IEmailSender
    {
        private readonly string _smtpServer = "smtp.gmail.com"; // Сервер SMTP
        private readonly int _port = 587; // Порт (587 для TLS, 465 для SSL)
        private readonly string _fromEmail = "ribaetofish@gmail.com"; // Твой e-mail
        private readonly string _password = "Kluev2006"; // Пароль или app-password

        

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            using SmtpClient client = new SmtpClient(_smtpServer,_port)
            {
                Credentials = new NetworkCredential(_fromEmail, _password)
            };

            MailMessage mailMessage = new MailMessage(_fromEmail, email, subject, message)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
