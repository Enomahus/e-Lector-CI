using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordEmail(string? link, string toEmail);
        Task ValidateRegistrationRequestEmail(string? link, string toEmail);
        Task ActivateAccountEmail(string? link, string toEmail);
    }
}
