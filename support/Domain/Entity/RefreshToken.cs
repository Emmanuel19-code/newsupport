

namespace support.Domain
{
    public class RefreshTokenDb
{
    public Guid Id { get; set; }
    public string RefreshToken { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UserId { get; set; }  
}

}