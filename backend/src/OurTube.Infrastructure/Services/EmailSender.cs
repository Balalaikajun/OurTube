using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace OurTube.Infrastructure.Other;

public class EmailSender : IEmailSender
{
    private readonly string _fromEmail;
    private readonly string _password;
    private readonly int _port;
    private readonly string _smtpServer;
    private readonly ILogger<EmailSender> _logger;

    private static bool? _smptEnabled = null;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _logger = logger;
        _smtpServer = configuration["SMTP:Server"];
        _port = configuration.GetValue<int>("SMTP:Port");
        _fromEmail = configuration["SMTP:Email"];
        _password = configuration["SMTP:Password"];
    }


    public async Task SendEmailAsync(string email, string subject, string message)
    {
        if (!_smptEnabled.HasValue)
            _smptEnabled = await CheckSmtpConnectionAsync();

        if (!_smptEnabled.Value)
        {
            _logger.LogWarning("SMTP was not configured");
            return;
        }
        
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(MailboxAddress.Parse(_fromEmail));
        emailMessage.To.Add(MailboxAddress.Parse(email));
        emailMessage.Subject = subject;

        emailMessage.Body = new TextPart("plain") 
        {
            Text = message
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(_smtpServer, _port, SecureSocketOptions.StartTlsWhenAvailable); 

        await client.AuthenticateAsync(_fromEmail, _password);

        await client.SendAsync(emailMessage);

        await client.DisconnectAsync(true);
    }
    
    public async Task<bool> CheckSmtpConnectionAsync()
    {
        try
        {
            using var client = new SmtpClient();

            client.Timeout = 5000;

            // Подключение
            await client.ConnectAsync(_smtpServer, _port, SecureSocketOptions.StartTlsWhenAvailable);

            // Аутентификация
            if (!string.IsNullOrWhiteSpace(_fromEmail) && !string.IsNullOrWhiteSpace(_password))
            {
                await client.AuthenticateAsync(_fromEmail, _password);
            }

            await client.DisconnectAsync(true);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SMTP check failed: {ex.Message}");
            return false;
        }
    }

}