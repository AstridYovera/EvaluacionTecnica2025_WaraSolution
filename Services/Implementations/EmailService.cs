using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Wara.Api.Services.Interfaces;

namespace Wara.Api.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) => _config = config;

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var s = _config.GetSection("Email");

        using var client = new SmtpClient(s["Host"]!)
        {
            Port = int.Parse(s["Port"]!),
            EnableSsl = bool.Parse(s["UseSsl"] ?? "true"),
            Credentials = new NetworkCredential(s["User"], s["Password"])
        };

        var from = new MailAddress(s["FromAddress"]!, s["FromName"]);
        var to = new MailAddress(toEmail);

        using var msg = new MailMessage(from, to)
        {
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        await client.SendMailAsync(msg);
    }
}
