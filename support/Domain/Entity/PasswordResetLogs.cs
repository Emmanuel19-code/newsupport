namespace support.Domain
{
    public class PasswordResetLogs
{
    public Guid Id { get; set; }
    public DateTime RequestDate { get; set; }
    public Guid UserId { get; set; }
    public string CompanyName { get; set; }
}

}