using AskFm.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;
using AskFm.BLL.DTO;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AskFm.BLL.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;
    private readonly EmailSettings _emailSettings;

    // Inject IConfiguration and ILogger via the constructor
    public EmailSender(IConfiguration config)
    {
        _config = config;
        _emailSettings = new EmailSettings()
        {
            From = _config.GetValue<string>("EmailOption:from"),
            Client = _config.GetValue<string>("EmailOption:client"),
            Password = _config.GetValue<string>("EmailOption:password"),
            Port = _config.GetValue<int>("EmailOption:port"),
        };
    }

    public async Task<ServiceResult<bool>> SendConfirmationLinkAsync(string email, string confirmationLink)
    {
        string subject = "Confirm Your Email for AskFm";
        string body = $@"
            <h1>Welcome to AskFm!</h1>
            <p>Thanks for registering. Please confirm your email address by clicking the link below:</p>
            <p><a href='{confirmationLink}'>Confirm My Email</a></p>
            <p>If you did not create an account, you can safely ignore this email.</p>
            <br>
            <p>Thank you,</p>
            <p>The AskFm Team</p>";

        return await SendEmailAsync(email, subject, body);
    }


    public async Task<ServiceResult<bool>> SendPasswordResetLinkAsync(string email, string resetLink)
    {
        string subject = "Reset Your AskFm Password";
        string body = $@"
            <h1>Password Reset Request</h1>
            <p>We received a request to reset your password. You can reset your password by clicking the link below:</p>
            <p><a href='{resetLink}'>Reset My Password</a></p>
            <p>If you did not request a password reset, please ignore this email.</p>
            <br>
            <p>Thank you,</p>
            <p>The AskFm Team</p>";

        return await SendEmailAsync(email, subject, body);
    }

    public async Task<ServiceResult<bool>> SendPasswordResetCodeAsync( string email, string resetCode)
    {
        string subject = "Your AskFm Password Reset Code";
        string body = $@"
            <h1>Password Reset Code</h1>
            <p>We received a request to reset your password. Use the following code to complete the process:</p>
            <h2><strong>{resetCode}</strong></h2>
            <p>This code will expire shortly. If you did not request a password reset, please ignore this email.</p>
            <br>
            <p>Thank you,</p>
            <p>The AskFm Team</p>";

        return await SendEmailAsync(email, subject, body);
    }


    public async Task<ServiceResult<bool>> SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {

       var client = new SmtpClient(_emailSettings.Client, 587)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_emailSettings.From, _emailSettings.Password)
        };

        // Create and send the email
        await client.SendMailAsync(
            new MailMessage(from: _emailSettings.From,
                to: toEmail,
                subject,
                htmlMessage
            ));
        return await ServiceResult<bool>.Success(true);
    }
}