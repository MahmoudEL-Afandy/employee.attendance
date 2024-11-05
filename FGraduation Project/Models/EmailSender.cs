using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace FGraduation_Project.Models
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailsender = "mahmoudeldrenyelafandy2000@gmail.com";
            var emailSenderPass = "xampdtbskjhtisho";
            var mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(emailsender);
            mailMessage.To.Add(email);
            mailMessage.Subject = subject;
            mailMessage.Body = $"<html><body>{htmlMessage}</html></body>";
            mailMessage.IsBodyHtml = true;
            using(SmtpClient  smtpClient = new SmtpClient()) 
            { 
              
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailsender, emailSenderPass);
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port = 587;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(mailMessage);


            
            }
        }
    }
}
