namespace support.Domain
{
    public class Users
{
    public Guid Id { get; set; }
    public required string CompanyName { get; set; }
    public required string UserName { get; set; }
    public required string Email {get;set;}
    public required string Password { get; set; }
    public bool Supporting { get; set; } = true;
    public DateTime SupportExpiry { get; set; }
    public string Role { get; set; } = "User"; 
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
}

}