namespace BlogDesk.Api.Models;

public class AdminSession
{
    public long Id { get; set; }
    public string Token { get; set; } = "";           // opaque session token, not a JWT
    public string Email { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}