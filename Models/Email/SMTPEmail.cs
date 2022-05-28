using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace BQC.Models.Email
{
    public class SMTPEmail
    {
        private string smtpServer;

        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Cc { get; set; }
        public string ErrorMsg { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public SMTPEmail()
        {
            GetConfiguration();
        }

        public static bool SendEmail(List<string> recipients, string subject, string msg)
        {
            try
            {
                MailMessage message = new MailMessage();

                string emailFrom = "no-reply@croudace.co.uk";
                string fromDisplayName = "Croudace Homes";
                string smtpHost = "mailsend";

                message.From = new MailAddress(emailFrom, fromDisplayName);
                message.To.Clear();
                foreach (string recipient in recipients)
                {
                    MailAddress toAddress = new MailAddress(recipient);
                    message.To.Add(toAddress);
                }

                message.Subject = subject;
                message.Body = msg;
                message.IsBodyHtml = true;

                SmtpClient client = new SmtpClient(smtpHost);

                client.Send(message);

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private bool GetConfiguration()
        {
            smtpServer = "mailsend";
            return true;
        }
        public static bool SendEmailWithAttachment(List<string> recipients, string subject, string msg, string attachmentName, Stream attachment)
        {
            MailMessage mm = new MailMessage();
            SmtpClient smtp = new SmtpClient();
            bool bln = false;

            string emailFrom = ConfigurationManager.AppSettings["NoReplyEmailAddress"].ToString();
            string fromDisplayName = ConfigurationManager.AppSettings["EmailDisplayName"].ToString();
            string smtpHost = ConfigurationManager.AppSettings["SMTPServerAddress"].ToString();

            MailAddress fromAddress = new MailAddress(emailFrom, fromDisplayName);

            foreach (string recipient in recipients)
            {
                if (recipient != null)
                {
                    MailAddress toAddress = new MailAddress(recipient);
                    mm.To.Add(toAddress);
                }
            }
            mm.From = fromAddress;
            mm.IsBodyHtml = true;
            mm.Subject = subject;
            mm.Body = msg;
            mm.Attachments.Add(new Attachment(attachment, attachmentName));

            try
            {
                smtp.Host = smtpHost;
                smtp.Send(mm);
                bln = true;
            }
            catch (Exception ex)
            {
                // RaiseError(ex);
                bln = false;
            }

            return bln;
        }
    }
}