using Microsoft.AspNetCore.Mvc;
using BlogDesk.Api.Data;
using BlogDesk.Api.DTOs;
using BlogDesk.Api.Models;
using BlogDesk.Api.Services;

namespace BlogDesk.Api.Controllers;

[ApiController]
[Route("api/posts")]
public class EngagementController : ControllerBase
{
    private readonly AppDbContext _db;
    public EngagementController(AppDbContext db) => _db = db;

    // Called once per page load from inside the iframe (debounced client-side, see 4.4).
    [HttpPost("view")]
    public IActionResult PingView(ViewPingRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.PostSlug)) return BadRequest();
        _db.PostViews.Add(new PostView { PostSlug = req.PostSlug });
        _db.SaveChanges();
        return Ok();
    }

    [HttpGet("{slug}/stats")]
    public ActionResult<PostStatsResponse> GetStats(string slug)
    {
        var views = _db.PostViews.Count(v => v.PostSlug == slug);
        var ratings = _db.PostRatings.Where(r => r.PostSlug == slug).ToList();
        var comments = _db.Comments.Count(c => c.PostSlug == slug && !c.Hidden);

        return Ok(new PostStatsResponse(
            slug, views,
            ratings.Count > 0 ? ratings.Average(r => r.Stars) : 0,
            ratings.Count, comments));
    }

    // Upsert: re-rating the same post with the same email updates instead of duplicating
    // (enforced by the unique index from Step 1.3).
    [HttpPost("rate")]
    public IActionResult Rate(RatingRequest req)
    {
        if (req.Stars < 1 || req.Stars > 5) return BadRequest("Stars must be 1-5.");
        if (!IsValidEmail(req.Email)) return BadRequest("A valid email is required.");

        var existing = _db.PostRatings.FirstOrDefault(r =>
            r.PostSlug == req.PostSlug && r.ReaderEmail == req.Email.Trim().ToLowerInvariant());

        if (existing != null)
        {
            existing.Stars = req.Stars;
        }
        else
        {
            _db.PostRatings.Add(new PostRating
            {
                PostSlug = req.PostSlug,
                ReaderEmail = req.Email.Trim().ToLowerInvariant(),
                Stars = req.Stars
            });
        }
        _db.SaveChanges();
        return Ok();
    }

    [HttpGet("{slug}/comments")]
    public ActionResult<List<CommentResponse>> GetComments(string slug)
    {
        var comments = _db.Comments
            .Where(c => c.PostSlug == slug && !c.Hidden)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CommentResponse(c.Id, c.DisplayHandle, c.Body, c.CreatedAt))
            .ToList();
        return Ok(comments);
    }

    [HttpPost("comment")]
    public ActionResult<CommentResponse> PostComment(CommentRequest req)
    {
        if (!IsValidEmail(req.Email)) return BadRequest("A valid email is required.");
        if (string.IsNullOrWhiteSpace(req.Body) || req.Body.Length > 2000)
            return BadRequest("Comment must be 1-2000 characters.");

        var email = req.Email.Trim().ToLowerInvariant();
        var comment = new Comment
        {
            PostSlug = req.PostSlug,
            ReaderEmail = email,
            DisplayHandle = HandleGenerator.FromEmail(email),
            Body = req.Body.Trim()
        };
        _db.Comments.Add(comment);
        _db.SaveChanges();

        return Ok(new CommentResponse(comment.Id, comment.DisplayHandle, comment.Body, comment.CreatedAt));
    }

    private static bool IsValidEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) &&
        System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
}