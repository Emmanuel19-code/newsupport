namespace support.Domain
{
   public class SystemAdminDb
{
    public Guid Id { get; set; }
    public string AdminUserName { get; set; }
    public string AdminUserEmail { get; set; }
    public string AdminPassword { get; set; }
    public string AssignedTo { get; set; }
    public string Role { get; set; } = "SuperAdmin"; // Default role
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiry { get; set; }
    public DateTime LastLoggedIn { get; set; }
    public DateTime CreatedAt { get; set; }
}

}