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
using Microsoft.Extensions.Configuration;

namespace YsrisCoreLibrary.Services
{
    public class MailHelperService
    {
        private ILogger<MailHelperService> MyLogger;
        private IHostingEnvironment Env;
        private readonly IConfiguration _conf;

        public MailHelperService(ILogger<MailHelperService> logger, IHostingEnvironment env, IConfiguration conf)
        {
            MyLogger = logger;
            Env = env;
            _conf = conf;
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
        public void SendMail(string from, IEnumerable<string> to, string subject, string htmlMsg)
        {
            MyLogger.LogInformation($"+MailHelper:SendMail(from:'{from}', to:'{to}', subject:'{subject}', htmlMsg:'{htmlMsg.Replace(Environment.NewLine, string.Empty)}')");
            var smtpServerDns = _conf.GetValue<string>("Data:SmtpServer");
            var SmtpServer = new SmtpClient();
            SmtpServer.ServerCertificateValidationCallback = CustomCertificateValidationCallback;
            SmtpServer.Connect(smtpServerDns, Convert.ToInt32(_conf.GetValue<string>("Data:SmtpPort")), Convert.ToBoolean(_conf.GetValue<string>("Data:SmtpEnableSsl")) ? SecureSocketOptions.Auto : SecureSocketOptions.None);
            if (!string.IsNullOrEmpty(_conf.GetValue<string>("Data:SmtpLogin")))
            {
                SmtpServer.Authenticate(_conf.GetValue<string>("Data:SmtpLogin"), _conf.GetValue<string>("Data:SmtpPassword"));
            }

            //EnableSsl = true;
            //UseDefaultCredentials = true

            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(from));

            to.ForEach(a => mail.To.Add(new MailboxAddress(a)));
            mail.Subject = subject;
            mail.Body = new BodyBuilder() { HtmlBody = htmlMsg, TextBody = htmlMsg }.ToMessageBody();

            SmtpServer.Send(mail);
            MyLogger.LogInformation($"-MailHelper:SendMail");
        }

        public void SendMail(string to, string templateUri, string subject, Dictionary<string, string> mailViewBag)
        {
            string from = _conf.GetValue<string>("Data:SmtpLogin");
            var htmlContent = File.ReadAllText(templateUri);
            mailViewBag.ForEach(a => htmlContent = htmlContent.Replace($"**{a.Key}**", a.Value));
            SendMail(from, new List<string> { to }, subject, htmlContent);
        }
    }
}
