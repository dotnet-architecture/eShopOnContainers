﻿using System.Threading.Tasks;

namespace Identity.API.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
