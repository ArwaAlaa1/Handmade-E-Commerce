using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using ECommerce.Core.Services.Contract.SendEmail;
using ECommerce.DashBoard.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Services
{
    public class EmailProvider : IEmailProvider
    {
        private readonly IConfiguration _config;
        private readonly ECommerceDbContext _context;

        public EmailProvider(IConfiguration config, ECommerceDbContext context)
        {
            _context = context;
            _config = config;
        }

     

        public async Task<int> SendResetCode(string Email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null) return 0;

            string subject;
            string templatePath;

            Random rnd = new Random();
            int pin = rnd.Next(1, 999999);
            string pinStr = pin.ToString();
            while (pinStr.Length < 6)
            {
                pinStr = "0" + pinStr;
            }
      
        subject = "Vervify email";
            templatePath = Directory.GetCurrentDirectory() + "/wwwroot/Email.html";
            
            string htmlTemplate = System.IO.File.ReadAllText(templatePath);

            htmlTemplate = htmlTemplate.Replace("{UserName}", user.UserName);
            htmlTemplate = htmlTemplate.Replace("{VerificationCode}", pinStr);

            var message = new MailMessage();
            message.From = new MailAddress("qassemshaban397@gmail.com");
            message.To.Add(new MailAddress(user.Email));
            message.Subject = subject;
            message.Body = htmlTemplate;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient(_config["stmp:Host"], int.Parse(_config["stmp:Port"])))
            {
                smtp.Credentials = new NetworkCredential("qassemshaban397@gmail.com", "kirp afby zxeq csxn");
                smtp.EnableSsl = true;

                try
                {
                    await smtp.SendMailAsync(message);
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }

            return pin;
        }
        public async Task<string> SendConfirmAccount(string Email, string UrlConfirmation)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null) return "User Not Found";

            string subject;
            string templatePath;
            string body;

            subject = "Confirm Your Email Address";
            //body = $"Hi {user.DisplayName},\r\n\r\nThank you for registering with Handmade E-Commerce!\r\nTo complete your registration and verify your email address, please click the link below:" +
            //    $"{UrlConfirmation}";
            templatePath = Directory.GetCurrentDirectory() + "/wwwroot/EmailConfirm.html";
            //D:\Final\ECommerce\ECommerce.DashBoard\wwwroot\EmailConfirm.html
            string htmlTemplate = System.IO.File.ReadAllText(templatePath);

            htmlTemplate = htmlTemplate.Replace("{user.DisplayName}", user.DisplayName);
            htmlTemplate = htmlTemplate.Replace("{UrlConfirmation}", UrlConfirmation);

            var message = new MailMessage();
            message.From = new MailAddress("qassemshaban397@gmail.com");
            message.To.Add(new MailAddress(user.Email));
            message.Subject = subject;
            message.Body = htmlTemplate;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient(_config["stmp:Host"], int.Parse(_config["stmp:Port"])))
            {
                smtp.Credentials = new NetworkCredential("qassemshaban397@gmail.com", "kirp afby zxeq csxn");
                smtp.EnableSsl = true;

                try
                {
                    await smtp.SendMailAsync(message);
                }
                catch (SmtpException ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }

            return "Done";
        }

    }
}