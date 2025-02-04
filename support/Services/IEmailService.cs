namespace support.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string CompanyName,string To,string Body,string Subject);
    }
}
