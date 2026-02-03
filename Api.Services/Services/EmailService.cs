using Api.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Api.Services.Services;

/// <summary>
/// Service for sending Emails with Google SMTP
/// </summary>
/// <seealso cref="Api.Services.Interfaces.IEmailService" />
public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="config">The configuration.</param>
    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    /// <inheritdoc/>
    public async Task SendAsync(string to, string subject, string body)
    {
        SmtpClient smtp = new SmtpClient(_config["Smtp:Host"])
        {
            Port = int.Parse(_config["Smtp:Port"]!),
            Credentials = new NetworkCredential(
                _config["Smtp:User"],
                _config["Smtp:Pass"]
            ),
            EnableSsl = true
        };

        MailMessage mail = new MailMessage
        {
            From = new MailAddress(_config["Smtp:From"]!),
            Subject = subject,
            Body = body
        };

        mail.To.Add(to);

        await smtp.SendMailAsync(mail);
    }
}