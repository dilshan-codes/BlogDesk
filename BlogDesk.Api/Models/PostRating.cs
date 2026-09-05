namespace BlogDesk.Api.Models;

public class PostRating
{
    public long Id { get; set; }
    public string PostSlug { get; set; } = "";
    public string ReaderEmail { get; set; } = "";   // private, never returned to clients
    public int Stars { get; set; }                   // 1-5
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}