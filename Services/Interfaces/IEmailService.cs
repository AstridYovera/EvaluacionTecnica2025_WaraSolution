﻿namespace Wara.Api.Services.Interfaces;

public interface IEmailService
{
    Task SendAsync(string toEmail, string subject, string htmlBody);
}
