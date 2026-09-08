using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using BlogDesk.Api.Data;
using BlogDesk.Api.DTOs;
using BlogDesk.Api.Models;
using BlogDesk.Api.Services;

namespace BlogDesk.Api.Controllers;

[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly EmailService _email;
    private readonly IConfiguration _config;

    public AdminAuthController(AppDbContext db, EmailService email, IConfiguration config)
    {
        _db = db; _email = email; _config = config;
    }

    // Step 1: ask for a code. Always returns the same generic message.
    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp(RequestOtpRequest req)
    {
        var allowed = _config["Admin:AllowedEmail"];
        const string generic = "If that email is authorized, a code has been sent.";

        if (!string.Equals(req.Email, allowed, StringComparison.OrdinalIgnoreCase))
            return Ok(new { message = generic }); // don't reveal whether it matched

        // Rate limit: max 3 outstanding requests in the last 10 minutes.
        var recentCount = _db.AdminOtps.Count(o =>
            o.Email == req.Email && o.CreatedAt > DateTime.UtcNow.AddMinutes(-10));
        if (recentCount >= 3)
            return Ok(new { message = generic }); // silently drop, still generic

        var code = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
        var otp = new AdminOtp
        {
            Email = req.Email,
            CodeHash = BCrypt.Net.BCrypt.HashPassword(code),
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        };
        _db.AdminOtps.Add(otp);
        await _db.SaveChangesAsync();

        await _email.SendOtpAsync(req.Email, code);
        return Ok(new { message = generic });
    }

    // Step 2: verify the code, issue a session token.
    [HttpPost("verify-otp")]
    public async Task<ActionResult<AdminSessionResponse>> VerifyOtp(VerifyOtpRequest req)
    {
        var otp = _db.AdminOtps
            .Where(o => o.Email == req.Email && !o.Used && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefault();

        if (otp is null) return Unauthorized("Code expired or not found.");
        if (otp.Attempts >= 5) return Unauthorized("Too many attempts. Request a new code.");

        otp.Attempts++;

        if (!BCrypt.Net.BCrypt.Verify(req.Code, otp.CodeHash))
        {
            await _db.SaveChangesAsync();
            return Unauthorized("Incorrect code.");
        }

        otp.Used = true;

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var session = new AdminSession
        {
            Token = token,
            Email = req.Email,
            ExpiresAt = DateTime.UtcNow.AddHours(12)
        };
        _db.AdminSessions.Add(session);
        await _db.SaveChangesAsync();

        return Ok(new AdminSessionResponse(token, session.ExpiresAt));
    }
}