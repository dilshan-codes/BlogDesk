namespace BlogDesk.Api.Models;

public class Comment
{
    public long Id { get; set; }
    public string PostSlug { get; set; } = "";
    public string ReaderEmail { get; set; } = "";     // private
    public string DisplayHandle { get; set; } = "";    // derived once, immutable, shown publicly
    public string Body { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool Hidden { get; set; } = false;           // soft-delete from admin panel
}