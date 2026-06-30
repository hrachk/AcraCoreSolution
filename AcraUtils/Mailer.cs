using System.Net.Mail;

namespace AcraUtils
{
    public class Mailer
    {
        //public void SendErrorMessage(string subject, string errorMessage, string host)
        //{
        //    MailMessage mail = new MailMessage("noreply@acra.am", "anna.badalyan@acra.am");           
        //    SmtpClient client = new SmtpClient();
        //    client.Port = 25;
        //    client.DeliveryMethod = SmtpDeliveryMethod.Network;
        //    client.UseDefaultCredentials = false;
        //    client.Host = "192.168.0.11";
        //    mail.Subject = subject;
        //    mail.Body = errorMessage;
        //    client.Send(mail);
        //}

        public void SendErrorMessage(string host, int port, string from, string to, string subject, string errorMessage )
        {            
            MailMessage mail = new MailMessage(from, to);
            SmtpClient client = new SmtpClient();
            client.Port = port;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = host;
            mail.Subject = subject;
            mail.Body = errorMessage;
            client.Send(mail);
        }
    }  
}
