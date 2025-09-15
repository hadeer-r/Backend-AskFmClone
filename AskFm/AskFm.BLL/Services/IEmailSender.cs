using AskFm.DAL.Models;
using Microsoft.AspNetCore.Identity;

namespace AskFm.BLL.Services;

public interface IEmailSender
{
    public Task<ServiceResult<bool>> SendPasswordResetLinkAsync( string email, string resetLink);
    public Task<ServiceResult<bool>> SendConfirmationLinkAsync(string email, string confirmationLink);
    public Task<ServiceResult<bool>> SendPasswordResetCodeAsync( string email, string resetCode);
    Task<ServiceResult<bool>> SendEmailAsync (string email, string subject, string message);
}
