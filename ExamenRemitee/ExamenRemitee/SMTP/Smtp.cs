using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ExamenRemitee.SMTP
{
    public class Smtp
    {
        public static void EnviarCorreo(string mensaje)
        {
            try
            {
                using MailMessage correo = new MailMessage();
                correo.From = new MailAddress("juancomande@gmail.com", "Error proceso CurrencyLayer", System.Text.Encoding.UTF8);
                correo.To.Add("juancomande@gmail.com");
                correo.Subject = "Error proceso CurrencyLayer";
                correo.Body = $"Hubo un error en el proceso a las {DateTime.Now}. {Environment.NewLine} {mensaje}";
                correo.IsBodyHtml = false;
                correo.Priority = MailPriority.Normal;
                using SmtpClient smtp = new SmtpClient();
                smtp.UseDefaultCredentials = false;
                smtp.Host = "smtp.gmail.com"; //Host del servidor de correo
                smtp.Port = 25; //Puerto de salida
                smtp.Credentials = new NetworkCredential("juancomande@gmail.com", "peperodrigez12");//Cuenta de correo
                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                smtp.EnableSsl = true;//True si el servidor de correo permite ssl
                smtp.Send(correo);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
