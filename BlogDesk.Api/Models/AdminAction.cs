namespace BlogDesk.Api.Models;

public class AdminAction
{
    public long Id { get; set; }
    public string AdminEmail { get; set; } = "";
    public string Action { get; set; } = "";           // e.g. "hide_comment", "delete_rating"
    public string? TargetSlug { get; set; }
    public string? Details { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}