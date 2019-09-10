using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AkExpenses.Api.Services
{
    public interface IMailService
    {

        Task<bool> SendMailAsync(string subject, string body, string mailTo);

    }

    public class MailService : IMailService
    {
        IConfiguration configuration;

        public MailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<bool> SendMailAsync(string subject, string body, string mailTo)
        {
            // Get the smtp 
            var emailSectionKeys = configuration.GetSection("MailKeys");
            var myServer = emailSectionKeys["MailServer"];
            var username = emailSectionKeys["Username"];
            var password = emailSectionKeys["Password"];
            var email = emailSectionKeys["MyEmail"];
            var port = Convert.ToInt32(emailSectionKeys["Port"]);

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(username, email));
            message.To.Add(new MailboxAddress(mailTo));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                client.SslProtocols = System.Security.Authentication.SslProtocols.Tls;

                client.ServerCertificateValidationCallback += (s, c, h, e) =>
                {
                    return true;
                };
                try
                {
                    Debug.Write("Connecting...");
                    await client.ConnectAsync(myServer, port, false);
                    Debug.WriteLine("Done!");
                    Debug.Write("Authenticating...");
                    await client.AuthenticateAsync(email, password);
                    Debug.WriteLine("Done!");
                    Debug.Write("Sending...");
                    await client.SendAsync(message);
                    Debug.WriteLine("Done!");
                    await client.DisconnectAsync(true);
                    Debug.WriteLine("Disconnected");
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

        }
    }
}
