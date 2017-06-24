using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MimeKit;
using YsrisCoreLibrary.Extensions;
using YsrisCoreLibrary.Helpers;

namespace YsrisCoreLibrary.Services
{
    public class MailHelperService
    {
        private SessionHelperService SessionHelperInstance;
        private ILogger<MailHelperService> MyLogger;
        private IHostingEnvironment Env;

        public MailHelperService(SessionHelperService sessionHelper, ILogger<MailHelperService> logger, IHostingEnvironment env)
        {
            SessionHelperInstance = sessionHelper;
            MyLogger = logger;
            Env = env;
        }

        bool CustomCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Send an email
        /// </summary>
        /// <param name="from">sender</param>
        /// <param name="to">dest</param>
        /// <param name="subject">subject</param>
        /// <param name="htmlMsg">msg in html</param>
        public void SendMail(string from, string to, string subject, string htmlMsg)
        {
            MyLogger.LogInformation($"+MailHelper:SendMail(from:'{from}', to:'{to}', subject:'{subject}', htmlMsg:'{htmlMsg.Replace(Environment.NewLine, string.Empty)}')");

            var smtpServerDns = ConfigurationHelper.SmtpServer;
            var SmtpServer = new SmtpClient();
            SmtpServer.ServerCertificateValidationCallback = CustomCertificateValidationCallback;
            SmtpServer.Connect(smtpServerDns, Convert.ToInt32(ConfigurationHelper.SmtpPort), Convert.ToBoolean(ConfigurationHelper.SmtpEnableSsl) ? SecureSocketOptions.Auto : SecureSocketOptions.None);
            if (!string.IsNullOrEmpty(ConfigurationHelper.SmtpLogin))
            {
                SmtpServer.Authenticate(ConfigurationHelper.SmtpLogin, ConfigurationHelper.SmtpPassword);
            }

            //EnableSsl = true;
            //UseDefaultCredentials = true

            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(from));
            mail.To.Add(new MailboxAddress(to));
            mail.Subject = subject;

            mail.Body = new BodyBuilder() {HtmlBody = htmlMsg, TextBody = htmlMsg }.ToMessageBody();

            SmtpServer.Send(mail);
            MyLogger.LogInformation($"-MailHelper:SendMail");
        }

        public void SendMail(string to, string templateUri, string subject, Dictionary<string, string> mailViewBag)
        {
            string from = ConfigurationHelper.SmtpLogin;
            var htmlContent = File.ReadAllText(templateUri);
            mailViewBag.ForEach(a => htmlContent = htmlContent.Replace($"**{a.Key}**", a.Value));
            SendMail(from, to, subject, htmlContent);
        }
    }
}
