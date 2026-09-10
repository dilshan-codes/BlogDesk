using System.Security.Cryptography;
using System.Text;

namespace BlogDesk.Api.Services;

public static class HandleGenerator
{
    // "sarah.k+blog@gmail.com" -> "sarah.k" -> if collision risk, append a short stable hash suffix.
    public static string FromEmail(string email)
    {
        var local = email.Split('@')[0].Split('+')[0]; // strip +tags and domain
        var cleaned = new string(local.Where(c => char.IsLetterOrDigit(c) || c == '.' || c == '_').ToArray());
        if (string.IsNullOrWhiteSpace(cleaned)) cleaned = "reader";

        // Short deterministic suffix from the FULL email so two different people
        // who both type "sarah.k" locally (different domains) don't collide.
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLowerInvariant()));
        var suffix = Convert.ToHexString(hash)[..4].ToLowerInvariant();

        return $"{cleaned}-{suffix}";
    }
}