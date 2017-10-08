using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
namespace ludoux.DuduSpider
{
    class Mail
    {
        private string _emailFromAdress;
        private string _host;
        private string _userName;
        private string _password;
        private int _imapSslPort;
        private List<string> _emailToAdress;
        private string _mobiPath;
        public Mail(string emailFromAdress, string host, string userName, string passpord, int imapSslPort, List<string> emailToAdress, string mobiPath)
        {
            _emailFromAdress = emailFromAdress;
            _host = host;
            _userName = userName;
            _password = passpord;
            _imapSslPort = imapSslPort;
            _emailToAdress = emailToAdress;
            _mobiPath = mobiPath;

            LogWriter.WriteLine("Start sending email!");
            foreach(string to in _emailToAdress)
            {
                LogWriter.WriteLine("Sending to " + to + "...");
                if (Send(to))
                    LogWriter.WriteLine("Sended to " + to + ".");
                else
                    LogWriter.WriteLine("Error: Failed to send mail to " + to + ".");
            }
        }
        private bool Send(string to)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(_emailFromAdress, _userName, Encoding.UTF8);
            message.To.Add(to);
            message.Subject = "convert";
            message.SubjectEncoding = Encoding.UTF8;
            message.Body = "";
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = false;
            message.Attachments.Add(new Attachment(_mobiPath));
            SmtpClient client = new SmtpClient(_host, _imapSslPort);
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential(_emailFromAdress, _password);
            try
            {
                client.Send(message);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
