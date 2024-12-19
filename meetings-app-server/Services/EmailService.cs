//// Services/EmailService.cs
//using MimeKit;
//using MailKit.Net.Smtp;
//using System;
////using System.Net.Mail;

//namespace MyApp.Services
//{
//    public class EmailService
//    {
//        private readonly string _smtpServer = "smtp.yourprovider.com";  // Replace with your SMTP server
//        private readonly string _smtpUser = "snehit5541@gmial.com";    // Replace with your email
//        private readonly string _smtpPassword = "";         // Replace with your email password or app password
//        private readonly int _smtpPort = 587;                           // Use 587 for TLS, or 465 for SSL

//        public void SendRegistrationEmail(string userEmail, string userName)
//        {
//            var message = new MimeMessage();
//            message.From.Add(new MailboxAddress("Your App Name", _smtpUser));
//            message.To.Add(new MailboxAddress(userName, userEmail));
//            message.Subject = "Welcome to Our App!";

//            message.Body = new TextPart("plain")
//            {
//                Text = $"Dear {userName},\n\nThank you for registering with us. We're excited to have you on board."
//            };

//            try
//            {
//                using (var client = new SmtpClient())
//                {
//                    client.Connect(_smtpServer, _smtpPort, false); // Use your SMTP details
//                    client.Authenticate(_smtpUser, _smtpPassword); // Use your credentials
//                    client.Send(message);
//                    client.Disconnect(true);
//                }
//            }
//            catch (Exception ex)
//            {
//                // Log error here (e.g., log to a file or database)
//                throw new Exception("Email sending failed", ex);
//            }
//        }


//    }
//}
