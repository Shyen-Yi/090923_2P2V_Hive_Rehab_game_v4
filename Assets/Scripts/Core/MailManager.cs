using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Mail;
using System.IO;
using System;

namespace com.hive.projectr
{
    /// @ingroup Core
    /// @class MailManager
    /// @brief Manages email sending functionality, including sending messages with attachments via an SMTP server.
    /// 
    /// The `MailManager` class is responsible for sending emails from a specified sender to one or more recipients.
    /// It supports including attachments in the email and works asynchronously using an SMTP client. This class also manages
    /// the lifecycle of email sending, from initializing the email configuration to handling the completion of the email send process.
    public class MailManager : SingletonBase<MailManager>, ICoreManager
    {
        #region Lifecycle
        /// <summary>
        /// Initializes the MailManager. This method is called when the system is initialized.
        /// </summary>
        public void OnInit()
        {
        }

        /// <summary>
        /// Disposes of the MailManager. This method is called when the system is disposed.
        /// </summary>
        public void OnDispose()
        { 
        }
        #endregion

        /// <summary>
        /// Sends an email with optional attachments asynchronously.
        /// </summary>
        /// <param name="senderEmail">The email address of the sender.</param>
        /// <param name="senderPassword">The password for the sender's email account.</param>
        /// <param name="receiverEmails">A list of email addresses to receive the message.</param>
        /// <param name="attachmentPaths">A list of file paths to attach to the email.</param>
        /// <param name="subject">The subject of the email.</param>
        /// <param name="body">The body content of the email.</param>
        /// <param name="onComplete">A callback that will be invoked when the email has been sent.</param>
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
                        Logger.Log("Email sent successfully!");
                    }
                    else
                    {
                        Logger.LogError("Failed to send email. Error: " + e.Error.Message);
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