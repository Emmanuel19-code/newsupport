namespace support.Domain
{
    public class Users
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool Supporting { get; set; } = true;
    public DateTime SupportExpiry { get; set; }
    public string Role { get; set; } = "User"; // Default role
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
}

}