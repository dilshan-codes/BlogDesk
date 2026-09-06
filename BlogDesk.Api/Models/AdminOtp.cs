namespace BlogDesk.Api.Models;

public class AdminOtp
{
    public long Id { get; set; }
    public string Email { get; set; } = "";
    public string CodeHash { get; set; } = "";     // never store the raw code
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; } = false;
    public int Attempts { get; set; } = 0;           // brute-force guard
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}