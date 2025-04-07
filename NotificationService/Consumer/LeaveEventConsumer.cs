using CommonService.Shared.Models;
using MassTransit;
using System.Net.Mail;
using System.Net;

namespace NotificationService.Consumer
{
    public class LeaveEventConsumer : IConsumer<LeaveEvent>
    {
        public Task Consume(ConsumeContext<LeaveEvent> context)
        {
            var message = context.Message;
            string body = "Leave " + message.Status;
            SendMail(message.ToUser, "Leave", body);

            return Task.CompletedTask;
        }

        private async static Task<string> SendMail(string toEmail, string subject, string body)
        {
            var smtpServer = "smtp.gmail.com";
            var port = 587; // Use 465 for SSL
            var fromEmail = "testsmartdata06@gmail.com";
            var appPassword = "wplx toho cbvj rpjl";

            try
            {
                // Create the MailMessage
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "SmartData Test"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Set to true if your email body is HTML
                };
                mailMessage.To.Add(toEmail);
                mailMessage.To.Add("kamalsinghgaira@smartdatainc.net");

                // Configure the SMTP client
                var smtpClient = new SmtpClient(smtpServer, port)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(fromEmail, appPassword)
                };

                // Send the email
                smtpClient.Send(mailMessage);
                return "Email sent successfully!";
            }
            catch (Exception ex)
            {
                return $"Failed to send email: {ex.Message}";
            }
        }
    }
}
