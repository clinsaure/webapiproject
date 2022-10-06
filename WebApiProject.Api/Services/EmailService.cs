using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace WebApiProject.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<Response> SendEMailAsync(string userEmail, string bodySubject, string bodyMessage)
    {
        try
        {
            var apiKey = _configuration.GetSection("EmailConfig:API_KEY").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_configuration.GetSection("EmailConfig:Email_KEY").Value, _configuration.GetSection("EmailConfig:SendName_Key").Value);
            var subject = bodySubject;
            var to = new EmailAddress(userEmail, userEmail);
            var plainTextContent = bodyMessage;
            var htmlContent = $"<strong>{bodyMessage}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

            return response;

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}
