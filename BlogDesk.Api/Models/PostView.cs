namespace BlogDesk.Api.Models;

public class PostView
{
    public long Id { get; set; }
    public string PostSlug { get; set; } = "";
    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    // Not tied to a person -- just a raw counter row, so we can also compute
    // views-over-time later without extra migrations.
}