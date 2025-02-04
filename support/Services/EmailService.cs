using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace support.Services
{
    
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _email;
        private readonly string _password;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _email = _configuration.GetValue<string>("EmailSetup:Email");
            _password = _configuration.GetValue<string>("EmailSetup:EmailPassword");
        }
        public async Task  SendEmailAsync(string CompanyName,string To,string Body,string Subject)
        {
            var mail = new MimeMessage();
           mail.From.Add(new MailboxAddress("TENTH GENERATION TECHNOLOGY SYSTEM",_email));
           mail.To.Add(new MailboxAddress(CompanyName,To));
           mail.Subject = Subject;
           mail.Body = new TextPart(TextFormat.Html) { Text = Body };
            Console.WriteLine(_email,_password);
           using var client = new SmtpClient(); 
           client.Connect("smtp.gmail.com", 587,  SecureSocketOptions.StartTls); 
           client.Authenticate(_email,_password);
           await client.SendAsync(mail);
           client.Disconnect(true);
        }
    }
}