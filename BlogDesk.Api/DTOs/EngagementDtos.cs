namespace BlogDesk.Api.DTOs;

public record ViewPingRequest(string PostSlug);
public record RatingRequest(string PostSlug, string Email, int Stars);
public record CommentRequest(string PostSlug, string Email, string Body);
public record CommentResponse(long Id, string DisplayHandle, string Body, DateTime CreatedAt);
public record PostStatsResponse(string PostSlug, long Views, double AverageStars, int RatingCount, int CommentCount);