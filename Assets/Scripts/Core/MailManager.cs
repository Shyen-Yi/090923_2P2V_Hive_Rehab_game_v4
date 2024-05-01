using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Mail;
using System.IO;
using System;
using System.Net.Mime;

namespace com.hive.projectr
{
    public class MailManager : SingletonBase<MailManager>, ICoreManager
    {
        #region Lifecycle
        public void OnInit()
        {
        }

        public void OnDispose()
        { 
        }
#endregion

        public void SendData(string senderEmail, string senderPassword, List<string> receiverEmails, List<string> attachmentPaths, string subject, string body, Action onComplete)
        {
            if (string.IsNullOrEmpty(senderEmail))
            {
                Logger.LogError($"Empty sender email");
                return;
            }

            if (receiverEmails == null || receiverEmails.Count < 1)
            {
                Logger.LogError($"No receiver");
                return;
            }

            // mail
            var mail = new MailMessage();
            mail.From = new MailAddress(senderEmail);
            foreach (var email in receiverEmails)
            {
                mail.To.Add(email);
            }
            mail.Subject = subject;
            mail.Body = body;

            // attachments
            if (attachmentPaths != null &&  attachmentPaths.Count > 0)
            {
                foreach (var path in attachmentPaths)
                {
                    if (File.Exists(path)) 
                    {
                        // use MemoryStream so that it won't keep the actual file locked while sending
                        var ms = new MemoryStream();
                        using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            file.CopyTo(ms);
                        }
                        ms.Position = 0; // Reset memory stream position for reading during send

                        var fileName = Path.GetFileName(path);
                        var attachment = new Attachment(ms, fileName);
                        mail.Attachments.Add(attachment);
                    }
                }
            }

            // server
            var smtpServer = new SmtpClient("smtp-mail.outlook.com");
            smtpServer.Port = 587;
            smtpServer.Credentials = new NetworkCredential(senderEmail, senderPassword);
            smtpServer.EnableSsl = true;

            // send
            smtpServer.SendCompleted += (sender, e) =>
            {
                var mail = e.UserState as MailMessage;
                try
                {
                    if (e.Error == null)
                    {
                        Console.WriteLine("Email sent successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Failed to send email. Error: " + e.Error.Message);
                    }
                }
                finally
                {
                    if (mail != null)
                    {
                        // Dispose all attachments (which will dispose their content streams)
                        foreach (var attachment in mail.Attachments)
                        {
                            attachment.ContentStream.Dispose();
                        }
                        mail.Attachments.Dispose();
                        mail.Dispose();
                    }

                    ((SmtpClient)sender).Dispose();

                    onComplete?.Invoke();
                }
            };

            smtpServer.SendAsync(mail, null);
        }
    }
}